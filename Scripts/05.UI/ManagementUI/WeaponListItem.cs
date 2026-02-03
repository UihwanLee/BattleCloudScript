using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class WeaponListItem : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private GameObject expGroup;
    [SerializeField] private Image expFill;

    [Header("Index")]
    [SerializeField] private int index;

    private Coroutine changeWeaponLocaleCoroutine;
    private WeaponSlot currentWeaponSlot;

    private void Reset()
    {
        weaponIcon = transform.FindChild<Image>("Weapon Icon");
        levelText = transform.FindChild<TextMeshProUGUI>("WeaponLevel Text");
        weaponName = transform.FindChild<TextMeshProUGUI>("WeaponName Text");
        expFill = transform.FindChild<Image>("WeaponExp Fill");
    }

    public void Initialize(WeaponSlot weapon)
    {
        if (weapon.WeaponBase == null || weapon.Item == null)
        {
            weaponIcon.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);
            weaponName.gameObject.SetActive(false);
            expGroup.gameObject.SetActive(false);
            expFill.gameObject.SetActive(false);
        }
        else
        {
            OnChangeLocale();
            weaponIcon.sprite = weapon.Item.GetIcon();
            levelText.text = $"Lv.{weapon.WeaponBase.Condition.Lv.Value}";
            expFill.fillAmount = (weapon.WeaponBase.Condition.Exp.Value / weapon.WeaponBase.Condition.MaxExp.Value);

            weaponIcon.gameObject.SetActive(true);
            levelText.gameObject.SetActive(true);
            weaponName.gameObject.SetActive(true);
            expGroup.gameObject.SetActive(true);
            expFill.gameObject.SetActive(true);
        }

        currentWeaponSlot = weapon;
    }

    private void OnChangeLocale()
    {
        if (changeWeaponLocaleCoroutine != null)
            StopCoroutine(changeWeaponLocaleCoroutine);

        changeWeaponLocaleCoroutine = StartCoroutine(ChangeLocaleWeaponRoutine());
    }

    private IEnumerator ChangeLocaleWeaponRoutine()
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("WeaponTable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(currentWeaponSlot.Item.GetName())?.GetLocalizedString();
            weaponName.text = text;
        }
    }

    public void OnWeaponClickEvent()
    {
        UIManager.Instance.ManagementUI.DetailView.SetUI(index);
    }
}
