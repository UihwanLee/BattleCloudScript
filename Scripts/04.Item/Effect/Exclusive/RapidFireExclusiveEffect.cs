using WhereAreYouLookinAt.Enum;

public class RapidFireExclusiveEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private WeaponBase weapon;
    private Effect effect;

    public RapidFireExclusiveEffect(int id, Effect effect)
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
        float value = (100 - effect.Value) / 100;
        weapon.Stat.AddMultiplier(AttributeType.AttackInterval, value);
    }

    public void OnUnequip()
    {
        float value = (100 - effect.Value) / 100;
        weapon.Stat.RemoveMultiplier(AttributeType.AttackInterval, value);
    }
}
