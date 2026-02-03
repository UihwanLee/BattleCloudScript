using WhereAreYouLookinAt.Enum;

public class HealOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int healAmount;

    public HealOnHitEffect(int id, Effect effect)
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
        healAmount = (int)effect.Value;
        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnHit -= OnHit;
    }

    private void OnHit(HitInfo hitInfo)
    {
        GameManager.Instance.Player.Condition.Add(AttributeType.Hp, healAmount);
    }
}
