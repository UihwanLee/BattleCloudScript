using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class AnomalyResultManager : MonoBehaviour
{
    private static AnomalyResultManager instance;

    public static AnomalyResultManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindFirstObjectByType<AnomalyResultManager>();
            }
            return instance;
        }
    }

    private AnomalyResultManager() { }

    [Header("이상현상 결과 세팅")]
    [Header("이상현상 보상 세팅")]
    [SerializeField] private AnomalyBenfitType benfitType = AnomalyBenfitType.UNDEF;
    [Header("이상현상 패널티 세팅")]
    [SerializeField] private AnomalyPenaltyType penaltyType = AnomalyPenaltyType.UNDEF;

    [SerializeField] private AnomalyBenefitManager anomalyBenefitManager = new AnomalyBenefitManager();
    [SerializeField] private AnomalyPenaltyManager anomalyPenaltyManager = new AnomalyPenaltyManager();

    private AnomalyResultType anomalyResult = AnomalyResultType.UNDEF;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// 이상현상 보상 세팅
    /// </summary>
    public void SetAnomalyBenefit()
    {
        if(benfitType != AnomalyBenfitType.UNDEF)
        {
            // 만약 타입이 지정되어 있다면 그 타입으로 실행
            anomalyBenefitManager.SetAnomalyBenefitByType(benfitType);
        }
        else
        {
            // 아니면 랜덤으로 뽑기
            anomalyBenefitManager.SetRandomAnomalyBenefit();
        }

        // 타입 지정
        anomalyResult = AnomalyResultType.Benefit;
    }

    /// <summary>
    /// 이상현상 패널티 세팅
    /// </summary>
    public void SetAnomalyPenalty()
    {
        if (penaltyType != AnomalyPenaltyType.UNDEF)
        {
            // 만약 타입이 지정되어 있다면 그 타입으로 실행
            anomalyPenaltyManager.SetAnomalyPenaltyByType(penaltyType);
        }
        else
        {
            // 아니면 랜덤으로 뽑기
            anomalyPenaltyManager.SetRandomAnomalyPenalty();
        }

        // 타입 지정
        anomalyResult = AnomalyResultType.Penalty;
    }

    /// <summary>
    /// 이상현상 결과 적용
    /// </summary>
    public void ApplyAnomalyResult()
    {
        switch(anomalyResult)
        {
            case AnomalyResultType.Benefit:
                {
                    anomalyBenefitManager.ExcuteBenefit();
                }
                break;
            case AnomalyResultType.Penalty:
                {
                    anomalyPenaltyManager.ExcutePenalty();
                }
                break;
            case AnomalyResultType.UNDEF:
                {

                }
                break;
            default:
                break;
        }

        // 결과 UI 띄우기
        EventBus.OnAnomalyResultApply?.Invoke(this);
    }

    #region 프로퍼티

    public AnomalyResultType AnomalyResult { get { return anomalyResult; } } 
    public AnomalyBenefitManager BenefitManager { get { return anomalyBenefitManager; } }
    public AnomalyPenaltyManager PenaltyManager { get { return anomalyPenaltyManager; } }

    #endregion
}
