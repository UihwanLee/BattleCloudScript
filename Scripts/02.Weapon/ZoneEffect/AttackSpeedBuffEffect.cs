using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class AttackSpeedBuffEffect : MonoBehaviour, IZoneEffect
{
    [Header("Attack Speed Buff")]
    [SerializeField] private float attackSpeedIncrease;
    float resultValue;

    private Dictionary<GameObject, float> timers = new Dictionary<GameObject, float>();
    private WeaponBase owner;

    public WeaponBase Owner => owner;

    public void Initialize(WeaponBase owner)
    {
        this.owner = owner;

        resultValue = (100 - attackSpeedIncrease) / 100;
    }

    private void OnDisable()
    {
        foreach (GameObject go in timers.Keys)
        {
            if (go == null) continue;
            if (go.TryGetComponent<WeaponStat>(out var component))
            {
                component.RemoveMultiplier(AttributeType.AttackInterval, resultValue);
            }
        }

        timers.Clear();
    }

    public void OnEnter(GameObject target)
    {
        if (target == owner.gameObject) return;

        timers[target] = 0f;
        if (target.TryGetComponent<WeaponStat>(out var component))
        {
            
            component.AddMultiplier(AttributeType.AttackInterval, resultValue);
        }
    }

    public void OnExit(GameObject target)
    {
        if (target == owner.gameObject) return;

        timers.Remove(target);

        if (target.TryGetComponent<WeaponStat>(out var component))
        {
            component.RemoveMultiplier(AttributeType.AttackInterval, resultValue);
        }
    }

    public bool OnStay(GameObject target, float deltaTime)
    {
        if (target == owner.gameObject) return false;

        if (owner.IsInBase) return false;

        timers[target] += deltaTime;

        if (timers[target] >= owner.Stat.AttackInterval.Value)
        {
            timers[target] = 0f;
            return true;
        }

        return false;
    }
}
