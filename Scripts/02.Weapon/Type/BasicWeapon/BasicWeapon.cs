using WhereAreYouLookinAt.Enum;

public class BasicWeapon : WeaponBase<BasicWeaponController>
{
    protected virtual void Reset()
    {
        weaponType = WeaponType.Base;
    }
}
