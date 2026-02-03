using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class DrainOnHitEffect : IWeaponEffectInstance, IRebindableWeaponEffect
{
    public int ItemId { get; }

    private Effect effect;
    private WeaponBase weapon;

    private float chance;
    private float healAmount;

    public DrainOnHitEffect(int id, Effect effect)
    {
        ItemId = id;
        this.effect = effect;
    }

    public void Bind(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

    public void OnEquip()
    {
        chance = effect.ValueList[0];
        healAmount = effect.ValueList[1];
        weapon.Controller.OnHit += OnHit;
    }

    public void OnUnequip()
    {
        weapon.Controller.OnHit -= OnHit;
    }

    public void OnHit(HitInfo hitInfo)
    {
        if (chance <= Random.Range(0, 100))
            return;

        GameManager.Instance.Player.Condition.Add(AttributeType.Hp, healAmount);
        FloatingTextPoolManager.Instance.SpawnText(FloatingTextType.Heal.ToString(), healAmount.ToString("0"), GameManager.Instance.Player.transform);
    }
}
