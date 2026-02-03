using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class PierceProjectile : BasicProjectile
{
    //[Header("Projectile Type")]
    //[SerializeField] private int pierceCount;

    //protected override void Reset()
    //{
    //    base.Reset();

    //    moveSpeed = 10f;
    //}

    //public void Initialize(WeaponBase owner, LayerMask targetLayer, ProjectileType type, int pierceCount, float existenceDistance, float damage, float knockbackPower)
    //{
    //    base.Initialize(owner, targetLayer, type, existenceDistance, damage, knockbackPower);

    //    this.pierceCount = pierceCount;
    //}

    //protected override void OnHit(IDamageable target)
    //{
    //    pierceCount--;

    //    target.TakeDamage((int)damage, owner);

    //    NotifyHitResult(target);

    //    if (pierceCount < 0)
    //    {
    //        poolManager.Release(projectileType, this);
    //    }
    //}
}
