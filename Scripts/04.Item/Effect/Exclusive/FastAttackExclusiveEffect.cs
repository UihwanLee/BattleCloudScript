public class FastAttackExclusiveEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private WeaponBase weapon;
    private Effect effect;

    public FastAttackExclusiveEffect(int id, Effect effect)
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
            controller.AddShotCount((int)effect.Value);
        }
    }

    public void OnUnequip()
    {
        if (weapon.TryGetComponent<BasicWeaponController>(out var controller))
        {
            controller.SubShotCount((int)effect.Value);
        }
    }
}
