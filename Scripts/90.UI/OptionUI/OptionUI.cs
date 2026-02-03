using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using WhereAreYouLookinAt.Enum;
public enum OptionOpenContext
{
    Lobby,   // Start / Lobby 씬
    Battle   // Battle 씬 (EscUI에서 열림)
}
public class OptionUI : MonoBehaviour
{
    [Header("패널 UI")]
    [SerializeField] private GameObject window_Option;
    //[SerializeField] private Stack<GameObject> panels = new Stack<GameObject>();
    //[SerializeField] private GameObject panel_Button;
    //[SerializeField] private GameObject panel_Audio;
    //[SerializeField] private GameObject panel_Language;
    //[SerializeField] private GameObject panel_Resolution;

    [Header("언어 세팅")]
    [SerializeField] private TMP_Dropdown language_Dropdown;

    [Header("로비 이동")]
    [SerializeField] private GameObject btn_Exit;

    [Header("애니메이션 세팅")]
    [Header("애니메이션 시간")]
    [SerializeField] private float animationDuration;
    private Coroutine optionAnimCoroutine;

    private bool isLocalChanging;
    private OptionOpenContext openContext;

    private void Reset()
    {
        window_Option = GameObject.Find("Window_Option");

        language_Dropdown = transform.FindChild<TMP_Dropdown>("Dropdown_Language");

        btn_Exit = GameObject.Find("Btn_Exit");
    }

    private void Awake()
    {
        language_Dropdown.ClearOptions();

        language_Dropdown.AddOptions(new List<string>
        {
            "한국어",
            "English",
        });

        isLocalChanging = false;
        language_Dropdown.value = 0;      // 기본 선택
        language_Dropdown.RefreshShownValue();

        InitPanel();
        SetWindow(false);
    }

    #region 패널 설정

    public void SetWindow(bool active)
    {
        window_Option.SetActive(active);
    }

    public bool IsWindowOpen()
    {
        return window_Option.activeSelf;
    }

    private void InitPanel()
    {
        btn_Exit.SetActive(false);
    }

    public void OpenFromLobby()
    {
        openContext = OptionOpenContext.Lobby;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.State = GameState.PAUSE;
        }

        Time.timeScale = 0f;

        InitPanel();
        SetWindow(true);

        // 로비면 나가기 버튼 표시
        if (SceneController.Instance.CurrentScene == 0 || SceneController.Instance.CurrentScene == 1)
            btn_Exit.SetActive(true);
    }
    public void OpenFromBattle()
    {
        openContext = OptionOpenContext.Battle;

        InitPanel();
        SetWindow(true);

        // Time.timeScale 만지지 않음
        // GameState 만지지 않음
    }
    public void OpenPanel(GameObject panel)
    {
        return;
    }

    public void CloseCurrentPanel()
    {
        SetWindow(false);

        if (openContext == OptionOpenContext.Lobby)
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
                GameManager.Instance.State = GameState.RUNNING;
        }
        else if (openContext == OptionOpenContext.Battle)
        {
            EscUI escUI = FindObjectOfType<EscUI>();
            if (escUI != null)
                escUI.Unlock();
        }
    }
    public void ExitFromBattle()
    {
        // OptionPanel만 닫기
        SetWindow(false);

        // EscPanel로 돌아가기
        EscUI escUI = FindObjectOfType<EscUI>();
        if (escUI != null)
            escUI.Unlock();
    }
    #endregion

    #region 언어 설정

    public void OnChanged(int index)
    {
        if (isLocalChanging)
            return;

        int changedIndex = 1;

        switch(index)
        {
            case 0:
                changedIndex = 3;
                break;
            case 1:
                changedIndex = 1;
                break;
            default:
                break;
        }

        StartCoroutine(ChangeRoutine(changedIndex));
    }

    private IEnumerator ChangeRoutine(int index)
    {
        isLocalChanging = true;

        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        isLocalChanging = false;
    }

    #endregion

    #region 로비 이동

    public void OnClickExit(int index)
    {
        SceneController.Instance.LoadScene(index);
        //CloseCurrentPanel();
    }

    #endregion
}
