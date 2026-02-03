using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// 아이템 효과를 담당할 Effect 클래스
public class Effect 
{
    [Header("Effect 정보")]
    [SerializeField] private ItemEffectType effectType;
    [SerializeField] private ItemTriggerType triggerType;
    [SerializeField] private ItemApplyTarget applyTarget;
    [SerializeField] private List<ItemApplyType> applyTypeList = new List<ItemApplyType>();
    [SerializeField] private List<float> valueList = new List<float>();
    [SerializeField] private List<float> valueMultiplierList = new List<float>();

    private int currentValueIndex = 0;
    private int currentValueTypeIndex = 0;

    public void Set(ItemEffectType _effectType, ItemTriggerType _triggerType, ItemApplyTarget _applyTarget, ItemApplyType _applyType, float _value)
    {
        // Effect 전체 설정
        effectType = _effectType;
        triggerType = _triggerType;
        applyTarget = _applyTarget;
        applyTypeList[currentValueTypeIndex] = _applyType;

        valueList[currentValueIndex] = _value;
    }

    public void SetEffectType(ItemEffectType _effectType)
    {
        effectType = _effectType;
    }

    public void SetTriggerType(ItemTriggerType _triggerType)
    {
        triggerType = _triggerType;
    }

    public void SetApplyTarget(ItemApplyTarget _applyTarget)
    {
        applyTarget = _applyTarget;
    }

    public void SetApplyType(ItemApplyType _applyType)
    {
        applyTypeList.Add(_applyType);
    }

    public void ResetValue()
    {
        applyTypeList.Clear();
        valueList.Clear();
        valueMultiplierList.Clear();
    }

    public void SetValue(float _value)
    {
        valueList.Add(_value);
    }

    public void SetValueMultlier(float _value)
    {
        valueMultiplierList.Add(_value);
    }

    public void ApplyMultiplier()
    {
        for(int i=0; i<valueList.Count; i++)
        {
            valueList[i] = valueList[i] + valueMultiplierList[i];
        }
    }

    //public void Apply(Player player, bool add, int index)
    //{
    //    player.PlayerEffectHandler.ApplyItemByWeapon(this, add, index);
    //}

    /// <summary>
    /// 현재 EffectType이 WeaponType과 일치하는지 반환하는 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool CheckExclusiveWeaponMatch(WeaponType type)
    {
        return Enum.TryParse(effectType.ToString(), out WeaponType parsed) && parsed == type;
    }

    /// <summary>
    /// 하나라도 WeaponType에 맞는지 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public bool CheckExclusive()
    {
        return Enum.TryParse<WeaponType>(effectType.ToString(), out _);
    }

    #region 프로퍼티

    public ItemEffectType EffectType { get { return effectType; } }
    public ItemTriggerType TriggerType { get {return triggerType; } }
    public ItemApplyTarget ApplyTarget { get {return applyTarget; } }
    public ItemApplyType ApplyType { get { return applyTypeList[0]; } }
    public List<ItemApplyType> ApplyTypeList { get { return applyTypeList; } }
    public float Value { get { return valueList[0]; } }
    public float ValueMultiplier { get { return valueMultiplierList[0]; } }
    public List<float> ValueList { get { return valueList; } }
    public List<float> ValueMultiList { get { return valueMultiplierList; } }

    #endregion
}
