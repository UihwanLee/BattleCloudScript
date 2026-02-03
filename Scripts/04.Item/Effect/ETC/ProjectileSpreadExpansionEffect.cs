public class ProjectileSpreadExpansionEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;

    private float angleStep = 18f;

    public ProjectileSpreadExpansionEffect(int id, Effect effect)
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
            controller.AddProjectilePerShot((int)effect.ValueList[0]);
            controller.AddSpreadAngle((int)effect.ValueList[0] * angleStep);
        }
    }

    public void OnUnequip()
    {
        if (weapon.TryGetComponent<BasicWeaponController>(out var controller))
        {
            controller.SubProjectilePerShot((int)effect.ValueList[0]);
            controller.SubSpreadAngle((int)effect.ValueList[0] * angleStep);
        }
    }
}
