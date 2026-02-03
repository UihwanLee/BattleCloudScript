using Org.BouncyCastle.Math.EC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class LevelUpRewardUI : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private GameObject window;
    [SerializeField] private List<LevelUpRewardSlot> levelUpRewardSlots = new List<LevelUpRewardSlot>();

    [Header("애니메이션 연출 데이터")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve openCurve;
    [SerializeField] private AnimationCurve closeCurve;

    [Header("슬롯 티어별 UI")]
    [SerializeField] private Sprite tier1Sprite;
    [SerializeField] private Sprite tier2Sprite;
    [SerializeField] private Sprite tier3Sprite;
    [SerializeField] private Sprite tier4Sprite;


    private Coroutine scaleChangeCoroutine;

    // LevelUpReward에 사용할 데이터
    private Dictionary<EnhancementTier, List<LevelUpRewardEnhancementData>> levelUpRewardEnhancementDict;
    private List<LevelUpRewardRuleData> levelUpRewardRuleDataList;

    private Player player;

    private bool isCanSlotClick = true;

    private int rewardCount = 0;            // RewardCount

    private bool isRewardIsDone = true;


    private void Reset()
    {
        window = GameObject.Find("LevelUpRewardWindow");
        levelUpRewardSlots = transform.GetComponentsInChildren<LevelUpRewardSlot>().ToList();

        // 등장 커브 설정
        openCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.7f, 1.15f), new Keyframe(1f, 1f));
        // 퇴장 커브 설정
        closeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    }

    private void Start()
    {
        levelUpRewardEnhancementDict = DataManager.GetDict<LevelUpRewardEnhancementData>();
        levelUpRewardRuleDataList = DataManager.GetList<LevelUpRewardRuleData>();

        GameManager.Instance.LevelUpRewardUI = this;
        player = GameManager.Instance.Player;

        InitSlot();
    }


    private void InitSlot()
    {
        foreach(var slot in levelUpRewardSlots)
        {
            slot.rewardSelectBtn.onClick.RemoveAllListeners();
            slot.rewardSelectBtn.onClick.AddListener(() => OnClickRewardBtn(slot));
        }
    }

    public void OnClickRewardBtn(LevelUpRewardSlot slot)
    {
        // 슬롯을 클릭할 수 있는 상태인지 체크
        if (!isCanSlotClick) return;

        // 중복 클릭 못하도록 변경
        isCanSlotClick = false;

        // 타켓 지정
        ItemApplyTarget target = DataManager.GetItemApplyTarget(slot.Data.APPLYTARGET);

        // 보따리인지 확인
        if(slot.Items.Count == 0)
        {
            // 효과 적용
            player.PlayerEffectHandler.ApplyItemByPlayer(slot.Type, slot.Data.VALUE, true);
        }
        else
        {
            foreach(var item in slot.Items)
            {
                // UI 컨테이너에 적용
                EventBus.OnGainItem?.Invoke(item);
            }
        }

        rewardCount--;

        if(rewardCount <= 0)
        {
            // 다 끝남을 알림
            StartScaleSlotRoutine(false);
            rewardCount = 0;
        }
        else
        {
            // 보상 한번 더 남으면 다시 소환
            SetLevelUpSlot();
            isCanSlotClick = true;
        }
    }

    #region Reward 슬롯 설정 로직

    public void OpenLevelUpReward(int count)
    {
        if (count <= 0)
        {
            GameManager.Instance.PhaseManager.PanelOff();
            return;
        }

        isRewardIsDone = false;

        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiPopUp, WhereAreYouLookinAt.Enum.SFXPlayType.Single);

        // RewardData 참조하여 가져옴
        window.SetActive(true);

        // 보상 몇 번 받을 지 설정
        rewardCount = count;

        SetLevelUpSlot();

        StartScaleSlotRoutine(true);
    }

    private void SetLevelUpSlot()
    {
        // DayManager의 Day 가져오기
        int day = GameManager.Instance.DayManager.CurrentDay;

        PlayerSlotUI slotUI = player.PlayerSlotUI;

        // 데이터 초기화
        levelUpRewardSlots[0].Set(this, GetRandomEnhancementDataByDay(day));
        levelUpRewardSlots[1].Set(this, GetRandomEnhancementDataByDay(day));
        levelUpRewardSlots[2].Set(this, GetRandomEnhancementDataByDay(day));
    }

    private LevelUpRewardEnhancementData GetRandomEnhancementDataByDay(int day)
    {

        // 현재 일 수에 따라 Tier 뽑기
        EnhancementTier tier = GameManager.Instance.GameRule.GetEnhancementTypeByDay(day);

        // Tire에 속해 있는 Data 뽑기
        int randIndex = Random.Range(0, levelUpRewardEnhancementDict[tier].Count);

        return levelUpRewardEnhancementDict[tier][randIndex];
    }

    #endregion

    #region 애니메이션 연출

    private void StartScaleSlotRoutine(bool isScaleUp)
    {
        if (scaleChangeCoroutine != null) StopCoroutine(scaleChangeCoroutine);
        scaleChangeCoroutine = StartCoroutine(ScaleSlotRoutine( isScaleUp));
    }

    private IEnumerator ScaleSlotRoutine(bool isScaleUp)
    {
        if (isScaleUp)
        {
            // 모두 커짐
            window.GetComponent<RectTransform>().localScale = Vector3.zero;

            // Localization이 끝날 때가지 기다리기
            bool isDone = true;
            do
            {
                foreach (var slot in levelUpRewardSlots)
                {
                    if (slot.IsLocalizationDone == false) isDone = false;
                }

                yield return null;
            }
            while (isDone);

            StartCoroutine(ScaleChange(window.GetComponent<RectTransform>(), true));

            // 모든 카드가 켜질 때까지 대기
            yield return new WaitForSeconds(animationDuration);
        }
        else
        {
            // 모두 작아짐
            StartCoroutine(ScaleChange(window.GetComponent<RectTransform>(), false));

            // 모든 카드가 사라질 때까지 대기
            yield return new WaitForSeconds(animationDuration);

            // 필요 시 패널 비활성화 등 추가
            window.SetActive(false);

            // 타임 슬로우 리셋
            //Time.timeScale = 1f;

            GameManager.Instance.PhaseManager.PanelOff();

            isRewardIsDone = true;
        }

        // 클릭 가능하게 세팅
        isCanSlotClick = true;
    }

    private IEnumerator ScaleChange(RectTransform target, bool isScaleUp)
    {
        float time = 0;

        Vector3 startScale = isScaleUp ? Vector3.zero : Vector3.one;
        Vector3 endScale = isScaleUp ? Vector3.one : Vector3.zero;

        // 커브 지정
        AnimationCurve currentCurve = isScaleUp ? openCurve : closeCurve;

        while (time < animationDuration)
        {
            // unscaledDeltaTime 사용
            time += Time.unscaledDeltaTime;
            float t = time / animationDuration;

            float curveValue = (currentCurve != null && currentCurve.length > 0)
                               ? currentCurve.Evaluate(t)
                               : t;

            if (isScaleUp)
                target.localScale = Vector3.one * curveValue;
            else
                target.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        target.localScale = endScale;
    }

    #endregion

    #region 프로퍼티

    public Sprite Tier1Sprite { get { return tier1Sprite; } }
    public Sprite Tier2Sprite { get { return tier2Sprite; } }
    public Sprite Tier3Sprite { get { return tier3Sprite; } }
    public Sprite Tier4Sprite { get { return tier4Sprite; } }

    public bool IsRewardIsDone { get { return isRewardIsDone; }}

    #endregion
}
