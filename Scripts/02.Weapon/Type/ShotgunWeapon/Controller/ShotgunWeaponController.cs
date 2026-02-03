using UnityEngine;

public class ShotgunWeaponController : BasicWeaponController
{
    //[Header("확산 각도")]
    //[SerializeField] float spreadAngle;
    //[Header("공격 당 발사되는 투사체 수")]
    //[SerializeField] int projectilesPerShot;

    //public override void FireOneShot()
    //{
    //    RotateFirePoint();

    //    float startAngleZ = firePoint.rotation.eulerAngles.z - (spreadAngle / 2);
    //    float angleStep = spreadAngle / projectilesPerShot;
        
    //    for (int i = 0; i < projectilesPerShot; i++)
    //    {
    //        BasicProjectile projectile = poolManager.GetProjectile(projectileType);

    //        projectile.transform.position = firePoint.position;

    //        Vector3 newRotation = Vector3.zero;
    //        newRotation.z = startAngleZ + (angleStep * i);
    //        projectile.transform.rotation = Quaternion.Euler(newRotation);
    //        projectile.Initialize(
    //            owner: weapon,
    //            targetLayer: target,
    //            type: projectileType,
    //            existenceDistance: stat.Range.Value,
    //            damage: stat.Damage.Value,
    //            knockbackPower: stat.KnockBack.Value
    //            );

    //        projectile.gameObject.SetActive(true);
    //    }
    //}

    //public void AddProjectilePerShot(int amount)
    //{
    //    projectilesPerShot += amount;
    //}

    //public void SubProjectilePerShot(int amount)
    //{
    //    projectilesPerShot -= amount;
    //}
}
