using NPOI.OpenXmlFormats.Dml.Diagram;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class HomingAttackOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;

    private float fireChance;
    private float damageMultiplier;

    public HomingAttackOnHitEffect(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        fireChance = effect.ValueList[0];
        damageMultiplier = 1 - (effect.ValueList[1] * 0.01f);
        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnHit -= OnHit;
    }

    private void OnHit(HitInfo hitInfo)
    {
        if (Random.Range(0, 100) >= fireChance)
            return;

        // 무조건 튕기도록 target이 null이면 다시 탐색
        Transform target = weapon.Controller.FindNearestTarget(hitInfo.Target.Collider); ;

        if (target == null)
            return;

        BasicProjectile projectile = ProjectilePoolManager.Instance.GetProjectile(ProjectileType.PlayerHoming);

        if (!projectile.TryGetComponent<HomingProjectile>(out var homing))
            return;

        Vector3 startPos = hitInfo.Position;
        Vector3 targetPos = target.transform.position;

        Vector2 dir = (targetPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        homing.transform.position = startPos;
        homing.transform.rotation = Quaternion.Euler(0, 0, angle);

        homing.Initialize(
            owner: weapon,
            targetLayer: LayerMask.GetMask("Monster"),
            type: ProjectileType.PlayerHoming,
            existenceDistance: 999,
            damage: weapon.Stat.Damage * damageMultiplier,
            knockbackPower: 0f,
            pierceCount: 0
            );

        homing.SetTarget(target);
        homing.SetExclude(hitInfo.Target.Collider);
    }
}
