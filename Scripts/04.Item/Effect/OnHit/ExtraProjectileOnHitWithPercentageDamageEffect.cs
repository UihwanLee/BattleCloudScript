using UnityEngine;

public class ExtraProjectileOnHitWithPercentageDamageEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int shotCount;
    private float damageMultiplier;
    private float fireChance;
    private float shotInterval = 0.2f;

    public ExtraProjectileOnHitWithPercentageDamageEffect(int id, Effect effect)
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
        shotCount = 1;
        fireChance = effect.ValueList[0];
        damageMultiplier = effect.ValueList[1] * 0.01f;

        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnHit -= OnHit;
    }

    public void OnHit(HitInfo hitInfo)
    {
        int randNum = Random.Range(0, 100);

        if (randNum < fireChance)
        {
            Transform target = weapon.Controller.FindNearestTarget();
            weapon.Controller.StartExtraFire(shotCount, weapon.Stat.Damage * damageMultiplier, target, shotInterval);
        }
    }
}
