using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ThunderImpactWithFlatOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;

    private float damage;
    private float chance;

    public ThunderImpactWithFlatOnHitEffect(int id, Effect effect)
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
        damage = effect.ValueList[1];
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
            weapon.Controller.SpawnThunder(ImpactType.Thunder, damage);
        }
    }
}
