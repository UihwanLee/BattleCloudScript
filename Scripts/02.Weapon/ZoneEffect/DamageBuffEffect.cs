using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using WhereAreYouLookinAt.Enum;

public class DamageBuffEffect : MonoBehaviour, IZoneEffect
{
    private Dictionary<GameObject, float> timers = new Dictionary<GameObject, float>();
    private WeaponBase owner;

    public WeaponBase Owner => owner;

    public void Initialize(WeaponBase owner)
    {
        this.owner = owner;
    }

    private void OnDisable()
    {
        foreach (GameObject go in timers.Keys)
        {
            if (go == null) continue;
            if (go.TryGetComponent<WeaponStat>(out var component))
            {
                component.Sub(AttributeType.Damage, owner.Stat.Damage);
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
            component.Add(AttributeType.Damage, owner.Stat.Damage);
        }
    }

    public void OnExit(GameObject target)
    {
        if (target == owner.gameObject) return;

        timers.Remove(target);

        if (target.TryGetComponent<WeaponStat>(out var component))
        {
            component.Sub(AttributeType.Damage, owner.Stat.Damage);
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
