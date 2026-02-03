using System.Collections.Generic;
using UnityEngine;

public class WeaponEffectController : MonoBehaviour
{
    private WeaponBase weapon;

    private void Awake()
    {
        weapon = GetComponent<WeaponBase>();
    }

    public void Equip(Item item)
    {
        foreach (IWeaponEffectInstance effect in item.EffectInstances)
        {
            if (effect is IRebindableWeaponEffect rebindable)
            {
                rebindable.Bind(weapon);
            }

            effect.OnEquip();
        }
    }

    public void Unequip(Item item)
    {
        foreach (IWeaponEffectInstance effect in item.EffectInstances)
        {
            effect.OnUnequip();
        }
    }
}
