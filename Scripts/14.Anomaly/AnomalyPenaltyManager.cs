using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

[Serializable]
public class AnomalyPenalty
{
    [Header("이상현상 패널티 이름")]
    public string name;
    [Header("이상현상 패널티 타입")]
    public AnomalyPenaltyType type;
    [Header("이상현상 패널티 값")]
    public float value;
    [Header("이상현상 아이콘")]
    public Sprite icon;
}

// 이상현상 정답 시 나타나는 패널티를 실행하는 클래스
[Serializable]
public class AnomalyPenaltyManager
{
    [Header("이상현상 보상 리스트")]
    [SerializeField] private List<AnomalyPenalty> anomalyPenalties = new List<AnomalyPenalty>();

    private AnomalyPenalty currentAnomalyPenalty;

    private WeaponBase currentWeaponBase;

    /// <summary>
    /// 타입으로 Penalty 설정
    /// </summary>
    /// <returns></returns>
    public void SetAnomalyPenaltyByType(AnomalyPenaltyType type)
    {
        if (anomalyPenalties.Count == 0) return;

        foreach(var penalty in anomalyPenalties)
        {
            if(penalty.type == type)
            {
                currentAnomalyPenalty = penalty;
                return;
            }
        }
    }

    /// <summary>
    /// 랜덤으로 Penalty 설정
    /// </summary>
    /// <returns></returns>
    public void SetRandomAnomalyPenalty()
    {
        if (anomalyPenalties.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, anomalyPenalties.Count);
        currentAnomalyPenalty = anomalyPenalties[randomIndex];
    }

    /// <summary>
    /// Penalty 실행
    /// </summary>
    public void ExcutePenalty()
    {
        if (currentAnomalyPenalty == null) return;

        switch (currentAnomalyPenalty.type)
        {
            case AnomalyPenaltyType.PENALTY_NONE:
                break;
            case AnomalyPenaltyType.PENALTY_SUB_PHASE_01:
                {
                    // 순찰 시간 감소
                    Debug.Log("순찰 시간 감소");
                    GameManager.Instance.PhaseManager.AddEndPhase(currentAnomalyPenalty.value);
                }
                break;
            case AnomalyPenaltyType.PENALTY_SUB_EXP_RANGE:
                {
                    // 경험치 흡수 반경 매우 감소
                    Debug.Log("경험치 흡수 반경 매우 감소");
                    //GameManager.Instance.Player.PlayerInteractHandler.AttractRadius -= currentAnomalyPenalty.value;
                }
                break;
            case AnomalyPenaltyType.PENALTY_START_HP_1:
                {
                    // HP 1로 시작
                    Debug.Log("HP 1로 시작");
                    GameManager.Instance.Player.Condition.Set(AttributeType.Hp, currentAnomalyPenalty.value);
                }
                break;
            case AnomalyPenaltyType.PENALTY_MONSTER_DENSITY:
                {
                    // 몬스터 스폰 밀도 증가
                    Debug.Log("몬스터 스폰 밀도 증가");
                    GameManager.Instance.spawnManager.AddSpawnPenalty((int)currentAnomalyPenalty.value);
                }
                break;
            case AnomalyPenaltyType.PENALTY_MONSTER_DAMAGE:
                {
                    // 몬스터 공격력 스탯 증가
                    Debug.Log("몬스터 공격력 스탯 증가");
                    GameManager.Instance.spawnManager.AddAdditionalDamageStat(currentAnomalyPenalty.value);
                }
                break;
            case AnomalyPenaltyType.PENALTY_SUB_ADVISOR_DAMAGE:
                {
                    Debug.Log("조언자 랜덤 한명 공격력 감소");

                    // 조언자 랜덤으로 뽑기
                    currentWeaponBase = GetRandomWeaponBase(GameManager.Instance.Player.PlayerSlotUI.WeaponSlots);

                    // 조언자 공격력 감소
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Sub(AttributeType.Damage, currentAnomalyPenalty.value);
                    }
                }
                break;
            case AnomalyPenaltyType.PENALTY_SUB_ADVISOR_MOVESPEED:
                {
                    Debug.Log("조언자 랜덤 한명 이동속도 감소");

                    // 조언자 이동속도 감소
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Sub(AttributeType.MoveSpeed, currentAnomalyPenalty.value);
                    }
                }
                break;
            case AnomalyPenaltyType.PENALTY_HUD_HEALTH:
                {
                    // HP Bar 미표시
                    Debug.Log("HP Bar 미표시");
                    EventBus.SetHUDHealthLock?.Invoke(true);
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
        switch (currentAnomalyPenalty.type)
        {
            case AnomalyPenaltyType.PENALTY_NONE:
                break;
            case AnomalyPenaltyType.PENALTY_SUB_PHASE_01:
                {
                    // 순찰 시간 증가
                }
                break;
            case AnomalyPenaltyType.PENALTY_SUB_EXP_RANGE:
                {
                    // 경험치 흡수 반경 매우 감소

                }
                break;
            case AnomalyPenaltyType.PENALTY_START_HP_1:
                {
                    // HP 1로 시작
                }
                break;
            case AnomalyPenaltyType.PENALTY_MONSTER_DENSITY:
                {
                    // 몬스터 스폰 밀도 증가
                }
                break;
            case AnomalyPenaltyType.PENALTY_MONSTER_DAMAGE:
                {
                    // 몬스터 공격력 스탯 증가
                }
                break;
            case AnomalyPenaltyType.PENALTY_SUB_ADVISOR_DAMAGE:
                {
                    // 조언자 랜덤으로 뽑기
                    currentWeaponBase = GetRandomWeaponBase(GameManager.Instance.Player.PlayerSlotUI.WeaponSlots);

                    // 조언자 공격력 감소
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Add(AttributeType.Damage, currentAnomalyPenalty.value);
                    }
                }
                break;
            case AnomalyPenaltyType.PENALTY_SUB_ADVISOR_MOVESPEED:
                {
                    // 조언자 이동속도 리셋
                    if (currentWeaponBase)
                    {
                        currentWeaponBase.Stat.Add(AttributeType.MoveSpeed, currentAnomalyPenalty.value);
                    }
                }
                break;
            case AnomalyPenaltyType.PENALTY_HUD_HEALTH:
                {
                    // HP Bar 표시
                    EventBus.SetHUDHealthLock?.Invoke(false);
                }
                break;
            default:
                break;
        }

        currentAnomalyPenalty = null;
        currentWeaponBase = null;
    }

    private WeaponBase GetRandomWeaponBase(List<WeaponSlot> weaponSlots)
    {
        WeaponBase weaponBase = null;

        List<WeaponSlot> activeWeaponSlot = new List<WeaponSlot>();

        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            if (weaponSlot.IsActive && weaponSlot.WeaponBase != null)
            {
                activeWeaponSlot.Add(weaponSlot);
            }
        }

        // 랜덤으로 뽑기
        if (activeWeaponSlot.Count != 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, activeWeaponSlot.Count);
            weaponBase = activeWeaponSlot[randomIndex].WeaponBase;
        }

        return weaponBase;
    }

    #region 프로퍼티

    public AnomalyPenalty AnomalyPenalty { get { return currentAnomalyPenalty; } set { currentAnomalyPenalty = value; } }

        #endregion
    }
