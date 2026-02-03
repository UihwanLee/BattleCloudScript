using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DPSInfo
{
    public Image dpsWeapon;
    public TextMeshProUGUI dpsTxt;
    public float totalDamage = 0;
}

public class DPSManager : MonoBehaviour
{
    [Header("DPS UI")]
    [SerializeField] List<DPSInfo> dPSInfos = new List<DPSInfo>();

    private List<WeaponSlot> weaponSlots = new List<WeaponSlot>();

    private void OnEnable()
    {
        EventBus.OnTakeDamageByWeapon += OnTakeDamageByWeapon;
    }

    private void OnDisable()
    {
        EventBus.OnTakeDamageByWeapon -= OnTakeDamageByWeapon;
    }

    private IEnumerator Start()
    {
        GameManager.Instance.DPSManager = this;
        yield return null;
        UpdateUI();
    }

    public void UpdateUI()
    {
        weaponSlots = GameManager.Instance.Player.PlayerSlotUI.WeaponSlots;

        for(int i=0; i< weaponSlots.Count; i++)
        {
            dPSInfos[i].totalDamage = 0f;
            dPSInfos[i].dpsTxt.text = "0";
            if (weaponSlots[i].Item != null)
            {
                dPSInfos[i].dpsWeapon.gameObject.SetActive(true);
                dPSInfos[i].dpsTxt.gameObject.SetActive(true);
                dPSInfos[i].dpsWeapon.sprite = weaponSlots[i].Item.GetIcon();
            }
            else
            {
                dPSInfos[i].dpsWeapon.gameObject.SetActive(false);
                dPSInfos[i].dpsTxt.gameObject.SetActive(false);
            }
        }
    }

    public void OnTakeDamageByWeapon(WeaponBase weapon, float damage)
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i].WeaponBase == weapon)
            {
                dPSInfos[i].totalDamage += damage;
                dPSInfos[i].dpsTxt.text = ((int)dPSInfos[i].totalDamage).ToString();
            }
        }
    }
}
