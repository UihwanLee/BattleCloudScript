using WhereAreYouLookinAt.Enum;

public class ZoneWeapon : WeaponBase<ZoneWeaponController>
{
    private void Reset()
    {
        weaponType = WeaponType.AreaDebuffer;
    }
}
