using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ResolutionSettings : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;

    struct Res { public int w; public int h; }

    private List<Res> fixedRes = new List<Res> {
        new Res { w = 960, h = 540 },   
        new Res { w = 1024, h = 576 },  
        new Res { w = 1280, h = 720 },  
        new Res { w = 1600, h = 900 },  
        new Res { w = 1920, h = 1080 } 
    };

    private int selectedResIndex;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangeLocale;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLocale;
    }

    private void Start()
    {
        StartCoroutine(InitScreenSetting());
    }

    private IEnumerator InitScreenSetting()
    {
        // 로컬라이제이션 설정이 로드될 때까지 대기
        yield return LocalizationSettings.InitializationOperation;

        InitResolutions();
        InitScreenMode();

        Locale currentLocale = LocalizationSettings.SelectedLocale;
        OnChangeLocale(currentLocale);
    }

    #region Resolution
    private void InitResolutions()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < fixedRes.Count; i++)
        {
            options.Add($"{fixedRes[i].w} x {fixedRes[i].h}");
            if (fixedRes[i].w == Screen.width && fixedRes[i].h == Screen.height)
            {
                selectedResIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = selectedResIndex;
        resolutionDropdown.RefreshShownValue();

        // 화면 모드 드롭다운 초기화
        int modeIndex = (Screen.fullScreenMode == FullScreenMode.Windowed) ? 1 : 0;
        if (screenModeDropdown != null)
        {
            screenModeDropdown.value = modeIndex;
            screenModeDropdown.RefreshShownValue();
        }
    }

    public void SetResolution(int index)
    {
        selectedResIndex = index;
        Res res = fixedRes[selectedResIndex];

        // 현재 설정된 전체화면 모드를 그대로 유지하며 해상도만 변경
        Screen.SetResolution(res.w, res.h, Screen.fullScreenMode);
    }
    #endregion

    #region ScreenMode
    private void InitScreenMode()
    {
        screenModeDropdown.ClearOptions();

        List<string> options = new List<string>()
        {
            "전체화면",
            "창모드"
        };

        screenModeDropdown.AddOptions(options);

        screenModeDropdown.value = Screen.fullScreenMode switch
        {
            FullScreenMode.FullScreenWindow => 0,
            FullScreenMode.Windowed => 1,
            _ => 0
        };

        screenModeDropdown.RefreshShownValue();
    }

    public void SetScreenMode(int index)
    {
        FullScreenMode mode = index switch
        {
            0 => FullScreenMode.FullScreenWindow, // 전체 화면
            1 => FullScreenMode.Windowed,         // 창모드
            _ => FullScreenMode.FullScreenWindow
        };

        // 현재 선택되어 있는 해상도 값을 가져와서 모드와 함께 적용
        Res res = fixedRes[selectedResIndex];
        Screen.SetResolution(res.w, res.h, mode);
    }
    #endregion

    #region Localization

    public void OnChangeLocale(Locale locale)
    {
        StartCoroutine(ChangeLocaleInteractRoutine(locale));
    }

    private IEnumerator ChangeLocaleInteractRoutine(Locale locale)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("UITable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            // 초기화
            screenModeDropdown.ClearOptions();

            string fullText = table.GetEntry("UI_ScreenFull").LocalizedValue;
            string windowText = table.GetEntry("UI_ScreenWindow").LocalizedValue;

            // DropDown Localization 갱신
            List<string> options = new List<string>()
            {
                fullText,
                windowText
            };

            screenModeDropdown.AddOptions(options);
            screenModeDropdown.RefreshShownValue();
        }
    }

    #endregion
}
