using WhereAreYouLookinAt.Enum;

public class HealWeapon : WeaponBase<HealWeaponController>
{
    private void Reset()
    {
        weaponType = WeaponType.TargetHealer;
    }
}
