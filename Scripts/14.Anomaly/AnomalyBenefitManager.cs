using System;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

[Serializable]
public class AnomalyBenefit
{
    [Header("이상현상 보상 이름")]
    public string name;
    [Header("이상현상 보상 타입")]
    public AnomalyBenfitType type;
    [Header("이상현상 보상 값")]
    public float value;
    [Header("이상현상 아이콘")]
    public Sprite icon;
}

// 이상현상 정답 시 나타나는 보상을 실행하는 클래스
[Serializable]
public class AnomalyBenefitManager
{
    [Header("이상현상 보상 리스트")]
    [SerializeField] private List<AnomalyBenefit> anomalyBenefits = new List<AnomalyBenefit>();

    private AnomalyBenefit currentAnomalyBenefit;

    private WeaponBase currentWeaponBase;

    /// <summary>
    /// 타입으로 Benefit 설정
    /// </summary>
    /// <returns></returns>
    public void SetAnomalyBenefitByType(AnomalyBenfitType type)
    {
        if (anomalyBenefits.Count == 0) return;

        foreach (var benefit in anomalyBenefits)
        {
            if (benefit.type == type)
            {
                currentAnomalyBenefit = benefit;
                return;
            }
        }
    }

    /// <summary>
    /// 랜덤으로 Benefit 설정
    /// </summary>
    /// <returns></returns>
    public void SetRandomAnomalyBenefit()
    {
        if (anomalyBenefits.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, anomalyBenefits.Count);
        currentAnomalyBenefit = anomalyBenefits[randomIndex];
    }

    /// <summary>
    /// Benefit 실행
    /// </summary>
    public void ExcuteBenefit()
    {
        if (currentAnomalyBenefit == null) return;

        switch(currentAnomalyBenefit.type)
        {
            case AnomalyBenfitType.BENEFIT_NONE:
                break;
            case AnomalyBenfitType.BENEFIT_GAIN_REINFORCEMENT:
                {
                    // 강화 재료 얻기
                    Debug.Log("강화 재료 얻기");
                    GameManager.Instance.Player.Condition.Add(AttributeType.Gold, currentAnomalyBenefit.value);
                }
                break;
            case AnomalyBenfitType.BENEFIT_ADD_PHASE_01:
                {
                    // 순찰 시간 증가
                    Debug.Log("순찰 시간 증가");
                    GameManager.Instance.PhaseManager.AddEndPhase(currentAnomalyBenefit.value);
                }
                break;
            case AnomalyBenfitType.BENEFIT_GAIN_ITEM:
                {
                    // 아이템 보따리 1개 획득
                    //Debug.Log("아이템 보따리 1개 획득");
                    //ISlotItem item = GameManager.Instance.ItemDataTable.GetRandomBaseItem();
                    //List<ISlotItem> items = new List<ISlotItem>();
                    //items.Add(item);
                    //EventBus.OnGainItemByAnomaly?.Invoke(items);
                }
                break;
            case AnomalyBenfitType.BENEFIT_GAIN_ITEM_EXCLUSIVE:
                {
                    // 전용 아이템 획득
                    Debug.Log("전용 아이템 획득");
                    //ISlotItem item = GameManager.Instance.ItemDataTable.GetRandomExclusiveItem();
                    //List<ISlotItem> items = new List<ISlotItem>();
                    //items.Add(item);
                    //EventBus.OnGainItemByAnomaly?.Invoke(items);
                }
                break;
            case AnomalyBenfitType.BENEFIT_ADD_ADVISOR_DAMAGE:
                {
                    Debug.Log("조언자 랜덤 한 마리 공격력 증가");

                    // 조언자 랜덤으로 뽑기
                    currentWeaponBase = GetRandomWeaponBase(GameManager.Instance.Player.PlayerSlotUI.WeaponSlots);

                    // 조언자 공격력 증가
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Add(AttributeType.Damage, currentAnomalyBenefit.value);
                    }
                }
                break;
            case AnomalyBenfitType.BENEFIT_SUB_ADVISOR_ATTACKINTERVAL:
                {
                    Debug.Log("조언자 랜덤 한 마리 공격주기 감소");

                    // 조언자 랜덤으로 뽑기
                    currentWeaponBase = GetRandomWeaponBase(GameManager.Instance.Player.PlayerSlotUI.WeaponSlots);

                    // 조언자 공격 속도 감소
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Sub(AttributeType.AttackInterval, currentAnomalyBenefit.value);
                    }
                }
                break;
            case AnomalyBenfitType.BENEFIT_ADD_ADVISOR_MOVESPEED:
                {
                    Debug.Log("조언자 랜덤 한 마리 이동속도 증가");

                    // 조언자 이동속도 증가
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Add(AttributeType.MoveSpeed, currentAnomalyBenefit.value);
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Anomaly 보상 리셋
    /// </summary>
    public void ResetAnomalyBenefit()
    {
        switch (currentAnomalyBenefit.type)
        {
            case AnomalyBenfitType.BENEFIT_NONE:
                break;
            case AnomalyBenfitType.BENEFIT_GAIN_REINFORCEMENT:
                {

                }
                break;
            case AnomalyBenfitType.BENEFIT_ADD_PHASE_01:
                {
                    // 순찰 시간 감소
                }
                break;
            case AnomalyBenfitType.BENEFIT_GAIN_ITEM:
                {

                }
                break;
            case AnomalyBenfitType.BENEFIT_GAIN_ITEM_EXCLUSIVE:
                {

                }
                break;
            case AnomalyBenfitType.BENEFIT_ADD_ADVISOR_DAMAGE:
                {
                    // 조언자 공격력 감소
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Sub(AttributeType.Damage, currentAnomalyBenefit.value);
                    }
                }
                break;
            case AnomalyBenfitType.BENEFIT_SUB_ADVISOR_ATTACKINTERVAL:
                {
                    // 조언자 공격 속도 증가
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Add(AttributeType.AttackInterval, currentAnomalyBenefit.value);
                    }
                }
                break;
            case AnomalyBenfitType.BENEFIT_ADD_ADVISOR_MOVESPEED:
                {
                    // 조언자 이동속도 감소
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Sub(AttributeType.MoveSpeed, currentAnomalyBenefit.value);
                    }
                }
                break;
            default:
                break;
        }

        currentWeaponBase = null;
    }

    private WeaponBase GetRandomWeaponBase(List<WeaponSlot> weaponSlots)
    {
        WeaponBase weaponBase = null;

        List<WeaponSlot> activeWeaponSlot = new List<WeaponSlot>();

        foreach(WeaponSlot weaponSlot in weaponSlots)
        {
            if(weaponSlot.IsActive && weaponSlot.WeaponBase != null)
            {
                activeWeaponSlot.Add(weaponSlot);
            }
        }

        // 랜덤으로 뽑기
        if(activeWeaponSlot.Count != 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, activeWeaponSlot.Count);
            weaponBase = activeWeaponSlot[randomIndex].WeaponBase;
        }

        return weaponBase;
    }

    #region 프로퍼티

    public AnomalyBenefit AnomalyBenefit { get { return currentAnomalyBenefit; } set { currentAnomalyBenefit = value; } }

    #endregion
}
