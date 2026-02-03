using WhereAreYouLookinAt.Enum;

public class GrowDamageOnKillEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;
    private int killsRequired;
    private int growDamage;

    private int killsRemaining;
    private int growCount;

    public GrowDamageOnKillEffect(int id, Effect effect)
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

        weapon.Controller.OnKill += OnKill;
        weapon.Stat.Add(AttributeType.Damage, growCount * growDamage);
    }

    public void OnUnequip()
    {
        weapon.Controller.OnKill -= OnKill;
        weapon.Stat.Sub(AttributeType.Damage, growCount * growDamage);
    }

    private void OnKill(HitInfo hitInfo)
    {
        killsRemaining++;

        if (killsRemaining >= killsRequired)
        {
            killsRemaining -= killsRequired;
            growCount++;

            weapon.Stat.Add(AttributeType.Damage, growDamage);
        }
    }
}
