using WhereAreYouLookinAt.Enum;

public class BaseExclusiveEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private WeaponBase weapon;
    Effect effect;

    public BaseExclusiveEffect(int id, Effect effect)
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
        if (!effect.CheckExclusiveWeaponMatch(weapon.WeaponType))
            return;

        float valuePercentage = (100 + effect.Value) / 100;
        float attackIntervalValue = (100 - effect.Value) / 100;

        weapon.Stat.AddMultiplier(AttributeType.Damage, valuePercentage);
        weapon.Stat.AddMultiplier(AttributeType.AttackInterval, attackIntervalValue);
        weapon.Stat.AddMultiplier(AttributeType.MoveSpeed, valuePercentage);
        weapon.Stat.AddMultiplier(AttributeType.KnockBack, valuePercentage);
    }

    public void OnUnequip()
    {
        if (!effect.CheckExclusiveWeaponMatch(weapon.WeaponType))
            return;

        float valuePercentage = (100 + effect.Value) / 100;
        float attackIntervalValue = (100 - effect.Value) / 100;

        weapon.Stat.RemoveMultiplier(AttributeType.Damage, valuePercentage);
        weapon.Stat.RemoveMultiplier(AttributeType.AttackInterval, attackIntervalValue);
        weapon.Stat.RemoveMultiplier(AttributeType.MoveSpeed, valuePercentage);
        weapon.Stat.RemoveMultiplier(AttributeType.KnockBack, valuePercentage);
    }
}
