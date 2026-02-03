using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

[System.Serializable]
public class DetailInfo
{
    public Image icon;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image levelContainer;
}

public class WeaponDescriptionUI : MonoBehaviour
{
    [Header("Weapon UI")]
    [SerializeField] private DetailInfo weaponInfo;

    [Header("Item UI")]
    [SerializeField] private List<DetailInfo> itemInfoList;

    [Header("Item Apply Scale")]
    [SerializeField] private TextMeshProUGUI itemApplyScaleTxt;

    [Header("Material")]
    [SerializeField] private List<Material> materials;

    private Coroutine changeLocaleCoroutine;
    private WeaponSlot currentWeaponSlot;

    private float currentReinfocementValue = 0f;
    private bool isWatchingDetailView = false;
    private int currentWeaponIndex = 0;

    private GameRule gameRule;
    private Player player;

    // 저장하고 있는 데이터

    private void Reset()
    {
        weaponInfo.icon = transform.FindChild<Image>("Selected Weapon Icon");
        weaponInfo.levelText = transform.FindChild<TextMeshProUGUI>("Level Text");
        weaponInfo.nameText = transform.FindChild<TextMeshProUGUI>("Name Text");
        weaponInfo.descriptionText = transform.FindChild<TextMeshProUGUI>("Description Text");
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangeLocale;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLocale;
    }

    /// <summary>
    /// WeaponSlot에 있는 Weapon의 설명 UI 표시 <br/>
    /// UI에 데이터를 입력하고 UI 활성화
    /// </summary>
    /// <param name="weapon"></param>
    public void SetUI(WeaponSlot weapon)
    {
        currentWeaponSlot = weapon;
        UpdateUI();
    }

    private void UpdateUI()
    {
        ClearUI();

        if (currentWeaponSlot == null) return;

        if (currentWeaponSlot.Item != null)
        {
            currentWeaponIndex = currentWeaponSlot.Index;
            float lv = currentWeaponSlot.Item.GetLv().Value;
            float reinforcemntCost = GameManager.Instance.GameRule.GetWeaponReinforcementCostByLv(currentWeaponSlot.Item.GetLv());
            weaponInfo.icon.sprite = currentWeaponSlot.Item.GetIcon();
            weaponInfo.icon.material = materials[(int)lv - 1];
            weaponInfo.icon.gameObject.SetActive(true);
            weaponInfo.levelText.text = (lv < Define.MAX_WEAPON_LV) ? $"Lv.{(int)lv}" : Define.MAX_LV_LABEL;
            weaponInfo.levelText.gameObject.SetActive(true);
            weaponInfo.levelContainer.color = currentWeaponSlot.Item.GetTierColor();
            weaponInfo.levelContainer.gameObject.SetActive(true);
            weaponInfo.descriptionText.gameObject.SetActive(true);
            weaponInfo.nameText.gameObject.SetActive(true);
            if(currentWeaponSlot.Item is Weapon weapon)
            {
                itemApplyScaleTxt.gameObject.SetActive(true);
                StartCoroutine(ChangeLocaleAdvisorScaleApply("UITable", "UI_Label_ItemScale", itemApplyScaleTxt, weapon.Scale * 100));
            }
            StartCoroutine(ChangeLocaleWeaponRoutine("WeaponTable", currentWeaponSlot.Item.GetName(), currentWeaponSlot.Item.GetDesc(),
                weaponInfo.nameText, weaponInfo.descriptionText));
        }

        for (int i = 0; i < currentWeaponSlot.ItemSlots.Count; i++)
        {
            int index = i;
            if (currentWeaponSlot.ItemSlots[i].Item != null)
            {
                SetItem(currentWeaponSlot.ItemSlots[i], index);
            }
        }
    }
    
    private void SetItem(ItemSlot item, int index)
    {
        float lv = item.Item.GetLv().Value;
        float reinforcemntCost = GameManager.Instance.GameRule.GetItemReinforcementCost(item.Item.GetLv());
        itemInfoList[index].icon.sprite = item.Item.GetIcon();
        itemInfoList[index].icon.gameObject.SetActive(true);
        itemInfoList[index].levelText.text = (lv < Define.MAX_ITEM_LV) ? $"Lv.{(int)lv}" : Define.MAX_LV_LABEL;
        itemInfoList[index].levelText.gameObject.SetActive(true);
        itemInfoList[index].levelContainer.color = item.Item.GetTierColor();
        itemInfoList[index].levelContainer.gameObject.SetActive(true);
        itemInfoList[index].descriptionText.gameObject.SetActive(true);
        itemInfoList[index].nameText.gameObject.SetActive(true);
        string tableName = (item.Item.GetType() == DropItemType.Item) ? "ItemTable" : "ExclusiveItemTable";
        StartCoroutine(ChangeLocaleItemRoutine(tableName, item.Item.GetName(), item.Item.GetDesc(),
            itemInfoList[index].nameText, itemInfoList[index].descriptionText, 
            item.Item.GetValueList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT),
            item.Item.GetValueTypeList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT)));
    }

    public void ClearUI()
    {
        itemApplyScaleTxt.text = string.Empty;
        itemApplyScaleTxt.gameObject.SetActive(false);

        ClearUIDetailView(weaponInfo);

        foreach(DetailInfo detailInfo in itemInfoList)
        {
            ClearUIDetailView(detailInfo);
        }
    }

    public void ClearUIDetailView(DetailInfo info)
    {
        info.icon.sprite = null;
        info.icon.gameObject.SetActive(false);
        info.levelText.text = string.Empty;
        info.levelText.gameObject.SetActive(false);
        info.nameText.gameObject.SetActive(false);
        info.nameText.text = string.Empty;
        info.descriptionText.gameObject.SetActive(false);
        info.descriptionText.text = string.Empty;
        info.levelContainer.gameObject.SetActive(false);
    }

    /// <summary>
    /// isEnabled 값에 따라 UI 활성화
    /// </summary>
    /// <param name="isEnabled"></param>
    public void SetUIEnabled(bool isEnabled)
    {
        weaponInfo.icon.gameObject.SetActive(isEnabled);
        weaponInfo.levelText.gameObject.SetActive(isEnabled);
        weaponInfo.nameText.gameObject.SetActive(isEnabled);
        weaponInfo.descriptionText.gameObject.SetActive(isEnabled);
    }

    #region Locale

    public void OnChangeLocale(Locale locale)
    {
        SetUI(currentWeaponSlot);
    }

    private IEnumerator ChangeLocaleAdvisorScaleApply(string tableName, string key, TextMeshProUGUI descTxt,float scale)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(key)
                    ?.GetLocalizedString(new
                    {
                        value_01 = UIHelper.ColorTextWithType(scale, ItemApplyType.PERCENT, UIHelper.ColorOrange),
                    });
            descTxt.text = text;
        }
    }

    private IEnumerator ChangeLocaleWeaponRoutine(string tableName, string nameKey, string descKey,
        TextMeshProUGUI nameTxt, TextMeshProUGUI descTxt)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string str = table.GetEntry(nameKey)?.GetLocalizedString();
            nameTxt.text = str;

            str = table.GetEntry(descKey)?.GetLocalizedString();
            descTxt.text = str;
        }
    }

    private IEnumerator ChangeLocaleItemRoutine(string tableName, string nameKey, string descKey, 
        TextMeshProUGUI nameTxt, TextMeshProUGUI descTxt, List<float> valueList, List<ItemApplyType> typeList)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string str = table.GetEntry(nameKey)?.GetLocalizedString();
            nameTxt.text = str;

            str = table.GetEntry(descKey)
                    ?.GetLocalizedString(new
                    {
                        value_01 = UIHelper.ColorTextWithType(valueList[0], typeList[0], UIHelper.ColorOrange),
                        value_02 = UIHelper.ColorTextWithType(valueList[1], typeList[1], UIHelper.ColorOrange),
                        value_03 = UIHelper.ColorTextWithType(valueList[2], typeList[2], UIHelper.ColorOrange),
                        value_04 = UIHelper.ColorTextWithType(valueList[3], typeList[3], UIHelper.ColorOrange),
                        value_05 = UIHelper.ColorTextWithType(valueList[4], typeList[4], UIHelper.ColorOrange),
                    });
            descTxt.text = str;
        }
    }
    #endregion

    #region 프로퍼티

    public int CurrentWeaponIndex { get { return currentWeaponIndex; } }

    #endregion
}
