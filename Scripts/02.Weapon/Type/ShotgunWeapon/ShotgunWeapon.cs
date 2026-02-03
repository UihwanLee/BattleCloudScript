using WhereAreYouLookinAt.Enum;

public class ShotgunWeapon : WeaponBase<ShotgunWeaponController>
{
    private void Reset()
    {
        weaponType = WeaponType.Shotgun;
    }
}
