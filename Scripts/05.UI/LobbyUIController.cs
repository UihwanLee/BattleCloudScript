using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [Header("캐릭터 설명 UI")]
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtDesc;
    [SerializeField] private TMP_Text txtStat;

    [Header("캐릭터 슬롯")]
    [SerializeField] private Transform characterSlotRoot;
    [SerializeField] private LobbyCharacterSlot slotPrefab;

    [Header("중앙 캐릭터 표시")]
    [SerializeField] private Image selectedCharacterImage;

    [Header("기본 조언자 UI")]
    [SerializeField] private TMP_Text advisorNameText;
    [SerializeField] private TMP_Text advisorDescText;
    [SerializeField] private Image advisorIconImage;

    [Header("Start 버튼")]
    [SerializeField] private Button startButton;

    private readonly List<LobbyCharacterSlot> slots = new();

    private Coroutine changeWeaponLocaleCoroutine;
    private Coroutine changePlayerLocaleCoroutine;

    private void Start()
    {
        GenerateCharacterSlots();
        //캐릭터 선택시 활성
        UpdateStartButtonState();
    }

    // 캐릭터 슬롯 자동 생성
    private void GenerateCharacterSlots()
    {
        foreach (var pair in DataManager.PlayerTraitDict)
        {
            PlayerTraitData data = pair.Value;

            LobbyCharacterSlot slot = Instantiate(slotPrefab, characterSlotRoot);
            slot.SetData(data);

            // 버튼 클릭 연결
            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() =>
                {
                    LobbyManager.Instance.SelectCharacter(data.ID);
                    UpdateSelectedCharacter(data);
                    UpdateStartButtonState();
                });
            }

            slots.Add(slot);
        }

        // 기본 캐릭터 자동 선택
        SelectDefaultCharacter();
    }

    // 선택 캐릭터 UI 갱신
    public void UpdateSelectedCharacter(PlayerTraitData data)
    {
        UpdateDescription(data);
        UpdateSlotHighlight(data);
        UpdateCenterCharacter(data);
        UpdateStatText(data);
        UpdateAdvisorUI(data);
    }

    private void UpdateDescription(PlayerTraitData data)
    {
        OnChangeWeaponLocale("WeaponTable", data.NAME, data.DESC, txtName, txtDesc);
    }
    private void UpdateStatText(PlayerTraitData traitData)
    {
        PlayerStatData statData = DataManager.Get<PlayerStatData>(traitData.ID_STAT);

        if (statData == null)
        {
            txtStat.text = "STAT DATA NOT FOUND";
            return;
        }

        txtStat.text =
            $"지휘력 : {statData.COMMAND}\n" +
            $"이동속도 : {statData.MOVE_SPEED}\n" +
            $"회피 : {statData.EVASION}%\n" +
            $"방어 : {statData.DEFENSE}%\n" +
            $"행운 : {statData.LUCK}";
    }
    private void UpdateSlotHighlight(PlayerTraitData selectedData)
    {
        foreach (var slot in slots)
        {
            slot.SetSelected(slot.Data == selectedData);
        }
    }

    private void UpdateCenterCharacter(PlayerTraitData data)
    {
        selectedCharacterImage.sprite = DataManager.GetImage(data.Image);
        OnChangePlayerLocale("PlayerTable", data.NAME, data.DESC, txtName, txtDesc);
    }
    private void UpdateAdvisorUI(PlayerTraitData traitData)
    {
        WeaponTraitData weaponData =
        DataManager.Get<WeaponTraitData>(traitData.ID_WEAPON);

        if (weaponData == null)
        {
            advisorNameText.text = "-";
            advisorDescText.text = "";
            advisorIconImage.sprite = null;
            return;
        }

        OnChangeWeaponLocale("WeaponTable", weaponData.NAME, weaponData.DESC, advisorNameText, advisorDescText);

        // 임시 수정: Prefab에서 Sprite 꺼내기
        GameObject prefab = DataManager.GetPrefab(weaponData.PREFAB);
        if (prefab == null)
        {
            Debug.LogError($"[Lobby] Weapon Prefab not found : {weaponData.PREFAB}");
            advisorIconImage.sprite = null;
            return;
        }

        SpriteRenderer sr = prefab.GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError($"[Lobby] SpriteRenderer not found in prefab : {weaponData.PREFAB}");
            advisorIconImage.sprite = null;
            return;
        }

        advisorIconImage.sprite = sr.sprite;
    }
    // 기본캐릭터 자동 선택
    private void SelectDefaultCharacter()
    {
        if (slots.Count == 0)
            return;

        LobbyCharacterSlot firstSlot = slots[0];
        PlayerTraitData data = firstSlot.Data;

        LobbyManager.Instance.SelectCharacter(data.ID);
        UpdateSelectedCharacter(data);
        UpdateStartButtonState();
    }
    private void UpdateStartButtonState()
    {
        bool canStart = LobbyManager.Instance.SelectedCharacterId >= 0;
        startButton.interactable = canStart;
    }

    #region Locale
    private void OnChangePlayerLocale(string tableName, string nameKey, string descKey,
        TMP_Text nameTxt, TMP_Text descTxt)
    {
        if (changePlayerLocaleCoroutine != null)
            StopCoroutine(changePlayerLocaleCoroutine);

        changePlayerLocaleCoroutine = StartCoroutine(ChangeLocalePlayerRoutine(tableName, nameKey, descKey, nameTxt, descTxt));
    }
    private void OnChangeWeaponLocale(string tableName, string nameKey, string descKey,
        TMP_Text nameTxt, TMP_Text descTxt)
    {
        if (changeWeaponLocaleCoroutine != null)
            StopCoroutine(changeWeaponLocaleCoroutine);

        changeWeaponLocaleCoroutine = StartCoroutine(ChangeLocaleWeaponRoutine(tableName, nameKey, descKey, nameTxt, descTxt));
    }

    private IEnumerator ChangeLocalePlayerRoutine(string tableName, string nameKey, string descKey,
        TMP_Text nameTxt, TMP_Text descTxt)
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

    private IEnumerator ChangeLocaleWeaponRoutine(string tableName, string nameKey, string descKey,
        TMP_Text nameTxt, TMP_Text descTxt)
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
    #endregion
}

