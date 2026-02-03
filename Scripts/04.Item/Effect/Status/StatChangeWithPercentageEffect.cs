using WhereAreYouLookinAt.Enum;

public class StatChangeWithPercentageEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private AttributeType type;
    private float value;

    public StatChangeWithPercentageEffect(int id, AttributeType type, Effect effect)
    {
        ItemId = id;
        this.effect = effect;
        this.type = type;
        this.value = (100 + value) / 100;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        if (type == AttributeType.AttackInterval)
            weapon.Stat.AddMultiplier(type, (100 - effect.Value) / 100);
        else
            weapon.Stat.AddMultiplier(type, (100 + effect.Value) / 100);
    }

    public void OnUnequip()
    {
        if (type == AttributeType.AttackInterval)
            weapon.Stat.RemoveMultiplier(type, (100 - effect.Value) / 100);
        else
            weapon.Stat.RemoveMultiplier(type, (100 + effect.Value) / 100);
    }
}
