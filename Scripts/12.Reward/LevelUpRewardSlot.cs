using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class LevelUpRewardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("컴포넌트")]
    [SerializeField] private Image rewardSlotImg;
    [SerializeField] private TextMeshProUGUI rewardName;
    [SerializeField] private TextMeshProUGUI rewardDesc;
    [SerializeField] private Image rewardHighlight;
    [SerializeField] private Image rewardEnhancmentIcon;
    [SerializeField] private Image rewardItemIcon;
    [SerializeField] public Button rewardSelectBtn;

    [Header("윈도우 UI")]
    [SerializeField] private GameObject window;
    [SerializeField] private GameObject windowItem;
    [SerializeField] private GameObject windowEnhancement;

    private LevelUpRewardEnhancementData data;

    private List<ISlotItem> items = new List<ISlotItem>();

    private EnhancementEffect effectType;

    private string rewardNameKey;
    private string rewardDescKey;

    private LevelUpRewardUI levelUpRewardUIManager;

    private bool isLocalizationDone = false;

    private void Reset()
    {
        rewardSlotImg = GetComponent<Image>();

        rewardName = transform.FindChild<TextMeshProUGUI>("RewardName");
        rewardDesc = transform.FindChild<TextMeshProUGUI>("RewardDesc");
        rewardSelectBtn = GetComponent<Button>();

        rewardHighlight = transform.FindChild<Image>("BG_Highlight");
        rewardEnhancmentIcon = transform.FindChild<Image>("RewardEnhancementIcon");
        rewardItemIcon = transform.FindChild<Image>("RewardItemIcon");

        windowItem = GameObject.Find("Winodw_Item");
        windowEnhancement = GameObject.Find("Winodw_Enhancement");
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangeLocale;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLocale;
    }

    public void Set(LevelUpRewardUI levelUpRewardUI, LevelUpRewardEnhancementData data)
    {
        isLocalizationDone = false;

        levelUpRewardUIManager = levelUpRewardUI;

        items.Clear();
        this.data = data;

        rewardName.gameObject.SetActive(true);

        effectType = GetEffectType(data.EFFECT);

        // 슬롯 이미지 적용
        switch(data.TIER)
        {
            case "TIER1":
                rewardSlotImg.sprite = levelUpRewardUIManager.Tier1Sprite;
                break;
            case "TIER2":
                rewardSlotImg.sprite = levelUpRewardUIManager.Tier2Sprite;
                break;
            case "TIER3":
                rewardSlotImg.sprite = levelUpRewardUIManager.Tier3Sprite;
                break;
            case "TIER4":
                rewardSlotImg.sprite = levelUpRewardUIManager.Tier4Sprite;
                break;
            default:
                rewardSlotImg.sprite = levelUpRewardUIManager.Tier1Sprite;
                break;
        }

        window.SetActive(true);
        if (effectType == EnhancementEffect.ADD_ITEM)
        {
            windowItem.SetActive(true);
            windowEnhancement.SetActive(false);
        }
        else
        {
            windowItem.SetActive(false);
            windowEnhancement.SetActive(true);
        }


        // 만약 보따리를 뽑았다면 랜덤으로 갱신
        if (effectType == EnhancementEffect.ADD_ITEM)
        {
            // VALUE에 따라서 설정
            switch(data.VALUE)
            {
                case 1:
                    rewardItemIcon.sprite = DataManager.BaseDropItemSprite;
                    break;
                case 2:
                    rewardItemIcon.sprite = DataManager.BaseDropItem2Sprite;
                    break;
                case 3:
                    rewardItemIcon.sprite = DataManager.BaseDropItem3Sprite;
                    break;
                default:
                    rewardItemIcon.sprite = DataManager.BaseDropItemSprite;
                    break;
            }

            // 아이템 리스트 비워두기
            items.Clear();

            // TIER별 아이템 증가
            for (int i = 0; i < data.VALUE; i++)
            {
                Item item = GameManager.Instance.ItemDataTable.GetRandomItemByTier();
                items.Add(item);
            }
        }
        else
        {
            rewardEnhancmentIcon.sprite = DataManager.GetImage(data.IMAGE);
        }

        rewardNameKey = data.NAME;
        rewardDescKey = data.DESC;

        Locale currentLocale = LocalizationSettings.SelectedLocale;
        OnChangeLocale(currentLocale);
    }

    public void ResetSlot()
    {
        window.SetActive(false);
        rewardName.gameObject.SetActive(false);
        windowEnhancement.gameObject.SetActive(false);
        windowItem.gameObject.SetActive(false);
    }

    public void OnChangeLocale(Locale locale)
    {
        StartCoroutine(ChangeLocaleRoutine(locale));
    }

    private IEnumerator ChangeLocaleRoutine(Locale locale)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync("EnhancementRewardTable");
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(rewardNameKey)?.GetLocalizedString();
            rewardName.text = text;

            text = table.GetEntry(rewardDescKey)
                    ?.GetLocalizedString(new { value = data.VALUE });
            rewardDesc.text = text;

            yield return null;

            isLocalizationDone = true;
        }
    }

    private EnhancementEffect GetEffectType(string value)
    {
        if (Enum.TryParse<EnhancementEffect>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 EnhancementEffect가 지정되지 않았습니다");
        return EnhancementEffect.UNDEF;
    }

    #region 마우스 인터페이스 구현

    public void OnPointerEnter(PointerEventData eventData)
    {
        rewardHighlight.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rewardHighlight.gameObject.SetActive(false);
    }

    #endregion

    #region 프로퍼티

    public LevelUpRewardEnhancementData Data { get { return data; } }
    public List<ISlotItem> Items { get { return items; } }
    public EnhancementEffect Type { get { return effectType; } }
    public bool IsLocalizationDone { get { return isLocalizationDone; } }

    #endregion
}
