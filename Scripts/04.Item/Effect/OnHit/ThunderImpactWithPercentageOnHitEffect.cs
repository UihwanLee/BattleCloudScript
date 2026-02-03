using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ThunderImpactWithPercentageOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;

    private float damageMultiplier;
    private float chance;

    public ThunderImpactWithPercentageOnHitEffect(int id, Effect effect)
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
        chance = effect.ValueList[0];
        damageMultiplier = effect.ValueList[1] * 0.01f;
        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnHit -= OnHit;
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (chance > Random.Range(0, 100))
        {
            weapon.Controller.SpawnThunder(ImpactType.Thunder, weapon.Stat.Damage *  damageMultiplier);
        }
    }
}
