using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class PierceWeaponController : BasicWeaponController
{
    //[Header("Pierce")]
    //[SerializeField] private int pierceCount;

    protected override void Reset()
    {
        base.Reset();

        projectileType = ProjectileType.PlayerPierce;
        pierceCount = 1;
    }

    //public override void FireOneShot()
    //{
    //    RotateFirePoint();
    //    BasicProjectile projectile = poolManager.GetProjectile(projectileType);

    //    if (projectile.TryGetComponent<PierceProjectile>(out var component))
    //    {
    //        component.transform.position = firePoint.position;
    //        component.transform.rotation = firePoint.rotation;

    //        component.Initialize(
    //            owner: weapon,
    //            targetLayer: target,
    //            type: projectileType,
    //            pierceCount: pierceCount,
    //            existenceDistance: stat.Range.Value,
    //            damage: stat.Damage.Value,
    //            knockbackPower: stat.KnockBack.Value
    //            );

    //        component.gameObject.SetActive(true);
    //    }
    //}

    //public void AddPierceCount(int count = 1)
    //{
    //    pierceCount += count;
    //}
}
