using Cinemachine;
using NPOI.SS.Formula.Functions;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NextTrigger : MonoBehaviour, IInteractable
{
    [Header("자식 트리거 콜라이더")]
    [SerializeField] private Collider2D houseTrigger;
    [SerializeField] private Collider2D altarTrigger;
    [SerializeField] private GameObject altarChild;
    [SerializeField] private Collider2D baseToBattleTrigger;
    [SerializeField] private GameObject baseToBattleChild;
    [SerializeField] private Collider2D battleToBaseTrigger;
    [SerializeField] private GameObject battleToBaseChild;
    [SerializeField] private Collider2D infoBoardTrigger;
    [SerializeField] private Collider2D spawnPoint;
    [Header("InputImage")]
    [SerializeField] private GameObject sprite;
    [Header("InfoBoard UI")]
    [SerializeField] private GameObject infoBoardUI;


    [Header("Virtual Camera")]
    [SerializeField] private CinemachineVirtualCamera baseCamera;
    [SerializeField] private CinemachineVirtualCamera combatCamera;

    [Header("DayUi")]
    [SerializeField] private DayTransitionUI dayTransitionUI;

    private bool altarChecked = false;
    private bool inAltar;
    private bool inHouse;
    private bool inBaseToBattle;
    private bool inBattleToBase;
    private bool inInfoBoard;
    private bool battleDone = false;
    private bool battleStart = false;
    private float fadeDuration = 0.5f;
    bool isCorrect;

    private UnityEngine.UI.Image[] boardImages;
    private TMPro.TMP_Text[] boardTexts;



    private void Start()
    {
        boardImages = infoBoardUI.GetComponentsInChildren<UnityEngine.UI.Image>(true);
        boardTexts = infoBoardUI.GetComponentsInChildren<TMPro.TMP_Text>(true);
        baseToBattleChild.SetActive(true);
        battleToBaseChild.SetActive(false);
        altarChild.SetActive(false);
        SetAlpha(0f); // 시작은 투명
        infoBoardUI.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.OnPhaseDone += OnPhaseFinished;
        EventBus.OnPhaseStart += HandlePhaseStart;

    }

    private void OnDisable()
    {
        EventBus.OnPhaseDone -= OnPhaseFinished;
        EventBus.OnPhaseStart -= HandlePhaseStart;
    }

    #region 플레이어 상호작용
    public void Interact(Player player)
    {
        if (inBaseToBattle || inBattleToBase)
        {
            if (GameManager.Instance.IsTutorial) // 튜토리얼 제한 조건
            {
                if (inBaseToBattle && GameManager.Instance.TutorialInfo)
                {
                    StartCoroutine(FlashRed());
                    return;
                }
                if (inBattleToBase && GameManager.Instance.TutorialAnomaly)
                {
                    StartCoroutine(FlashRed());
                    return;
                }
            }
            StartCoroutine(PressFeedback());
            TeleportCheck();
            return;
        }
        if (!battleDone && (inHouse || inAltar))
        {
            StartCoroutine(FlashRed());
            return;
        }
        if (battleDone && GameManager.Instance.IsTutorial)
        {
            if (inHouse)
            {
                if (GameManager.Instance.TutorialHouse)
                {
                    StartCoroutine(FlashRed());
                    return;
                }
            }

            if (inAltar)
            {
                if (GameManager.Instance.TutorialAltar)
                {
                    StartCoroutine(FlashRed());
                    return;
                }
            }

        }
        StartCoroutine(PressFeedback());
        AnswerCheck();
    }

    public void OpenInteractUI(Player player)
    {
        Collider2D interactColider = CheckCollider(player);

        inHouse = inAltar = inBaseToBattle = inBattleToBase = inInfoBoard = false;


        if (interactColider != null)
        {
            if (interactColider == houseTrigger)
            {
                inHouse = true;
                SetSpritePosition(houseTrigger);
            }
            else if (interactColider == altarTrigger)
            {
                inAltar = true;
                SetSpritePosition(altarTrigger);
            }
            else if (interactColider == baseToBattleTrigger)
            {
                inBaseToBattle = true;
                SetSpritePosition(baseToBattleTrigger);
            }
            else if (interactColider == battleToBaseTrigger)
            {
                inBattleToBase = true;
                SetSpritePosition(battleToBaseTrigger);
            }
            else if (interactColider == infoBoardTrigger)
            {
                Debug.Log("들어옴");
                inInfoBoard = true;
                StartCoroutine(FadeInInfoBoard());
            }
            if (inHouse || inAltar || inBaseToBattle || inBattleToBase)
                sprite.SetActive(true);
        }
        else
        {
            CloseInteractUI(player);
        }
    }

    private void SetSpritePosition(Collider2D col)
    {
        Vector3 pos = col.bounds.center;

        if (col == houseTrigger) pos = new Vector3(col.bounds.center.x, col.bounds.min.y);
        else if (col == altarTrigger) pos = new Vector3(col.bounds.center.x, col.bounds.min.y);
        else if (col == baseToBattleTrigger) pos = new Vector3(col.bounds.center.x, col.bounds.min.y - 1f);
        else if (col == battleToBaseTrigger) pos = new Vector3(col.bounds.center.x, col.bounds.min.y - 1f);

        sprite.transform.position = pos;
    }

    private Collider2D CheckCollider(Player player)
    {
        if (player.PlayerInteractHandler.InteractCollider == houseTrigger) return houseTrigger;
        else if (player.PlayerInteractHandler.InteractCollider == altarTrigger) return altarTrigger;
        else if (player.PlayerInteractHandler.InteractCollider == baseToBattleTrigger) return baseToBattleTrigger;
        else if (player.PlayerInteractHandler.InteractCollider == battleToBaseTrigger) return battleToBaseTrigger;
        else if (player.PlayerInteractHandler.InteractCollider == infoBoardTrigger) return infoBoardTrigger;
        else return null;
    }

    public void CloseInteractUI(Player player)
    {
        sprite.SetActive(false);
        if (inInfoBoard)
            StartCoroutine(FadeOutInfoBoard());
    }

    public int GetPriority()
    {
        return Define.INTERACT_PRIORITY_02;
    }
    #endregion

    #region 텔레포트체크
    private void TeleportCheck()
    {
        if (inBaseToBattle && !battleDone && !battleStart)
        {
            battleStart= true;
            baseToBattleChild.SetActive(false);
            altarChild.SetActive(true);
            GameManager.Instance.StartPhase();

            Teleport(battleToBaseTrigger, combatCamera);
            GameManager.Instance.Player.Controller.IsMoveToBase(false);

            DropItemPool.Instance.ReleaseAllDropItem();
            InfoUIPoolManager.Instance.ReleaseAll();
            GameManager.Instance.DayManager.UpdateGauge();

            GameManager.Instance.AnomalyManager.CheckAnomalyByDay(); //이상현상 초기화 및 실행
            Debug.Log("전투구간으로");

            UIManager.Instance.SetEnableTimer(true);
        }
        else if (inBattleToBase)// && GameManager.Instance.PhaseManager.isTeleport && battleStart)// && GameManager.Instance.DayManager.IsGauagueFull())
        {
            battleStart = false;
            battleDone = true;
            GameManager.Instance.EndPhase();

            Teleport(baseToBattleTrigger, baseCamera);
            GameManager.Instance.Player.Controller.IsMoveToBase(true);

            // 페이즈가 끝날 시 드롭 아이템 삭제 후 페이즈가 끝났음을 전달
            DropItemPool.Instance.ReleaseAllDropItem();

            UIManager.Instance.SetEnableTimer(false);
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }
    private void OnPhaseFinished()//강제종료시
    {
        if (GameManager.Instance.IsTutorial) return;

        Teleport(baseToBattleTrigger, baseCamera);
        GameManager.Instance.Player.Controller.IsMoveToBase(true);

        battleDone = true;
        battleStart = false;

        DropItemPool.Instance.ReleaseAllDropItem();

        UIManager.Instance.SetEnableTimer(false);
    }
    private void HandlePhaseStart(int phaseIndex)
    {
        if (phaseIndex == 4)
        {
            battleToBaseChild.SetActive(true);
        }
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    private void Teleport(Collider2D col, CinemachineVirtualCamera targetCamera = null)
    {
        Vector3 targetPos = col.transform.position;
        GameManager.Instance.Player.Controller.Teleportation(targetPos);
        GameManager.Instance.Player.GetComponentInChildren<SpriteRenderer>().flipX = true;

        if (!targetCamera)
            return;

        CameraManager.Instance.SwitchCamera(targetCamera);

        Debug.Log("이동");
    }
    #endregion

    #region 정답체크
    private void AnswerCheck()
    {
        bool anomaly = GameManager.Instance.AnomalyManager.isAnomaly;

        if (inAltar)
        {
            if (!altarChecked)
            {
                ApplyAltarEffect(anomaly);
                altarChecked = true;
            }
            else
            {
                StartCoroutine(FlashRed());
                Debug.Log("제단은 이미 확인했습니다.");
            }
            return;
        }


        if (inHouse)
        {
            if (!anomaly)
            {
                // 이상현상 없음인데 제단을 누른 경우 → 오답
                isCorrect = !altarChecked;
            }
            else
            {
                // 이상현상 있음 → 제단 확인해야 정답
                isCorrect = altarChecked;
            }

            ApplyHouseResult(isCorrect);
            NextDay();
            altarChecked = false;
            battleDone = false;
        }
    }
    #endregion

    #region 효과/판단 처리
    private void ApplyAltarEffect(bool anomaly)
    {
        if (anomaly)
        {
            Debug.Log("이상현상 있을때 효과");
            //정화 효과
        }
        else
        {
            Debug.Log("이상현상 없을때 효과");
            //무효 효과
        }
    }

    private void ApplyHouseResult(bool isCorrect)
    {
        if (isCorrect)
        {
            if (GameManager.Instance.IsTutorial)
                return;
            Debug.Log("정답!");
            //메리트 효과 / 아이템 떨구기
            AnomalyResultManager.Instance.SetAnomalyBenefit();
        }
        else
        {
            Debug.Log("오답!");
            //디메리트 효과 / 몬스터 스탯 증가
            AnomalyResultManager.Instance.SetAnomalyPenalty();
        }
        
    }
    #endregion

    #region 다음날처리
    private void NextDay()
    {
        StartCoroutine(NextDayRoutine());
    }
    #endregion

    #region 코루틴
    private IEnumerator NextDayRoutine()
    {
        
        GameManager.Instance.Player.Controller.SetMovementEnabled(false);

        yield return StartCoroutine(dayTransitionUI.FadeInPanel());

        GameManager.Instance.DayManager.NextDay();
        Teleport(spawnPoint);
        AnomalyResultManager.Instance.ApplyAnomalyResult();
        baseToBattleChild.SetActive(true);
        battleToBaseChild.SetActive(false);
        altarChild.SetActive(false);

        yield return StartCoroutine(dayTransitionUI.FadeInText(isCorrect));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(dayTransitionUI.TextAndFadeOut());

        // 효과 적용 // 
        

        GameManager.Instance.Player.Controller.SetMovementEnabled(true);
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer sr = sprite.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        Vector3 originalPos = sprite.transform.localPosition;

        sr.color = new Color(1f, 0f, 0f, 0.5f);

        float duration = 0.3f;
        float magnitude = 0.05f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float offsetX = Random.Range(-magnitude, magnitude);
            float offsetY = Random.Range(-magnitude, magnitude);

            sprite.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }

        // 원래 상태 복귀
        sr.color = originalColor;
        sprite.transform.localPosition = originalPos;

    }
    private IEnumerator PressFeedback()
    {
        // SpriteRenderer 대신 Transform을 이용해 크기 변화
        Vector3 originalScale = sprite.transform.localScale;
        Vector3 pressedScale = originalScale * 0.9f; // 살짝 줄이기

        float duration = 0.1f; // 눌림 속도
        float time = 0f;

        // Scale 줄이기
        while (time < duration)
        {
            time += Time.deltaTime;
            sprite.transform.localScale = Vector3.Lerp(originalScale, pressedScale, time / duration);
            yield return null;
        }

        // 잠깐 눌린 상태 유지
        yield return new WaitForSeconds(0.05f);

        // Scale 원래대로 복귀
        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            sprite.transform.localScale = Vector3.Lerp(pressedScale, originalScale, time / duration);
            yield return null;
        }

        sprite.transform.localScale = originalScale;
    }


    private IEnumerator FadeInInfoBoard()
    {
        infoBoardUI.SetActive(true);
        Debug.Log("들어옴");
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 0.8f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
    }

    private IEnumerator FadeOutInfoBoard()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0.8f, 0f, t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
        infoBoardUI.SetActive(false);
    }



    #endregion

    private void SetAlpha(float alpha)
    {
        foreach (var img in boardImages)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
        foreach (var txt in boardTexts)
        {
            Color c = txt.color;
            c.a = alpha;
            txt.color = c;
        }
    }

}