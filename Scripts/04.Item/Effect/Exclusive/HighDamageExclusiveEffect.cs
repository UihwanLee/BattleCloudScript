using WhereAreYouLookinAt.Enum;

public class HighDamageExclusiveEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private WeaponBase weapon;
    private Effect effect;

    public HighDamageExclusiveEffect(int id, Effect effect)
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
        weapon.Stat.SetFixedValue(AttributeType.Damage, effect.ValueList[0]);

        if (weapon.TryGetComponent<BasicWeaponController>(out var controller))
        {
            controller.SetEnabledHighDamageExclusiveItem(true, effect.ValueList[1]);
        }
    }

    public void OnUnequip()
    {
        weapon.Stat.DisableFixedValue(AttributeType.Damage);

        if (weapon.TryGetComponent<BasicWeaponController>(out var controller))
        {
            controller.SetEnabledHighDamageExclusiveItem(false, effect.ValueList[1]);
        }
    }
}
