public class ShotGunExclusiveEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private WeaponBase weapon;
    private Effect effect;

    public ShotGunExclusiveEffect(int id, Effect effect)
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

        if (weapon.TryGetComponent<ShotgunWeaponController>(out var controller))
        {
            controller.AddProjectilePerShot((int)effect.Value);
        }
    }

    public void OnUnequip()
    {
        if (!effect.CheckExclusiveWeaponMatch(weapon.WeaponType))
            return;

        if (weapon.TryGetComponent<ShotgunWeaponController>(out var controller))
        {
            controller.SubProjectilePerShot((int)effect.Value);
        }
    }
}
