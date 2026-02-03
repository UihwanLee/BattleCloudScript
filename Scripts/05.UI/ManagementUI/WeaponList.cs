using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponListItem[] weaponListItems = new WeaponListItem[Define.MAX_WEAPONCOUNT];

    private void Reset()
    {
        weaponListItems[0] = transform.FindChild<WeaponListItem>("Weapon List Item 0");
        weaponListItems[1] = transform.FindChild<WeaponListItem>("Weapon List Item 1");
        weaponListItems[2] = transform.FindChild<WeaponListItem>("Weapon List Item 2");
        weaponListItems[3] = transform.FindChild<WeaponListItem>("Weapon List Item 3");
        weaponListItems[4] = transform.FindChild<WeaponListItem>("Weapon List Item 4");
        weaponListItems[5] = transform.FindChild<WeaponListItem>("Weapon List Item 5");
    }

    private void OnEnable()
    {
        ShowWeaponList();
    }

    public void ShowWeaponList()
    {
        List<WeaponSlot> weapons = GameManager.Instance.Player.PlayerSlotUI.WeaponSlots;

        for (int i = 0; i < Define.MAX_WEAPONCOUNT; i++)
        {
            weaponListItems[i].Initialize(weapons[i]);
        }
    }
}
