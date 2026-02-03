using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class StackAOEWithPercentageOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int currentCount = 0;
    private int count;
    private float damageMultiplier;
    private float range;
    private float delay;

    public StackAOEWithPercentageOnHitEffect(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;
        range = 5f;
        delay = 0.2f;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        count = (int)effect.ValueList[0];
        damageMultiplier = effect.ValueList[1] / 100;
        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnHit -= OnHit;
    }

    private void OnHit(HitInfo hitInfo)
    {
        currentCount++;

        if (currentCount >= count)
        {
            currentCount = 0;

            weapon.Controller.StartExplosionWithDelay(
                type: ImpactType.Dust,
                position: hitInfo.Position,
                damage: hitInfo.Damage * damageMultiplier,
                range: range,
                delay: delay,
                layerMask: LayerMask.GetMask("Monster")
                );
        }
    }
}
