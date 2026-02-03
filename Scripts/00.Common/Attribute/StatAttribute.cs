using System;
using UnityEngine;

[Serializable]
public class StatAttribute : Attribute
{
    [SerializeField] private float baseValue;
    [SerializeField] private float additive;
    [SerializeField] private float multiplier = 1f;
    [SerializeField] private float fixedValue;
    [SerializeField] private bool isFixed = false;

    #region 프로퍼티
    public float FixedValue => fixedValue;
    public bool IsFixed => isFixed;
    public float BaseValue => baseValue;
    public float Additive => additive;
    public float Multiplier => multiplier;
    #endregion

    public StatAttribute(int localIndex, float baseValue, float minValue)
        : base(localIndex, baseValue, minValue)
    {
        this.baseValue = baseValue;
        Recalculate();
    }

    public override void Add(float amount)
    {
        additive += amount;
        Recalculate();
    }

    public override void Sub(float amount)
    {
        additive -= amount;
        Recalculate();
    }

    public override void Set(float amount)
    {
        SetBase(amount);
    }

    public void SetFixedValue(float amount)
    {
        isFixed = true;
        fixedValue = amount;
        Recalculate();
    }

    public void DisableFixedValue()
    {
        isFixed = false;
        fixedValue = 0f;
        Recalculate();
    }

    public void SetBase(float amount)
    {
        baseValue = amount;
        Recalculate();
    }

    public void AddMultiplier(float value)
    {
        multiplier *= value;
        Recalculate();
    }

    public void RemoveMultiplier(float value)
    {
        multiplier /= value;
        Recalculate();
    }

    public void ResetModifier()
    {
        additive = 0f;
        multiplier = 1f;
        Recalculate();
    }

    public void ResetMultiplier()
    {
        multiplier = 1f;
        Recalculate();
    }

    public void ResetValue()
    {
        value = baseValue;
        additive = 0f;
        multiplier = 1f;
        Recalculate();
    }

    private void Recalculate()
    {
        if (isFixed)
            value = fixedValue;
        else
            value = (baseValue + additive) * multiplier;

        value = MathF.Max(value, minValue);
    }
}
