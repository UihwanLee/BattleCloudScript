using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class StatChangeWithFlatEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private AttributeType type;
    private float value;

    public StatChangeWithFlatEffect(int id, AttributeType type, Effect effect)
    {
        ItemId = id;
        this.effect = effect;
        this.type = type;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        if (type == AttributeType.AttackInterval)
            weapon.Stat.Add(type, -effect.Value);
        else
            weapon.Stat.Add(type, effect.Value);
    }

    public void OnUnequip()
    {
        if (type == AttributeType.AttackInterval)
            weapon.Stat.Sub(type, -effect.Value);
        else
            weapon.Stat.Sub(type, effect.Value);
    }
}
