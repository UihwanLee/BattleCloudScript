using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using WhereAreYouLookinAt.Enum;

public class WeaponStatusUI : MonoBehaviour
{
    [Header("Label Texts")]
    [SerializeField] private TextMeshProUGUI weaponDamageLabel;
    [SerializeField] private TextMeshProUGUI itemDamageLabel;
    [SerializeField] private TextMeshProUGUI damageLabel;
    [SerializeField] private TextMeshProUGUI rangeLabel;
    [SerializeField] private TextMeshProUGUI knockbackLabel;
    [SerializeField] private TextMeshProUGUI moveSpeedLabel;
    [SerializeField] private TextMeshProUGUI attackIntervalLabel;

    [Header("Status Texts")]
    [SerializeField] private TextMeshProUGUI weaponDamageText;
    [SerializeField] private TextMeshProUGUI itemDamageText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI knockbackText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI attackIntervalText;

    private Coroutine changeLocaleCoroutine;
    private WeaponSlot currentWeapon;

    private void Reset()
    {
        weaponDamageLabel = transform.FindChild<TextMeshProUGUI>("Advisor Damage Label");
        itemDamageLabel = transform.FindChild<TextMeshProUGUI>("Advice Damage Label");
        damageLabel = transform.FindChild<TextMeshProUGUI>("Damage Label");
        rangeLabel = transform.FindChild<TextMeshProUGUI>("Range Label");
        knockbackLabel = transform.FindChild<TextMeshProUGUI>("Knockback Label");
        moveSpeedLabel = transform.FindChild<TextMeshProUGUI>("MoveSpeed Label");
        attackIntervalLabel = transform.FindChild<TextMeshProUGUI>("AttackInterval Label");

        weaponDamageText = transform.FindChild<TextMeshProUGUI>("Advisor Damage Text");
        itemDamageText = transform.FindChild<TextMeshProUGUI>("Advice Damage Text");
        damageText = transform.FindChild<TextMeshProUGUI>("Damage Text");
        rangeText = transform.FindChild<TextMeshProUGUI>("Range Text");
        knockbackText = transform.FindChild<TextMeshProUGUI>("Knockback Text");
        moveSpeedText = transform.FindChild<TextMeshProUGUI>("MoveSpeed Text");
        attackIntervalText = transform.FindChild<TextMeshProUGUI>("AttackInterval Text");
    }

    private void OnEnable()
    {
        EventBus.Subscribe(AttributeType.Damage, UpdateUI);
        EventBus.Subscribe(AttributeType.Range, UpdateUI);
        EventBus.Subscribe(AttributeType.KnockBack, UpdateUI);
        EventBus.Subscribe(AttributeType.MoveSpeed, UpdateUI);
        EventBus.Subscribe(AttributeType.AttackInterval, UpdateUI);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe(AttributeType.Damage, UpdateUI);
        EventBus.UnSubscribe(AttributeType.Range, UpdateUI);
        EventBus.UnSubscribe(AttributeType.KnockBack, UpdateUI);
        EventBus.UnSubscribe(AttributeType.MoveSpeed, UpdateUI);
        EventBus.UnSubscribe(AttributeType.AttackInterval, UpdateUI);
    }

    public void SetUI(WeaponSlot weapon)
    {
        OnChangeLabelLocale();

        ClearUI();

        currentWeapon = weapon;

        UpdateUI(0);
        
        SetUIEnabled(true);
    }

    public void UpdateUI(float value)
    {
        ClearUI();

        if (currentWeapon.WeaponBase == null)
            return;

        weaponDamageText.text = ((int)currentWeapon.WeaponBase.Stat.Attack.BaseValue).ToString();

        float multiplier = (currentWeapon.WeaponBase.Stat.Attack.Multiplier <= 1f) ? 0f : currentWeapon.WeaponBase.Stat.Attack.Multiplier - 1f;

        itemDamageText.text = ((int)(currentWeapon.WeaponBase.Stat.Attack.Additive +
                                    multiplier * currentWeapon.WeaponBase.Stat.Attack.BaseValue)).ToString();
        damageText.text = ((int)currentWeapon.WeaponBase.Stat.Damage).ToString();
        rangeText.text = ((int)currentWeapon.WeaponBase.Stat.Range.Value).ToString();
        knockbackText.text = ((int)currentWeapon.WeaponBase.Stat.KnockBack.Value).ToString();
        moveSpeedText.text = ((int)currentWeapon.WeaponBase.Stat.MoveSpeed.Value).ToString();
        attackIntervalText.text = currentWeapon.WeaponBase.Stat.AttackInterval.Value.ToString("F2");
    }

    public void ClearUI()
    {
        weaponDamageText.text = string.Empty;
        itemDamageText.text = string.Empty;
        damageText.text = string.Empty;
        rangeText.text = string.Empty;
        knockbackText.text = string.Empty;
        moveSpeedText.text = string.Empty;
        attackIntervalText.text = string.Empty;
    }

    /// <summary>
    /// isEnabled 값에 따라 UI 활성화
    /// </summary>
    /// <param name="isEnabled"></param>
    public void SetUIEnabled(bool isEnabled)
    {
        weaponDamageText.gameObject.SetActive(isEnabled);
        itemDamageText.gameObject.SetActive(isEnabled);
        damageText.gameObject.SetActive(isEnabled);
        rangeText.gameObject.SetActive(isEnabled);
        knockbackText.gameObject.SetActive(isEnabled);
        moveSpeedText.gameObject.SetActive(isEnabled);
        attackIntervalText.gameObject.SetActive(isEnabled);

        if (weaponDamageLabel == null)
            weaponDamageLabel = transform.FindChild<TextMeshProUGUI>("Advisor Damage Label");
        if (weaponDamageLabel == null)
            weaponDamageLabel = GameObject.Find("Advisor Damage Label").GetComponent<TextMeshProUGUI>();
        weaponDamageLabel.gameObject.SetActive(isEnabled);
        itemDamageLabel.gameObject.SetActive(isEnabled);
        damageLabel.gameObject.SetActive(isEnabled);
        rangeLabel.gameObject.SetActive(isEnabled);
        knockbackLabel.gameObject.SetActive(isEnabled);
        moveSpeedLabel.gameObject.SetActive(isEnabled);
        attackIntervalLabel.gameObject.SetActive(isEnabled);
    }

    private void OnChangeLabelLocale()
    {
        if (changeLocaleCoroutine != null)
            StopCoroutine(changeLocaleCoroutine);

        changeLocaleCoroutine = StartCoroutine(ChangeLabelLocaleRoutine());
    }

    private IEnumerator ChangeLabelLocaleRoutine()
    {
        // label locale
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("StatusTable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry("Status_WeaponDamage").GetLocalizedString();
            if(weaponDamageLabel == null)
                weaponDamageLabel = transform.FindChild<TextMeshProUGUI>("Advisor Damage Label");
            if (weaponDamageLabel == null)
                weaponDamageLabel = GameObject.Find("Advisor Damage Label").GetComponent<TextMeshProUGUI>();
            weaponDamageLabel.text = text;

            text = table.GetEntry("Status_ItemDamage").GetLocalizedString();
            itemDamageLabel.text = text;

            text = table.GetEntry("Status_Damage").GetLocalizedString();
            damageLabel.text = text;

            text = table.GetEntry("Status_Range").GetLocalizedString();
            rangeLabel.text = text;

            text = table.GetEntry("Status_Knockback").GetLocalizedString();
            knockbackLabel.text = text;

            text = table.GetEntry("Status_MoveSpeed").GetLocalizedString();
            moveSpeedLabel.text = text;

            text = table.GetEntry("Status_AttackInterval").GetLocalizedString();
            attackIntervalLabel.text = text;
        }
    }
}
