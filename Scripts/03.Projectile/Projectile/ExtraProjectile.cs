using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ExtraProjectile : BasicProjectile
{
    protected override void OnHit(IDamageable target)
    {
        target.TakeDamage(damage, owner);

        //CreateImpact(impactType);

        // Projectile Release
        poolManager.Release(projectileType, this);
    }
}
