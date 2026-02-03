using System;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public static class EventBus
{
    private static readonly Dictionary<AttributeType, Action<float>> events
        = new Dictionary<AttributeType, Action<float>>();

    public static Action<ISlotItem, Attribute> OnDropItemGain;
    public static Action<ISlotItem, Attribute, Vector3?> OnMouseOnSlot;
    public static Action<string, string, Vector3?> OnMouseOnInfo;
    public static Action OnMouseOffSlot;
    public static Action OnPlayerSlotUIOpen;
    public static Action OnPlayerSlotUIClose;
    public static Action<int, int> OnChangeWeaponSlotCount;
    public static Action<float> OnMonsterKilled;
    public static Action<float, int> OnTimerChanged;
    public static Action<int> OnDayChanged;
    public static Action<float> OnMaxGaugeChange;
    public static Action<float> OnCurrentGaugeChange;
    public static Action<int> OnPhaseStart;
    public static Action OnPlayerMoveStart;
    public static Action OnPlayerMoveStop;

    public static Action<float> OnPlayerLevelUp;
    public static Action<Sprite> OnPlayerStart;

    public static Action<WeaponBase, float> OnTakeDamageByWeapon;       // 조언자가 데미지를 입힐 때 발생

    public static Action<ISlotItem> OnGainItem;                         // 몬스터에서 드롭된 <무기,아이템> 획득 시 발생
    public static Action OnPhaseDone;                                   // Phase가 모두 끝났을 때 발생
    public static Action<List<HUDItemSlot>> OnCheckDropItem;            // Phase가 모두 종료되고 HUD에 있는 <무기,아이템> 상자화 이벤트

    public static Action<bool> SetHUDHealthLock;                        // 이상현상 패널티 : HUD 체력 표시 활성/비활성화

    public static Action<List<ISlotItem>> OnGainItemByAnomaly;          // 이상현상 보상으로 아이템 획득 이벤트       
    public static Action<AnomalyResultManager> OnAnomalyResultApply;    // 이상현상 결과 적용 시 발생

    public static Action<int, WeaponBase> OnWeaponLevelUp;

    public static Action SpawnPlayer;                                   // 처음 플레이어 스폰 시 이벤트

    public static Action OnClearGame;                                   // 게임 클리어 시 이벤트

    /// <summary>
    /// Attribute 이벤트 구독
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action">이벤트에 연결 Action</param>
    public static void Subscribe(AttributeType type, Action<float> action)
    {
        if (!events.ContainsKey(type))
            events[type] = null;

        events[type] += action;
    }

    /// <summary>
    /// Attribute 이벤트 구독 해제
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action">이벤트 해제 Action</param>
    public static void UnSubscribe(AttributeType type, Action<float> action)
    {
        if (events.ContainsKey(type))
            events[type] -= action;
    }

    /// <summary>
    /// Attribute Invoke 호출
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value">변경되는 값</param>
    public static bool Publish(AttributeType type, float value)
    {
        if (events.TryGetValue(type, out Action<float> action))
        {
            action?.Invoke(value);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void Clear()
    {
        events.Clear();
    }
}
