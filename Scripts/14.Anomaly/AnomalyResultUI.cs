using NPOI.POIFS.Properties;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AnomalyResultUI : MonoBehaviour
{
    [Header("이상현상 결과 UI BG")]
    [Header("이상현상 정답 BG")]
    [SerializeField] private Sprite anomalyBenfitBG;
    [Header("이상현상 오답 BG")]
    [SerializeField] private Sprite anomalyPenaltyBG;

    [Header("컴포넌트")]
    [SerializeField] private GameObject anomalyResultWindow;
    [SerializeField] private Image anomalyResultBG;
    [SerializeField] private TextMeshProUGUI anomalyResultTxt;
    [SerializeField] private Image anomalyResultIcon;

    [Header("애니메이션 세팅")]
    [Header("결과 현상 UI 나오는데 까지 시간")]
    [SerializeField] private float startDuration = 5f;
    [Header("BG 펴지는 애니메이션 시간")]
    [SerializeField] private float resultBGDuration;
    [Header("투명도 애니메이션 시간")]
    [SerializeField] private float resultTxtDuration;
    [Header("결과 UI 유지되는 시간")]
    [SerializeField] private float displayDuration = 2.0f; 

    private Coroutine changeLocalCoroutine;
    private Coroutine showResultUICoroutine;
    private bool isShowResultUI = false;
    private string key;

    private void Reset()
    {
        anomalyResultWindow = GameObject.Find("AnomalyUIWindow");
        anomalyResultBG = transform.FindChild<Image>("AnomalyResultBG");
        anomalyResultTxt = transform.FindChild<TextMeshProUGUI>("AnomalyResultText");
        anomalyResultIcon = transform.FindChild<Image>("AnomalyResultImage");
    }


    private void OnEnable()
    {
        EventBus.OnAnomalyResultApply += StartAnomalyResultUI;
        LocalizationSettings.SelectedLocaleChanged += OnChangeLocale;
    }

    private void OnDisable()
    {
        EventBus.OnAnomalyResultApply -= StartAnomalyResultUI;
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLocale;
    }

    private void Awake()
    {
        anomalyResultWindow.SetActive(false);
    }

    public void StartAnomalyResultUI(AnomalyResultManager manager)
    {
        Sprite resultBG;
        string resultTxt;
        Sprite resultIcon;

        // Anomaly 이상현상 결과 보여주기
        if(manager.AnomalyResult == WhereAreYouLookinAt.Enum.AnomalyResultType.Benefit)
        {
            resultBG = anomalyBenfitBG;
            resultTxt = manager.BenefitManager.AnomalyBenefit.type.ToString();
            resultIcon = manager.BenefitManager.AnomalyBenefit.icon;
            manager.BenefitManager.AnomalyBenefit = null;
        }
        else
        {
            resultBG = anomalyPenaltyBG;
            resultTxt = manager.PenaltyManager.AnomalyPenalty.type.ToString();
            resultIcon = manager.PenaltyManager.AnomalyPenalty.icon;
            manager.PenaltyManager.AnomalyPenalty = null;
        }

        SetAnomalyResultUI(resultBG, resultTxt, resultIcon);
    }

    private void SetAnomalyResultUI(Sprite _anomalyResultBG, string resultTxt, Sprite resultIcon)
    {
        anomalyResultBG.sprite = _anomalyResultBG;
        anomalyResultIcon.sprite = resultIcon;
        Locale currentLocale = LocalizationSettings.SelectedLocale;

        isShowResultUI = true;

        key = resultTxt;
        anomalyResultTxt.text = string.Empty;
        OnChangeLocale(currentLocale);
    }

    public void OnChangeLocale(Locale locale)
    {
        if (changeLocalCoroutine != null) StopCoroutine(changeLocalCoroutine);
        changeLocalCoroutine = StartCoroutine(ChangeLocaleRoutine(locale));
    }

    private IEnumerator ChangeLocaleRoutine(Locale locale)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("AnomalyResultTable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(key)?.GetLocalizedString();
            anomalyResultTxt.text = text;
        }

        // 이상현상 결과 보여주는 애니메이션 동작
        if(isShowResultUI)
        {
            if (showResultUICoroutine != null) StopCoroutine(showResultUICoroutine);
            showResultUICoroutine = StartCoroutine(ShowResultUI());
        }
    }

    private IEnumerator ShowResultUI()
    {
        // 시작 대기
        yield return new WaitForSeconds(startDuration);

        // 초반 세팅
        anomalyResultTxt.gameObject.SetActive(false);
        anomalyResultIcon.gameObject.SetActive(false);
        anomalyResultWindow.SetActive(true);

        RectMask2D mask = anomalyResultTxt.GetComponent<RectMask2D>();
        float txtFullWidth = anomalyResultTxt.rectTransform.rect.width;
        mask.padding = new Vector4(0, 0, txtFullWidth, 0);

        CanvasGroup iconGroup = anomalyResultIcon.GetComponent<CanvasGroup>();
        CanvasGroup txtGroup = anomalyResultTxt.GetComponent<CanvasGroup>();
        CanvasGroup bgGroup = anomalyResultBG.GetComponent<CanvasGroup>();
        if (iconGroup == null) iconGroup = anomalyResultIcon.gameObject.AddComponent<CanvasGroup>();
        iconGroup.alpha = 0;
        txtGroup.alpha = 1;
        bgGroup.alpha = 0;

        // BG 애니메이션
        float elapsed = 0f;
        while (elapsed < resultBGDuration)
        {
            elapsed += Time.deltaTime;
            float percent = Mathf.SmoothStep(0, 1, elapsed / resultBGDuration);
            bgGroup.alpha = percent;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration * 0.5f);

        // 오브젝트 활성화
        anomalyResultTxt.gameObject.SetActive(true);
        anomalyResultIcon.gameObject.SetActive(true);

        // Icon, Text 연출
        elapsed = 0f;
        while (elapsed < resultTxtDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / resultTxtDuration;
            mask.padding = new Vector4(0, 0, Mathf.Lerp(txtFullWidth, 0, percent), 0);
            iconGroup.alpha = percent;
            yield return null;
        }
        mask.padding = Vector4.zero;
        iconGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < resultTxtDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float percent = 1 - (elapsed / (resultTxtDuration * 0.5f));

            txtGroup.alpha = percent;
            iconGroup.alpha = percent;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < resultBGDuration)
        {
            elapsed += Time.deltaTime;
            float percent = 1 - Mathf.SmoothStep(0, 1, elapsed / resultBGDuration);
            bgGroup.alpha = percent;
            yield return null;
        }

        bgGroup.alpha = 0f;
        anomalyResultWindow.SetActive(true);
    }
}
