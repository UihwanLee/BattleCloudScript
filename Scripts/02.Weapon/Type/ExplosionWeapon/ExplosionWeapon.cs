using WhereAreYouLookinAt.Enum;

public class ExplosionWeapon : BasicWeapon
{
    protected override void Reset()
    {
        weaponType = WeaponType.Explosive;
    }
}
