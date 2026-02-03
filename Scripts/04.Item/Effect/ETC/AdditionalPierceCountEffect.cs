public class AdditionalPierceCountEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; private set; }
    private Effect effect;
    private WeaponBase weapon;

    public AdditionalPierceCountEffect(int id, Effect effect)
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
        if (weapon.TryGetComponent<BasicWeaponController>(out var controller))
        {
            controller.AddPierceCount((int)effect.Value);
        }
    }

    public void OnUnequip()
    {
        if (weapon.TryGetComponent<BasicWeaponController>(out var controller))
        {
            controller.SubPierceCount((int)effect.Value);
        }
    }
}
