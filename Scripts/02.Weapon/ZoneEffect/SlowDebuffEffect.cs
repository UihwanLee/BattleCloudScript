using System.Collections.Generic;
using UnityEngine;

public class SlowDebuffEffect : MonoBehaviour, IZoneEffect
{
    [Header("Status")]
    // 0 ~ 1 사이의 값 / 만약 10% 같이 n%로 값이 들어온다면 값 조정
    [SerializeField] private float slowRatio = 0.2f;

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
            if (go.TryGetComponent<BaseController>(out var component))
            {
                component.RemoveSlow(this);
            }
        }

        timers.Clear();
    }

    public void OnEnter(GameObject target)
    {
        timers[target] = 0f;

        if (target.TryGetComponent<BaseController>(out var component))
        {
            component.ApplySlow(this, slowRatio);
        }
    }

    public void OnExit(GameObject target)
    {
        timers.Remove(target);

        if (target.TryGetComponent<BaseController>(out var component))
        {
            component.RemoveSlow(this);
        }
    }

    public bool OnStay(GameObject target, float deltaTime)
    {
        timers[target] += deltaTime;

        if (timers[target] >= owner.Stat.AttackInterval.Value)
        {
            timers[target] = 0f;
            return true;
        }

        return false;
    }
}
