using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class StackAOEWithFlatOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int currentCount = 0;
    private int count;
    private float damage;
    private float range;
    private float delay;

    public StackAOEWithFlatOnHitEffect(int itemId, Effect effect)
    {
        ItemId = itemId;
        this.effect = effect;
        range = 1f;
        delay = 0.2f;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        count = (int)effect.ValueList[0];
        damage = effect.ValueList[1];
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
                type: ImpactType.Fire,
                position: hitInfo.Position,
                damage: damage,
                range: range,
                delay: delay,
                layerMask: LayerMask.GetMask("Monster")
                );
        }
    }
}
