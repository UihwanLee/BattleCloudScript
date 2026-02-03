using WhereAreYouLookinAt.Enum;

public class PeriodicBombEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;

    private float period;
    private float damageMultiplier;
    private float range;

    public PeriodicBombEffect(int id, Effect effect)
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
        period = effect.ValueList[0];
        damageMultiplier = effect.ValueList[1] * 0.01f;
        range = 3f;

        weapon.Controller.StartPeriodicBomb(period, ImpactType.Explosion, damageMultiplier, range);
    }

    public void OnUnequip()
    {
        weapon.Controller.StopPeriodicBomb();
    }
}
