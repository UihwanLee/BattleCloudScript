using WhereAreYouLookinAt.Enum;

public class GrowDamageWithSpeedRiskOnKill : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int killsRequired;
    private int growDamage;
    private int riskSpeed;

    private int killsRemaining;
    private int growCount;

    public GrowDamageWithSpeedRiskOnKill(int id, Effect effect)
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
        riskSpeed = (int)effect.ValueList[2];

        weapon.Controller.OnKill += OnKill;
        weapon.Stat.Add(AttributeType.Damage, growDamage);
        weapon.Stat.Add(AttributeType.MoveSpeed, riskSpeed);
        weapon.Stat.Add(AttributeType.AttackInterval, -riskSpeed);
    }

    public void OnUnequip()
    {
        weapon.Controller.OnKill -= OnKill;
        weapon.Stat.Sub(AttributeType.Damage, growDamage);
        weapon.Stat.Sub(AttributeType.MoveSpeed, riskSpeed);
        weapon.Stat.Sub(AttributeType.AttackInterval, -riskSpeed); 
    }

    private void OnKill(HitInfo hitInfo)
    {
        killsRemaining++;

        if (killsRemaining >= killsRequired)
        {
            killsRemaining -= killsRequired;
            growCount++;

            weapon.Stat.Add(AttributeType.Damage, growDamage);
            weapon.Stat.Add(AttributeType.MoveSpeed, riskSpeed);
            weapon.Stat.Add(AttributeType.AttackInterval, -riskSpeed);
        }
    }
}
