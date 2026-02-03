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
using static UnityEditor.Progress;

[System.Serializable]
public class ReinforcementInfo
{
    public GameObject window;
    public TextMeshProUGUI lvTxt;
    public TextMeshProUGUI desc;
}

public class ReinforcementView : MonoBehaviour
{
    [Header("강화할 슬롯")]
    [SerializeField] private ReinforcementSlot reinforcementSlot;

    [Header("슬롯 View 컴포넌트")]
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI descTxt;

    [Header("강화 View 컴포넌트")]
    [SerializeField] private GameObject reinforcementWindow;
    [SerializeField] private Image reinforcementIcon;
    [SerializeField] private TextMeshProUGUI reinforcementCostTxt;
    [SerializeField] private ReinforcementInfo prevReinforcementView;
    [SerializeField] private ReinforcementInfo nextReinforcementView;
    [SerializeField] private Button reinforcementBtn;

    [Header("강화 연출")]
    [SerializeField] private Image reinforcementBackground;

    private float currentReinfocementValue = 0f;
    private int currentWeaponIndex = 0;
    private Slot currentSlot;
    private ISlotItem currentItem;

    private GameRule gameRule;
    private Player player;

    private void Start()
    {
        reinforcementWindow.SetActive(false);
        reinforcementSlot.SetReinforcementView(this);
    }

    public void OnClickReinforcementNPC()
    {
        reinforcementWindow.SetActive(true);
        ResetReinforcementView();
    }

    public void CloseReinforcementUI()
    {
        reinforcementWindow.SetActive(false);
        ResetReinforcementView();
    }

    private void ResetReinforcementView()
    {
        iconImg.gameObject.SetActive(false);
        nameTxt.text = string.Empty;
        descTxt.text = string.Empty;
        prevReinforcementView.lvTxt.text = string.Empty;
        prevReinforcementView.desc.text = string.Empty;
        reinforcementBtn.onClick.RemoveAllListeners();
        reinforcementBtn.gameObject.SetActive(false);
        nextReinforcementView.window.SetActive(false);
        reinforcementIcon.gameObject.SetActive(false);
        reinforcementCostTxt.text = string.Empty;

        currentSlot = null;
    }

    public void SetView(ISlotItem item)
    {
        if (item == null) return;

        currentItem = item;
        UpdateUI();
    }

    private void UpdateUI()
    {
        float lv = currentItem.GetLv().Value;
        float reinforcemntCost = GameManager.Instance.GameRule.GetItemReinforcementCost(currentItem.GetLv());

        iconImg.gameObject.SetActive(true);

        // 무기인지 아이템 인지 체크
        if (currentItem.GetType() == WhereAreYouLookinAt.Enum.DropItemType.Weapon)
        {
            // 최대 레벨인지 체크
            if (lv >= Define.MAX_WEAPON_LV)
            {
                prevReinforcementView.lvTxt.text = Define.MAX_LV_LABEL;
                nextReinforcementView.window.SetActive(false);
                reinforcementBtn.gameObject.SetActive(false);
            }
            else
            {
                // 강화 비용 가져오기
                if (currentItem.GetType() == WhereAreYouLookinAt.Enum.DropItemType.Weapon)
                {
                    currentReinfocementValue = GameManager.Instance.GameRule.GetWeaponReinforcementCostByLv(currentItem.GetLv());
                }

                prevReinforcementView.lvTxt.text = $"Lv.{lv}";
                nextReinforcementView.window.SetActive(true);
                nextReinforcementView.lvTxt.text = $"Lv.{lv+1}";
                reinforcementBtn.gameObject.SetActive(true);
                reinforcementBtn.onClick.RemoveAllListeners();
                reinforcementBtn.onClick.AddListener(() => TryReinforement(currentItem, currentReinfocementValue));
                reinforcementIcon.gameObject.SetActive(true);
                reinforcementCostTxt.text = currentReinfocementValue.ToString();

                // Reinforcement Current Stat
                if (currentSlot != null)
                {
                    if (currentSlot is WeaponSlot weaponSlot)
                    {
                        float nextValue = GameManager.Instance.GameRule.GetWeaponLvIncreaseDamageStatByType(weaponSlot.WeaponBase.Condition.WeaponType, currentItem.GetLv().Value + 1);
                        float nextInterval = GameManager.Instance.GameRule.GetWeaponLvIncreaseAttackIntervalStatByType(weaponSlot.WeaponBase.Condition.WeaponType, currentItem.GetLv().Value + 1);

                        StartCoroutine(ChangeLocaleWeaponNextReinforcementRoutine(nextReinforcementView.desc, nextValue,
                                nextInterval));
                    }
                    else if(currentSlot is TemporarySlot temporarySlot)
                    {
                        if(currentItem is Weapon weapon)
                        {
                            float nextValue = GameManager.Instance.GameRule.GetWeaponLvIncreaseDamageStatByType(weapon.Type, currentItem.GetLv().Value + 1);
                            float nextInterval = GameManager.Instance.GameRule.GetWeaponLvIncreaseAttackIntervalStatByType(weapon.Type, currentItem.GetLv().Value + 1);

                            StartCoroutine(ChangeLocaleWeaponNextReinforcementRoutine(nextReinforcementView.desc, nextValue,
                                nextInterval));
                        }
                    }
                }
            }

            // Reinforcement 설명 창 Desc
            StartCoroutine(ChangeLocaleWeaponRoutine("WeaponTable", currentSlot.Item.GetName(), currentSlot.Item.GetDesc(),
                nameTxt, descTxt));

            // Reinforcement Current Stat
            if (currentSlot != null)
            {
                if(currentSlot is WeaponSlot weaponSlot)
                {
                    StartCoroutine(ChangeLocaleWeaponReinforcementRoutine(prevReinforcementView.desc, 
                        weaponSlot.WeaponBase.Stat.Attack.BaseValue,
                        weaponSlot.WeaponBase.Stat.AttackInterval.BaseValue));
                }
                else if (currentSlot is TemporarySlot temporarySlot)
                {
                    if (currentItem is Weapon weapon)
                    {
                        StartCoroutine(ChangeLocaleWeaponReinforcementRoutine(prevReinforcementView.desc,
                            weapon.Damage,
                            weapon.AttackInterval));
                    }
                }
            }
        }
        else
        {
            string tableName = (currentItem.GetType() == DropItemType.Item) ? "ItemTable" : "ExclusiveItemTable";

            // 최대 레벨인지 체크
            if (lv >= Define.MAX_ITEM_LV)
            {
                prevReinforcementView.lvTxt.text = Define.MAX_LV_LABEL;
                nextReinforcementView.window.SetActive(false);
                reinforcementBtn.gameObject.SetActive(false);
            }
            else
            {
                // 강화 비용 가져오기
                if (currentItem.GetType() == WhereAreYouLookinAt.Enum.DropItemType.Item || currentItem.GetType() == WhereAreYouLookinAt.Enum.DropItemType.ExclusiveItem)
                {
                    currentReinfocementValue = GameManager.Instance.GameRule.GetItemReinforcementCost(currentItem.GetLv());
                }

                prevReinforcementView.lvTxt.text = $"Lv.{lv}";
                nextReinforcementView.window.SetActive(true);
                nextReinforcementView.lvTxt.text = $"Lv.{lv + 1}";
                reinforcementBtn.gameObject.SetActive(true);
                reinforcementBtn.onClick.RemoveAllListeners();
                reinforcementBtn.onClick.AddListener(() => TryReinforement(currentItem, currentReinfocementValue));
                reinforcementIcon.gameObject.SetActive(true);

                reinforcementCostTxt.text = currentReinfocementValue.ToString();

                // Reinforcement Current Stat
                StartCoroutine(ChangeLocaleItemNextReinforcementRoutine(tableName, currentItem.GetDesc(), nextReinforcementView.desc,
                    currentItem.GetNextValueList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT),
                    currentItem.GetValueTypeList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT)));
            }

            // Reinforcement 설명 창 Desc
            StartCoroutine(ChangeLocaleItemRoutine(tableName, currentItem.GetName(), currentItem.GetDesc(),
                nameTxt, descTxt,
                currentItem.GetValueList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT),
                currentItem.GetValueTypeList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT)));

            // Reinforcement Current Stat
            StartCoroutine(ChangeLocaleItemReinforcementRoutine(tableName, currentItem.GetDesc(), prevReinforcementView.desc,
                currentItem.GetValueList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT),
                currentItem.GetValueTypeList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT)));
        }
    }


    private void TryReinforement(ISlotItem item, float reincementCost)
    {
        if (gameRule == null) gameRule = GameManager.Instance.GameRule;
        if (player == null) player = GameManager.Instance.Player;

        // 강화 비용 가져오기
        if (item.GetType() == WhereAreYouLookinAt.Enum.DropItemType.Weapon)
        {
            currentReinfocementValue = gameRule.GetWeaponReinforcementCostByLv(item.GetLv());
        }
        else
        {
            currentReinfocementValue = gameRule.GetItemReinforcementCost(item.GetLv());
        }

        //골드 확인
        if (currentReinfocementValue > player.Condition.Gold.Value)
        {
            Debug.Log($"강화 비용이 부족합니다 [강화비용: {currentReinfocementValue} / 현재골드 {player.Condition.Gold.Value}");
            AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.impossible);
            return;
        }


        Reinforement(item);
    }

    private void Reinforement(ISlotItem item)
    {
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UIItemUpgrade);
        // 골드 소모
        player.Condition.Sub(WhereAreYouLookinAt.Enum.AttributeType.Gold, currentReinfocementValue);

        // 강화 비용 누적
        item.AddAdditionalPrice(currentReinfocementValue);

        Debug.Log("레벨업");

        // 레벨업
        if (currentSlot is ItemSlot itemSlot)
        {
            item.AddLv(1, true);
        }
        else
        {
            item.AddLv(1, false);
        }

        // 조언자 강화인 경우에는 다시 뺏다 키기
        if (item.GetType() == WhereAreYouLookinAt.Enum.DropItemType.Weapon)
        {
            if(currentSlot is WeaponSlot weaponSlot)
            {
                weaponSlot.DeleteItem(item, weaponSlot.Index);
                weaponSlot.AddItem(item, weaponSlot.Index, false);
            }
        }

        // 플레이어 슬롯 쪽 UI 업데이트
        player.PlayerSlotUI.UpdateLeveUpUI();

        // 슬롯 업데이트
        if(currentSlot  != null)
        {
            currentSlot.UpdateLv(currentSlot.Item.GetLv());
            currentSlot.UpdateLvUI();
        }

        UpdateUI();

        //강화 성공 연출
        StartCoroutine(PlayReinforcementEffect());
    }

    private IEnumerator PlayReinforcementEffect()
    {
        if (reinforcementBackground == null)
            yield break;

        Color color = reinforcementBackground.color;

        float duration = 0.35f;
        float maxAlpha = 0.8f;
        float baseAlpha = color.a;

        // 페이드인
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(baseAlpha, maxAlpha, t / duration);
            reinforcementBackground.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }

        // 페이드아웃
        t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(maxAlpha, baseAlpha, t / duration);
            reinforcementBackground.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }
    }

    #region Locale
    public void OnChangeLocale(Locale locale)
    {
        SetView(currentItem);
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

    private IEnumerator ChangeLocaleWeaponReinforcementRoutine(TextMeshProUGUI descTxt, float damge, float attackInterval)
    {
        // label locale
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("StatusTable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string option1 = table.GetEntry("Status_WeaponDamage")?.GetLocalizedString();
            string option2 = table.GetEntry("Status_AttackInterval")?.GetLocalizedString();

            if (currentSlot != null)
            {
                string desc = $"{option1}: {damge}\n{option2}: {attackInterval}";
                descTxt.text = desc;
            }
        }
    }

    private IEnumerator ChangeLocaleWeaponNextReinforcementRoutine(TextMeshProUGUI descTxt, float damge, float attackInterval)
    {
        // label locale
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("StatusTable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string option1 = table.GetEntry("Status_Damage")?.GetLocalizedString();
            string option2 = table.GetEntry("Status_AttackInterval")?.GetLocalizedString();

            if (currentSlot != null)
            {
                string desc = $"{option1}: {damge}\n{option2}: {attackInterval}";
                descTxt.text = desc;
            }
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

    private IEnumerator ChangeLocaleItemReinforcementRoutine(string tableName, string descKey,
       TextMeshProUGUI descTxt, List<float> valueList, List<ItemApplyType> typeList)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string str;
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

    private IEnumerator ChangeLocaleItemNextReinforcementRoutine(string tableName, string descKey,
       TextMeshProUGUI descTxt, List<float> valueList, List<ItemApplyType> typeList)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string str;
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

    public Slot CurrentSlot { get { return currentSlot; } set { currentSlot = value; } }

    #endregion
}
