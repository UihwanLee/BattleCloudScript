using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ExplosionWeaponController : BasicWeaponController
{
    [Header("Explosion Range")]
    [SerializeField] private float explosionRange;

    protected override void Reset()
    {
        base.Reset();

        projectileType = ProjectileType.PlayerExplosion;
        explosionRange = 3f;
    }

    public override void FireOneShot()
    {
        RotateFirePoint();
        BasicProjectile projectile = poolManager.GetProjectile(projectileType);

        if (projectile.TryGetComponent<ExplosionProjectile>(out var component))
        {
            component.transform.position = firePoint.position;
            component.transform.rotation = firePoint.rotation;

            float finalDamage = CalculateFinalDamage();

            component.Initialize(
                owner: weapon,
                targetLayer: target,
                type: projectileType,
                explosionRange: explosionRange,
                existenceDistance: stat.Range.Value,
                damage: finalDamage,
                knockbackPower: stat.KnockBack.Value,
                pierceCount:pierceCount
                );

            component.gameObject.SetActive(true);
        }
    }

    public void AddExplosionRange(float range)
    {
        explosionRange += range;
    }
}
