using WhereAreYouLookinAt.Enum;

public class PiercingExclusiveEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private WeaponBase weapon;
    private Effect effect;

    private int pierceCountValue;
    private float rangeValue;

    public PiercingExclusiveEffect(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;

        pierceCountValue = 99;
        rangeValue = 99f;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        if (!effect.CheckExclusiveWeaponMatch(weapon.WeaponType))
            return;

        if (weapon.TryGetComponent<PierceWeaponController>(out var controller))
        {
            controller.AddPierceCount(pierceCountValue);
            weapon.Stat.SetFixedValue(AttributeType.Range, rangeValue);
        }
    }

    public void OnUnequip()
    {
        if (!effect.CheckExclusiveWeaponMatch(weapon.WeaponType))
            return;

        if (weapon.TryGetComponent<PierceWeaponController>(out var controller))
        {
            controller.AddPierceCount(pierceCountValue);
            weapon.Stat.DisableFixedValue(AttributeType.Range);
        }
    }
}
