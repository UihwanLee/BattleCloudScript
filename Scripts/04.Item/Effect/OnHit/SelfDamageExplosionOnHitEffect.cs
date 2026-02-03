using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class SelfDamageExplosionOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int hpCost;
    private float damageMultiplier;
    private float range;
    private float delay;

    private PlayerCondition condition;

    public SelfDamageExplosionOnHitEffect(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;
        range = 2f;
        delay = 0.2f;
        this.effect = effect;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        hpCost = (int)effect.ValueList[0];
        damageMultiplier = effect.ValueList[1] / 100;

        condition = GameManager.Instance.Player.Condition;
        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnHit -= OnHit;
        condition = null;
    }

    private void OnHit(HitInfo hitInfo)
    {
        condition.Sub(AttributeType.Hp, hpCost);
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
