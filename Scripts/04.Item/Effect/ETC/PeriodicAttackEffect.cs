using UnityEngine;

public class PeriodicAttackEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private float period;
    private int shotCount;
    private float damageMultiplier;
    private float interval;

    public PeriodicAttackEffect(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;

        interval = 0.2f;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        period = effect.ValueList[0];
        shotCount = (int)effect.ValueList[1];
        damageMultiplier = effect.ValueList[2] / 100;

        Transform target = weapon.Controller.FindRandomTarget();
        weapon.Controller.StartPeriodicAttack(period, shotCount, damageMultiplier, target, interval);
    }

    public void OnUnequip()
    {
        weapon.Controller.StopPeriodicAttack();
    }
}
