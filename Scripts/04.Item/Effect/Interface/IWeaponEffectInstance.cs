public interface IWeaponEffectInstance
{
    int ItemId { get; }
    void OnEquip();
    void OnUnequip();
}
