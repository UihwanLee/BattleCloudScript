using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ExplosionProjectile : BasicProjectile
{
    [Header("Explosion Range")]
    [SerializeField] private float explosionRange;

    protected override void Reset()
    {
        base.Reset();

        // default explosion range
        explosionRange = 3f;
        impactType = ImpactType.Explosion;
    }

    public void Initialize(WeaponBase owner, LayerMask targetLayer, ProjectileType type, float explosionRange, float existenceDistance, float damage, float knockbackPower, int pierceCount)
    {
        base.Initialize(owner, targetLayer, type, existenceDistance, damage, knockbackPower, pierceCount);

        this.explosionRange = explosionRange;
    }

    protected override void OnHit(IDamageable target)
    {
        pierceCount--;

        ImpactBase impact = CreateImpact(impactType);

        if (impact.TryGetComponent<ExplosionImpact>(out var explosionImpact))
        {
            explosionImpact.Initialize(impactType, owner, damage, explosionRange, targetLayer);
        }

        NotifyHitResult(target);

        if (pierceCount < 0)
            poolManager.Release(projectileType, this);
    }
}
