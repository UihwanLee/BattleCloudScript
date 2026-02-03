using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class DotDamageEffect : MonoBehaviour, IZoneEffect
{
    private Dictionary<GameObject, float> timers = new Dictionary<GameObject, float>();
    private WeaponBase owner;

    public WeaponBase Owner => owner;

    public void OnEnter(GameObject target)
    {
        timers[target] = 0f;
    }

    public bool OnStay(GameObject target, float deltaTime)
    {
        bool attackSuccess = false;

        timers[target] += deltaTime;

        // 도트 데미지 적용
        if (timers[target] >= owner.Stat.AttackInterval.Value)
        {
            if (target.TryGetComponent<IDamageable>(out var component))
            {
                float finalDamage = owner.Controller.CalculateFinalDamage();
                component.TakeDamage(finalDamage, owner);
                attackSuccess = true;

                NotifyHitResult(component);
            }

            timers[target] = 0f;
        }

        return attackSuccess;
    }

    public void OnExit(GameObject target)
    {
        timers.Remove(target);
    }

    public void Initialize(WeaponBase owner)
    {
        this.owner = owner;
    }

    protected virtual void NotifyHitResult(IDamageable target)
    {
        HitInfo hit = new HitInfo
        {
            Owner = owner,
            Target = target,
            Position = transform.position,
            Damage = owner.Stat.Damage,
            IsCritical = false,
            ProjectileType = ProjectileType.None
        };

        owner.Controller.RaiseHit(hit);

        if (target is IKillable killable && killable.IsDead)
        {
            owner.Controller.RaiseKill(hit);
        }
    }
}
