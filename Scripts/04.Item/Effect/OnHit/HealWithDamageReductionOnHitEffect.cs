using WhereAreYouLookinAt.Enum;

public class HealWithDamageReductionOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int healAmount;
    private float damageReduction;

    public HealWithDamageReductionOnHitEffect(int id, Effect effect)
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
        healAmount = (int)effect.ValueList[0];
        damageReduction = (100 + effect.ValueList[1]) / 100;

        weapon.Stat.AddMultiplier(AttributeType.Damage, damageReduction);
        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Stat.RemoveMultiplier(AttributeType.Damage, damageReduction);
        weapon.Controller.OnHit -= OnHit;
    }

    private void OnHit(HitInfo hitInfo)
    {
        GameManager.Instance.Player.Condition.Add(AttributeType.Hp, healAmount);
    }
}
