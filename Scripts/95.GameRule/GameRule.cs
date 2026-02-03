using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class GameRule : MonoBehaviour
{
    [Header("행운 1당 이동 비율 TIER")]
    [SerializeField] private float[] movePerLuck = { 1.0f, 0.7f, 0.3f };

    [SerializeField] private Dictionary<PhaseType, GameRulePhaseData> gameRulePhaseDict;
    [SerializeField] private Dictionary<DropType, GameRuleDropRate> gameRuleDropDict;
    [SerializeField] private Dictionary<WeaponType, GameRuleExpWeaponData> gameRuleExpWeaponDict;
    [SerializeField] private List<GameRuleDayData> gameRuleDayDatas;
    [SerializeField] private List<GameRuleItemLvData> gameRuleItemLvDatas;
    [SerializeField] private List<GameRuleWeaponLvData> gameRuleWeaponLvDatas;
    [SerializeField] private List<GameRuleWaveData> gameRuleWaveDatas;

    private static GameRule instance;

    public static GameRule Instance
    {
        get
        {
            if (null == instance)
            {
                instance = FindObjectOfType<GameRule>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private GameRule() { }


    private void Start()
    {
        // GameRule 적용
        InitData();

        GameManager.Instance.GameRule = this;
    }

    private void InitData()
    {
        gameRulePhaseDict = DataManager.GetGameRulePhaseDict();
        gameRuleDropDict = DataManager.GetGameRuleDropDict();
        gameRuleDayDatas = DataManager.GetList<GameRuleDayData>();
        gameRuleItemLvDatas = DataManager.GetList<GameRuleItemLvData>();
        gameRuleWeaponLvDatas = DataManager.GetList<GameRuleWeaponLvData>();
        gameRuleExpWeaponDict = DataManager.GameRuleExpWeaponDataDict;
        gameRuleWaveDatas = DataManager.GameRuleWaveDataList;
    }

    public GameRulePhaseData GetPhaseRule(int phase)
    {
        GameRulePhaseData rule = null;

        switch (phase)
        {
            case 0:
            case 1:
                rule = gameRulePhaseDict[PhaseType.Phase_01];
                break;
            case 2:
                rule = gameRulePhaseDict[PhaseType.Phase_02];
                break;
            case 3:
                rule = gameRulePhaseDict[PhaseType.Phase_03];
                break;
            case 4: // 마지막 페이즈 앞전 페이즈 데이터 가져오기
                if (GameManager.Instance.DayManager.CurrentAct == 1) rule = gameRulePhaseDict[PhaseType.Phase_01];
                else if (GameManager.Instance.DayManager.CurrentAct == 2) rule = gameRulePhaseDict[PhaseType.Phase_02];
                else if (GameManager.Instance.DayManager.CurrentAct == 3) rule = gameRulePhaseDict[PhaseType.Phase_03];
                break;

            default:
                break;
        }

        return rule;
    }

    public GameRulePhaseData GetPhaseRule(PhaseType type)
    {
        if (gameRulePhaseDict.ContainsKey(type))
        {
            return gameRulePhaseDict[type];
        }

        return null;
    }

    /// <summary>
    /// 드롭 확률 구하기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameRuleDropRate GetDropRate(DropType type)
    {
        if (gameRuleDropDict == null) Debug.Log("없는데요");

        if(gameRuleDropDict.ContainsKey(type))
        {
            return gameRuleDropDict[type];
        }

        return null;
    }

    /// <summary>
    /// Day에 따라 요구 게이트 경험치 반환
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public float GetRequireGateEnergyByDay(int day)
    {
        float requiredEnergy = 0;

        foreach(var ruleDayData in gameRuleDayDatas)
        {
            if(ruleDayData.DAY_MIN <= day && ruleDayData.DAY_MAX >= day)
            {
                requiredEnergy = ruleDayData.REQUIRED_GATE_ENERGY;
            }
        }

        return requiredEnergy;
    }

    /// <summary>
    /// Day에 따라 몬스터 체력 배율 반환
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public float GetMonsterHpMultiplierByDay(int day)
    {
        float multiplier = 0;

        foreach (var ruleDayData in gameRuleDayDatas)
        {
            if (ruleDayData.DAY_MIN <= day && ruleDayData.DAY_MAX >= day)
            {
                multiplier = ruleDayData.MONSTER_HP_MULTIPLIER;
            }
        }

        return multiplier;
    }

    public Tier GetTierByDay(int day)
    {
        // 현재 날짜가 언제인지 확인
        foreach (var ruleDayData in gameRuleDayDatas)
        {
            if (ruleDayData.DAY_MIN <= day && ruleDayData.DAY_MAX >= day)
            {
                Debug.Log($"현재 Wave: {day}, [티어확률 COMMAN: {ruleDayData.RATE_TIER1}, RARE: {ruleDayData.RATE_TIER2}, EPIC: {ruleDayData.RATE_TIER3}, LEGEND: {ruleDayData.RATE_TIER4}");
                float[] baseRates = { ruleDayData.RATE_TIER1, ruleDayData.RATE_TIER2, ruleDayData.RATE_TIER3, ruleDayData.RATE_TIER4 };
                float luck = GameManager.Instance.Player.Stat.Luck.Value;
                float[] bonuseRates = ApplyLuckBonus(baseRates, luck);
                Debug.Log($"현재 Wave: {day}, [행운 보정 티어확률 COMMAN: {bonuseRates[0]}, RARE: {bonuseRates[1]}, EPIC: {bonuseRates[2]}, LEGEND: {bonuseRates[3]}");
                
                return GetTierByRate(ApplyLuckBonus(baseRates, luck));
            }
        }

        return Tier.COMMAN;
    }

    /// <summary>
    /// 각 티어별 확률에 따른 티어 구하기
    /// </summary>
    /// <param name="tier1"></param>
    /// <param name="tier2"></param>
    /// <param name="tier3"></param>
    /// <param name="tier4"></param>
    /// <returns></returns>
    private Tier GetTierByRate(float[] rates)
    {
        float rand = UnityEngine.Random.Range(0, 100f);

        float cumulative = 0f;

        for (int i = 0; i < rates.Length; i++)
        {
            cumulative += rates[i];

            if (rand < cumulative)
                return (Tier)i;
        }

        return (Tier)(rates.Length - 1);
    }

    /// <summary>
    /// Day에 따라 TIER 값 구하기
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public EnhancementTier GetEnhancementTypeByDay(int day)
    {
        // 현재 날짜가 언제인지 확인
        foreach (var ruleDayData in gameRuleDayDatas)
        {
            if (ruleDayData.DAY_MIN <= day && ruleDayData.DAY_MAX >= day)
            {
                float[] baseRates = { ruleDayData.RATE_TIER1, ruleDayData.RATE_TIER2, ruleDayData.RATE_TIER3, ruleDayData.RATE_TIER4 };
                float luck = GameManager.Instance.Player.Stat.Luck.Value;
                return GetEnhancementTypeByRate(ApplyLuckBonus(baseRates, luck));
            }
        }

        return EnhancementTier.TIER1;
    }

    /// <summary>
    /// 각 티어별 확률에 따른 티어 구하기
    /// </summary>
    /// <param name="tier1"></param>
    /// <param name="tier2"></param>
    /// <param name="tier3"></param>
    /// <param name="tier4"></param>
    /// <returns></returns>
    private EnhancementTier GetEnhancementTypeByRate(float[] rates)
    {
        float rand = UnityEngine.Random.Range(0, 100f);

        float cumulative = 0f;

        for (int i = 0; i < rates.Length; i++)
        {
            cumulative += rates[i];

            if (rand < cumulative)
                return (EnhancementTier)i;
        }

        return (EnhancementTier)(rates.Length - 1);
    }

    private float[] ApplyLuckBonus(float[] baseRates, float luck)
    { 
        // 행운 1당 이동 비율
        float[] movePerLuck = { 1.0f, 0.7f, 0.3f };

        // 복사본
        float[] rates = (float[])baseRates.Clone();

        for (int i = 0; i < movePerLuck.Length; i++)
        {
            float moveAmount = luck * movePerLuck[i];

            moveAmount = Mathf.Min(moveAmount, rates[i]);

            rates[i] -= moveAmount;
            rates[i + 1] += moveAmount;
        }

        return rates;
    }

    /// <summary>
    /// Day에 따라 WEAPON/ITEM 확률 값 구하기
    /// </summary>
    /// <param name="day"></param>
    /// <returns></returns>
    public float GetWeaponOrItemRate(DropItemType type, int day)
    {
        // 현재 날짜가 언제인지 확인
        foreach (var ruleDayData in gameRuleDayDatas)
        {
            if (ruleDayData.DAY_MIN <= day && ruleDayData.DAY_MAX >= day)
            {
                if (type == DropItemType.Weapon) return ruleDayData.RATE_WEAPON;
                else
                {
                    return ruleDayData.RATE_ITEM;
                }
            }
        }

        return 0f;
    }

    /// <summary>
    /// Day에 따라 WEAPON/ITEM 레벨 구하기
    /// </summary>
    /// <param name="type"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    public float GetWeaponOrItemLv(DropItemType type, int day)
    {
        // 현재 날짜가 언제인지 확인
        foreach (var ruleDayData in gameRuleDayDatas)
        {
            if (ruleDayData.DAY_MIN <= day && ruleDayData.DAY_MAX >= day)
            {
                if (type == DropItemType.Weapon) return ruleDayData.WEAPON_AVERAGE_LEVEL;
                else
                {
                    return ruleDayData.ITEM_AVERAGE_LEVEL;
                }
            }
        }

        return 0f;
    }

    /// <summary>
    /// WeaponType에 따라 Lv별 증가(고정)되는 값 가져오기
    /// </summary>
    /// <returns></returns>
    public float GetWeaponLvIncreaseDamageStatByType(WeaponType type, float lv)
    {
        if (gameRuleExpWeaponDict.TryGetValue(type, out GameRuleExpWeaponData data))
        {
            // lv이 해당 범위 내 들어오면 반환
            if (data.LV_RANGE_1 == lv)
            {
                return (data.DAMAGE_FIXED_1);
            }
            else if(data.LV_RANGE_2 == lv)
            {
                return (data.DAMAGE_FIXED_2);
            }
            else if (data.LV_RANGE_3 == lv)
            {
                return (data.DAMAGE_FIXED_3);
            }
            else if (data.LV_RANGE_4 == lv)
            {
                return (data.DAMAGE_FIXED_4);
            }
            else
            {
                return (data.DAMAGE_FIXED_1);
            }
        }

        // 아니면 최솟값 반환
        return (data.DAMAGE_FIXED_1);
    }

    public float GetWeaponLvIncreaseAttackIntervalStatByType(WeaponType type, float lv)
    {
        if (gameRuleExpWeaponDict.TryGetValue(type, out GameRuleExpWeaponData data))
        {
            // lv이 해당 범위 내 들어오면 반환
            if (data.LV_RANGE_1 == lv)
            {
                return (data.ATTACK_INTERVAL_FIXED_1);
            }
            else if (data.LV_RANGE_2 == lv)
            {
                return (data.ATTACK_INTERVAL_FIXED_2);
            }
            else if (data.LV_RANGE_3 == lv)
            {
                return (data.ATTACK_INTERVAL_FIXED_3);
            }
            else if (data.LV_RANGE_4 == lv)
            {
                return (data.ATTACK_INTERVAL_FIXED_4);
            }
            else
            {
                return (data.ATTACK_INTERVAL_FIXED_1);
            }
        }

        // 아니면 최솟값 반환
        return (data.ATTACK_INTERVAL_FIXED_1);
    }

    /// <summary>
    /// 아이템 레벨 별 분해 비용 반환
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public float GetItemDisassemblyCostByLv(Attribute lv)
    {
        // Lv별 분해 비용 반환
        foreach (var ruleItemData in gameRuleItemLvDatas)
        {
            if(ruleItemData.LV == lv.Value)
            {
                return ruleItemData.DISASSEMBLY_COST;
            }
        }

        // 마지막 레벨보다 높으면 최댓값 반환
        if (gameRuleItemLvDatas[gameRuleItemLvDatas.Count - 1].LV > lv.Value) 
            return gameRuleItemLvDatas[gameRuleItemLvDatas.Count - 1].DISASSEMBLY_COST;

        // 아니면 -1 반환
        return -1;
    }

    /// <summary>
    /// 아이템 레벨 별 강화 비용 반환
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public float GetItemReinforcementCost(Attribute lv)
    {
        // Lv별 강화 비용 반환
        foreach (var ruleItemData in gameRuleItemLvDatas)
        {
            if (ruleItemData.LV == lv.Value)
            {
                return ruleItemData.REINFORCEMENT_COST;
            }
        }

        // 마지막 레벨보다 높으면 최댓값 반환
        if (gameRuleItemLvDatas[gameRuleItemLvDatas.Count - 1].LV > lv.Value)
            return gameRuleItemLvDatas[gameRuleItemLvDatas.Count - 1].REINFORCEMENT_COST;

        // 아니면 -1 반환
        return -1;
    }

    /// <summary>
    /// 무기 레벨 별 분해 비용 반환
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public float GetWeaponDisassemblyCostByLv(Attribute lv)
    {
        // Lv별 분해 비용 반환
        foreach (var ruleItemData in gameRuleWeaponLvDatas)
        {
            if (ruleItemData.LV == lv.Value)
            {
                return ruleItemData.DISASSEMBLY_COST;
            }
        }

        // 마지막 레벨보다 높으면 최댓값 반환
        if (gameRuleWeaponLvDatas[gameRuleWeaponLvDatas.Count - 1].LV > lv.Value)
            return gameRuleWeaponLvDatas[gameRuleWeaponLvDatas.Count - 1].DISASSEMBLY_COST;

        // 아니면 -1 반환
        return -1;
    }

    /// <summary>
    /// 무기 레벨 별 강화 비용 반환
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public float GetWeaponReinforcementCostByLv(Attribute lv)
    {
        // Lv별 분해 비용 반환
        foreach (var ruleItemData in gameRuleWeaponLvDatas)
        {
            if (ruleItemData.LV == lv.Value)
            {
                return ruleItemData.REINFORCEMENT_COST;
            }
        }

        // 마지막 레벨보다 높으면 최댓값 반환
        if (gameRuleWeaponLvDatas[gameRuleWeaponLvDatas.Count - 1].LV > lv.Value)
            return gameRuleWeaponLvDatas[gameRuleWeaponLvDatas.Count - 1].REINFORCEMENT_COST;

        // 아니면 -1 반환
        return -1;
    }

    /// <summary>
    /// 현재 웨이브에 맞는 리롤 가격 구하기
    /// </summary>
    /// <returns></returns>
    public float GetRerollPriceByWave(int wave)
    {
        foreach(var ruleWaveData in gameRuleWaveDatas)
        {
            if (ruleWaveData.WAVE == wave)
            {
                return ruleWaveData.REROLL_VALUE;
            }
        }

        return 1;
    }

    /// <summary>
    /// 현재 웨이브에 맞는 리롤 증가량 구하기
    /// </summary>
    /// <returns></returns>
    public float GetRerollIncreasePriceByWave(int wave)
    {
        foreach (var ruleWaveData in gameRuleWaveDatas)
        {
            if (ruleWaveData.WAVE == wave)
            {
                return ruleWaveData.REROLL_MULTIPLIER;
            }
        }

        return 1;
    }

    #region 프로퍼티

    public Dictionary<PhaseType, GameRulePhaseData> PhaseRuleDict { get { return gameRulePhaseDict; } }
    public Dictionary<DropType, GameRuleDropRate> DropRuleDict { get { return gameRuleDropDict; } }
    public List<GameRuleDayData > DayRule { get { return gameRuleDayDatas; } }

    #endregion
}
