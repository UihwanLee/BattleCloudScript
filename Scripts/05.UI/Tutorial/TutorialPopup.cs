using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [Header("UI-Window")]
    [SerializeField] private GameObject tutorialPopupWindow;

    [Header("UI - Title")]
    [SerializeField] private TMP_Text titleText;

    [Header("Content Layouts")]
    [SerializeField] private GameObject contentNormal;
    [SerializeField] private GameObject contentExtended;

    [Header("Normal Content")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Extended Content")]
    [SerializeField] private Image mainIcon;
    [SerializeField] private TMP_Text mainDescription;
    [SerializeField] private Image subIcon;
    [SerializeField] private TMP_Text subDescription;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    private List<TutorialPage> pages;
    private int currentPageIndex = 0;

    private Coroutine changeTutorialLocale;

    // TextKey
    private string titleKey;
    private string descKey;
    private string subDescKey;

    bool isExtended;

    public static bool IsOpen { get; private set; }

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);

        tutorialPopupWindow.SetActive(false);
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangeLocale;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLocale;
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    // 외부에서 호출
    public void Open(List<TutorialPage> tutorialPages)
    {
        if (tutorialPages == null || tutorialPages.Count == 0)
        {
            Debug.LogWarning("TutorialPopup: 페이지 데이터 없음");
            return;
        }

        pages = tutorialPages;
        currentPageIndex = 0;

        IsOpen = true;
        tutorialPopupWindow.SetActive(true);
        Refresh();
    }

    private void Close()
    {
        IsOpen = false;
        tutorialPopupWindow.SetActive(false);
    }

    private void NextPage()
    {
        if (pages == null)
            return;

        currentPageIndex++;

        if (currentPageIndex >= pages.Count)
            currentPageIndex = pages.Count - 1;

        Refresh();
    }

    private void PrevPage()
    {
        if (pages == null)
            return;

        currentPageIndex--;

        if (currentPageIndex < 0)
            currentPageIndex = 0;

        Refresh();
    }

    private void Refresh()
    {
        TutorialPage page = pages[currentPageIndex];

        isExtended = page.layoutType == TutorialLayoutType.Extended;

        // 레이아웃 스위칭만
        contentNormal.SetActive(!isExtended);
        contentExtended.SetActive(isExtended);

        titleKey = page.title;

        if (!isExtended)
        {
            iconImage.sprite = page.icon;
            descKey = page.description;
        }
        else
        {
            mainIcon.sprite = page.icon;
            descKey = page.description;

            subIcon.sprite = page.subIcon;
            subDescKey = page.subDescription;
        }

        Locale currentLocale = LocalizationSettings.SelectedLocale;
        OnChangeLocale(currentLocale);

        // 버튼 표시 제어
        prevButton.gameObject.SetActive(currentPageIndex > 0);
        nextButton.gameObject.SetActive(currentPageIndex < pages.Count - 1);
    }


    public void OnChangeLocale(Locale locale)
    {

        if (changeTutorialLocale != null) StopCoroutine(changeTutorialLocale);

        changeTutorialLocale = StartCoroutine(ChangeLocaleTutorialRoutine(locale));
    }

    private IEnumerator ChangeLocaleTutorialRoutine(Locale locale)
    {

        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("TutorialUITable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(pages[currentPageIndex].title)?.GetLocalizedString();
            titleText.text = text;

            if (!isExtended)
            {
                text = table.GetEntry(pages[currentPageIndex].description)?.GetLocalizedString();
                descriptionText.text = text;
            }
            else
            {
                text = table.GetEntry(pages[currentPageIndex].description)?.GetLocalizedString();
                mainDescription.text = text;

                text = table.GetEntry(pages[currentPageIndex].subDescription)?.GetLocalizedString();
                subDescription.text = text;
            }
        }
    }

}
