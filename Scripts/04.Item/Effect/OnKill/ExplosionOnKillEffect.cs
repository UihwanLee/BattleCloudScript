using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ExplosionOnKillEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private float damageMultiplier;
    private float extent;
    private float delay;

    public ExplosionOnKillEffect(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;

        extent = 2f;
        delay = 0.2f;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        damageMultiplier = effect.Value / 100;
        weapon.Controller.OnKill += OnKill;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnKill += OnKill;
    }

    private void OnKill(HitInfo hitInfo)
    {
        weapon.Controller.StartExplosionWithDelay(
            type: ImpactType.Dust,
            position: hitInfo.Position,
            damage: weapon.Stat.Attack.Value * damageMultiplier,
            range: extent,
            delay: delay,
            layerMask: LayerMask.GetMask("Monster")
            );
    }
}
