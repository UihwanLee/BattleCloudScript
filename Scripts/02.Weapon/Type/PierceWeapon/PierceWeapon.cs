using WhereAreYouLookinAt.Enum;

public class PierceWeapon : BasicWeapon
{
    protected override void Reset()
    {
        weaponType = WeaponType.Piercing;
    }
}
