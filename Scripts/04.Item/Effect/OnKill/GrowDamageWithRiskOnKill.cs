using WhereAreYouLookinAt.Enum;

public class GrowDamageWithRiskOnKill : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int killsRequired;
    private int growDamage;
    private int risk;

    private int killsRemaining;
    private int growCount;

    public GrowDamageWithRiskOnKill(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;

        killsRemaining = 0;
        growCount = 0;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        killsRequired = (int)effect.ValueList[0];
        growDamage = (int)effect.ValueList[1];
        risk = (int)effect.ValueList[2];

        weapon.Controller.OnKill += OnKill;
        weapon.Stat.Add(AttributeType.Damage, growCount * growDamage);
        GameManager.Instance.Player.Stat.Sub(AttributeType.Defense, growCount * risk);
    }

    public void OnUnequip()
    {
        weapon.Controller.OnKill -= OnKill;
        weapon.Stat.Sub(AttributeType.Damage, growCount * growDamage);
        GameManager.Instance.Player.Stat.Add(AttributeType.Defense, growCount * risk);
    }

    private void OnKill(HitInfo hitInfo)
    {
        killsRemaining++;

        if (killsRemaining >= killsRequired)
        {
            killsRemaining -= killsRequired;
            growCount++;

            weapon.Stat.Add(AttributeType.Damage, growDamage);
            GameManager.Instance.Player.Stat.Sub(AttributeType.Defense, risk);
        }
    }
}
