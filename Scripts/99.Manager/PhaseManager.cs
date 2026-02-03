using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class PhaseManager : MonoBehaviour
{
    [Header("기본시간 설정")]
    [SerializeField] private float basePhasetime = 20f;
    [Header("액트1 증가량 설정")]
    [SerializeField] private float firstActphasetime = 5f;
    [Header("액트2 설정")]
    [SerializeField] private float secondActphasetime = 5f;
    [Header("액트3 설정")]
    [SerializeField] private float thirdActphasetime = 5f;
    [Header("보스페이즈 시간 설정")]
    [SerializeField] private float bossPhasetime = 100f;
    [Header("맥스 시간 설정")]
    [SerializeField] private float MaxPhasetime = 60f;

    [Header("대기 시간 설정")]
    [SerializeField]
    private float PhaseDelay;

    [Header("페이드 설정")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 1f;

    [Header("UI 컴포넌트")]
    [SerializeField] private GameObject waveClearWindow;
    [SerializeField] private TextMeshProUGUI waveClearTxt;
    [SerializeField] private Transform goldTransform;

    [Header("연출 설정")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float magnetDuration = 1.0f;
    [SerializeField] private AnimationCurve magnetCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    public int CurrentPhase { get; private set; } = 0;
    private float timer = 0f;
    private bool isRunning = false;


    private void Start()
    {
        GameManager.Instance.PhaseManager = this;
    }

    private void Update()
    {
        if (!isRunning)
            return;


        timer += Time.deltaTime;

        if (CurrentPhase > 0)
        {
            float remaining = Mathf.Max(GetPhaseDuration(CurrentPhase) - timer, 0f);
            EventBus.OnTimerChanged?.Invoke(remaining, CurrentPhase);

            if (timer >= GetPhaseDuration(CurrentPhase))
                EndPhase();
        }
    }

    #region 페이즈 시작/종료
    public void StartPhase()
    {
        isRunning = true;
        timer = 0f;
        GameManager.Instance.Player.Controller.SetMovementEnabled(true);  // 플레이어 이동 제한
        StartCoroutine(PrePhaseDelayCoroutine());
    }

    public void EndPhase()
    {
        Debug.Log("페이즈종료메서드 호출");
        StartCoroutine(EndPhaseRoutine());
    }



    private void BeginPhase(int phase)// 페이즈 시작
    {
        CurrentPhase = phase;
        timer = 0f;
        GameManager.Instance.spawnManager.StartSpawn(); //몬스터 스폰 시작
        UIManager.Instance.SetEnableTimer(true); // 타이머 온
        Debug.Log($"페이즈 {phase} 시작");
        switch (phase)
        {
            case 1:
                StartFirstActPhase();
                break;
            case 2:
                StartSecondActPhase();
                break;
            case 3:
                StartThirdActPhase();
                break;
        }
    }

    #endregion




    #region 페이즈별 실행 메서드

    private void StartFirstActPhase()
    {

        //액트1 페이즈 진입시 실행할 메서드들
    }

    private void StartSecondActPhase()
    {

        //액트2 페이즈 진입시 실행할 메서드들
    }
    private void StartThirdActPhase()
    {
        //GameManager.Instance.
        //액트3 페이즈 진입시 실행할 메서드들
    }
    #endregion

    #region 액트별 페이즈 시간 가져오기
    private float GetPhaseDuration(int phase)
    {
        int day = GameManager.Instance.DayManager.CurrentDay;
        int act = GameManager.Instance.DayManager.CurrentAct;
        int maxDay = GameManager.Instance.DayManager.MaxDay;

        float baseTime = basePhasetime;
        float increment = 0f;

        switch (act)
        {
            case 1:
                increment = firstActphasetime;
                break;
            case 2:
                increment = secondActphasetime;
                break;
            case 3:
                increment = thirdActphasetime; // 필요하다면 추가
                break;
            default:
                break;
        }

        float resultPhase = baseTime + ((day - 1) * increment);

        if (phase == 3)
        {
            // 맥스데이일 때는 100초
            if (day == maxDay)
            {
                return 100f;
            }

            // 일반적으로는 최대 60초 제한
            return Mathf.Min(resultPhase, 60f);
        }

        // 다른 페이즈는 기존 최대값 제한
        return Mathf.Min(resultPhase, MaxPhasetime);

    }
    #endregion

    #region 코루틴
    private IEnumerator PrePhaseDelayCoroutine()
    {
        Debug.Log("대기 시간 시작");
        float delay = PhaseDelay;
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            EventBus.OnTimerChanged?.Invoke(delay, 0);
            yield return null;
        }
        BeginPhase(GameManager.Instance.DayManager.CurrentAct); // 대기 끝나면 Phase1 시작
    }


    private IEnumerator EndPhaseRoutine()
    {
        Debug.Log("페이즈종료코루틴 호출");
        isRunning = false;
        CurrentPhase = 0;
        timer = 0f;
        EventBus.OnTimerChanged?.Invoke(timer, CurrentPhase);

        // 플레이어 움직임 제어
        GameManager.Instance.Player.Controller.SetMovementEnabled(false);

        // 몬스터 정리
        OldMonsterProjectile.ReleaseAll();
        GameManager.Instance.spawnManager.StopSpawn();
        GameManager.Instance.spawnManager.ClearAllWarnings();
        GameManager.Instance.spawnManager.ClearAllMonsters();

        // 타이머 오프
        UIManager.Instance.SetEnableTimer(false);

        // 클리어 연출 시작
        yield return StartCoroutine(ClearWaveCorutine());

        // 나머지 청소 시작
        PoolManager.Instance.ReleaseAllObject("Drop_Exp");
        PoolManager.Instance.ReleaseAllObject("Drop_Hp");

        // EXP 정산
        GameManager.Instance.Player.Condition.CaculationEXP();

        yield return null;

        //// RewardSlot 보상 정산
        while (!GameManager.Instance.LevelUpRewardUI.IsRewardIsDone)
        {
            yield return null;
        }

        // 1초 대기
        //yield return new WaitForSeconds(1f);

        // 페이드 인
        yield return StartCoroutine(FadeIn());

        // 웨이브 클리어 Window 비활성화
        waveClearWindow.SetActive(false);

        // 플레이어 위치 이동
        GameManager.Instance.Player.Controller.Teleportation(Vector3.zero);

        CameraManager.Instance.SetCenter(0.4f, 0.66f);

        // UI 활성화
        UIManager.Instance.OnWaveDone();

        // 상점 초기화
        GameManager.Instance.ShopManager.ResetInitailize();


        EventBus.OnPhaseDone?.Invoke();

        // 페이드 아웃
        yield return StartCoroutine(FadeOut());

        // 튜토리얼 여부 진행
        if(GameManager.Instance.IsTutorial == false)
        {
            // 튜토리얼 팝업 띄우기
            GameManager.Instance.TutorialInfoButton.OnClickInfo();
            GameManager.Instance.IsTutorial = true;
            SaveManager.Instance.Save();
        }

        // 클리어 조건 확인
        GameManager.Instance.CheckClearGame();
    }

    private IEnumerator ClearWaveCorutine()
    {
        // Localization 기다리기
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("UITable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string waveClear = table.GetEntry("UI_Label_WaveClear")?.GetLocalizedString();

            // 웨이브 클리어 Window 활성화
            waveClearWindow.SetActive(true);

            // 웨이브 클리어 Txt 타이핑 연출 시작
            this.StartTyping(waveClearTxt, waveClear, typingSpeed);

            // 동시에 현재 Screen Size에 맞는 WorldSpace 공간에서 ResourceDrop 컴포넌트 붙여져 있는 오브젝트 빨아들이는 연출 추가
            yield return StartCoroutine(CollectResources());
        }
    }

    // 자석 효과 구현 (Vector3.Lerp 사용)
    private IEnumerator CollectResources()
    {
        ResourceDrop[] drops = FindObjectsOfType<ResourceDrop>();
        Camera mainCam = Camera.main;

        Vector3 uiScreenPos = goldTransform.position;
        Vector3 targetWorldPos = mainCam.ScreenToWorldPoint(uiScreenPos);

        foreach (var drop in drops)
        {
            if (drop == null) continue;

            // 카메라 시야 체크 
            Vector3 viewPos = mainCam.WorldToViewportPoint(drop.transform.position);
            bool isVisible = viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0;

            if (isVisible)
            {
                // 개별 오브젝트에게 이동 명령 전달
                drop.StartMagnet(targetWorldPos, magnetDuration, magnetCurve);
            }
        }

        yield return new WaitForSeconds(magnetDuration);
    }

    private IEnumerator FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        float t = 0f;
        Color c = fadePanel.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 1f;
        fadePanel.color = c;

    }

    public void PanelOff()
    {
        fadePanel.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = fadePanel.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
    }
    #endregion

    #region 강화 NPC 업데이트

    private void UpdateReinforcementNPC()
    {
        if (GameManager.Instance.DayManager.CurrentDay % 5 == 0)
            GameManager.Instance.ReinforcementNPC.SetReinforcementNPC();
        else
            GameManager.Instance.ReinforcementNPC.ResetReinforcementNPC();
    }

    #endregion

    #region 순찰 페이즈 설정

    public void AddEndPhase(float amount)
    {
        //endPhasetime += amount;

        // 3초 이하로는 떨어지지 않는다.
        //if (endPhasetime < 3f) endPhasetime = 3f;
    }

    #endregion

    //private void NextPhase()
    //{
    //    if (CurrentPhase < maxPhase)
    //    {
    //        BeginPhase(CurrentPhase + 1);
    //    }
    //    else if (CurrentPhase < 4)
    //    {
    //        BeginPhase(4);
    //        GameManager.Instance.spawnManager.StopSpawn();
    //        GameManager.Instance.spawnManager.ClearAllWarnings();
    //        GameManager.Instance.spawnManager.ClearAllMonsters();
    //    }
    //    else if (CurrentPhase == 4)
    //    {
    //        if (!GameManager.Instance.IsTutorial)
    //            GameManager.Instance.EndPhase();
    //    }
    //}
}
