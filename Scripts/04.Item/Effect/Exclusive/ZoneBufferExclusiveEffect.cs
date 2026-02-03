using WhereAreYouLookinAt.Enum;

public class ZoneBufferExclusiveEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private WeaponBase weapon;
    private Effect effect;

    public ZoneBufferExclusiveEffect(int id, Effect effect)
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

        weapon.Stat.Add(AttributeType.Range, effect.Value);
    }

    public void OnUnequip()
    {
        if (!effect.CheckExclusiveWeaponMatch(weapon.WeaponType))
            return;

        weapon.Stat.Sub(AttributeType.Range, effect.Value);
    }
}
