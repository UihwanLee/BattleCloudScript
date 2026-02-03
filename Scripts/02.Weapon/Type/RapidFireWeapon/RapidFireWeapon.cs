using WhereAreYouLookinAt.Enum;

public class RapidFireWeapon : WeaponBase<RapidFireWeaponController>
{
    private void Reset()
    {
        weaponType = WeaponType.RapidFire;
    }
}
