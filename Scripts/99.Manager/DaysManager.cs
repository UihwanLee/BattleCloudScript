using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaysManager : MonoBehaviour
{
    [Header("최대 일수")]
    [SerializeField] private int maxDay;
    [SerializeField] private int midDay = 10;

    [Header("액트 구간 설정")]
    [SerializeField] private int act1End;
    [SerializeField] private int act2End;

    public int CurrentDay { get; private set; } = 0;
    public int CurrentAct { get; private set; } = 1;
    public int MaxDay { get; private set; }
    public int MidDay { get; private set; }

    public float CurrentGauage { get; private set; } = 0;
    public float MaxGauage { get; private set; } = 100;

    private void OnEnable()
    {
        EventBus.OnMonsterKilled += AddCurrentGauge;
    }

    private void OnDisable()
    {
        EventBus.OnMonsterKilled -= AddCurrentGauge;
    }

    private void Start()
    {
        GameManager.Instance.DayManager = this;
        MaxDay = maxDay;
        MidDay = midDay;
        UpdateGauge();
        EventBus.OnDayChanged?.Invoke(CurrentDay);
    }

    #region Day 증가
    public void NextDay()
    {
        if (CurrentDay >= maxDay)
        {
            Debug.Log($"이미 {maxDay}일임;");
            return;
        }

        CurrentDay++;
        ResetGauge();
        EventBus.OnDayChanged?.Invoke(CurrentDay);
        UpdateGauge();
        UpdateAct();
        UpdatePlayerSlot();
        Debug.Log($"Day{CurrentDay} / Act{CurrentAct}");

        // Day가 지난 후 DropItem 삭제
        DropItemPool.Instance.ReleaseAllDropItem();
    }
    #endregion

    #region Act 계산
    private void UpdateAct()
    {
        if (CurrentDay <= act1End)
            CurrentAct = 1;
        else if (CurrentDay <= act2End)
            CurrentAct = 2;
        else
            CurrentAct = 3;
        GameManager.Instance.LightingManager.ApplyPhaseLighting(CurrentAct);
    }
    #endregion

    #region 게이지 계산
    public void AddCurrentGauge(float value)
    {
        // 몬스터가 잡힐 때마다 함수 실행
        CurrentGauage += value;
        EventBus.OnCurrentGaugeChange?.Invoke(CurrentGauage);
    }

    public bool IsGauagueFull()
    {
        return CurrentGauage >= MaxGauage;
    }

    private void ResetGauge()
    {
        CurrentGauage = 0;
        EventBus.OnCurrentGaugeChange?.Invoke(CurrentGauage);
    }

    public void UpdateGauge()
    {
        GameRule rule = GameManager.Instance.GameRule;

        if (rule == null) return;

        MaxGauage = rule.GetRequireGateEnergyByDay(CurrentDay);
        EventBus.OnMaxGaugeChange?.Invoke(MaxGauage);
    }

    #endregion

    #region 플레이어 슬롯 업데이트

    private void UpdatePlayerSlot()
    {
        if (CurrentDay % 5 == 0)
            GameManager.Instance.Player.PlayerSlotUI.AddWeaponSlot();
    }

    #endregion
}
