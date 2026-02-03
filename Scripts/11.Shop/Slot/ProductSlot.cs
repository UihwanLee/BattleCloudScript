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
public class ProductSlotVisual
{
    public Tier tier;
    public Sprite backgroundSprite; 
}
public class ProductSlot : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private GameObject productWindow;
    [SerializeField] private Button productSelectBtn;
    [SerializeField] private Image productGoldIcon;
    [SerializeField] private TextMeshProUGUI productPriceTxt;
    [SerializeField] private TextMeshProUGUI productNameTxt;
    [SerializeField] private Image productIcon;
    [SerializeField] private TextMeshProUGUI productDescTxt;
    [SerializeField] private TextMeshProUGUI productTier;
    [SerializeField] private Image backgroundImage; //ProductWindow의 Image
    [SerializeField] private List<ProductSlotVisual> tierVisuals;
    [SerializeField] private CanvasGroup canvasGroup;

    private ISlotItem productItem;

    private float finalPrice;

    private string nameKey;
    private string descKey;
    private Coroutine changeItemLocalCoroutine;
    private string tableName;
    private List<float> valueList;
    private List<ItemApplyType> typeList;

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        productWindow = GameObject.Find("ProductWindow");
        productPriceTxt = transform.FindChild<TextMeshProUGUI>("ProductPrice");
        productNameTxt = transform.FindChild<TextMeshProUGUI>("ProductName");
        productDescTxt = transform.FindChild<TextMeshProUGUI>("ProductDesc");
        productIcon = transform.FindChild<Image>("ProductIcon");
        productTier = transform.FindChild<TextMeshProUGUI>("ProductTier");
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangeLocale;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLocale;
    }

    public void InitSlot()
    {
        productWindow.SetActive(false);
    }

    public void SetProduct(ISlotItem item)
    {
        productItem = item;
        nameKey = item.GetName();
        descKey = item.GetDesc();

        Tier tier = item.GetTier(); 
        ApplyTierVisual(tier);

        // 티어 정리
        productTier.text = item.GetTier().ToString();

        // 가격 계산
        float price = item.GetPrice();
        int day = GameManager.Instance.DayManager.CurrentDay;
        float rate = 0.1f;

        // 최종 가격은 반올림하여 설정
        finalPrice = Mathf.Round(price + day + (price * rate * day));

        productPriceTxt.text = finalPrice.ToString();
        productIcon.sprite = item.GetIcon();

        tableName = (item.GetType() == DropItemType.Weapon) ? Define.TABLE_WEAPON_NAME :
                (item.GetType() == DropItemType.ExclusiveItem) ? Define.TABLE_ITEM_EXCLUSIVE_NAME : Define.TABLE_ITEM_NAME;

        valueList = item.GetValueList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT);
        typeList = item.GetValueTypeList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT);

        Locale currentLocale = LocalizationSettings.SelectedLocale;
        OnChangeLocale(currentLocale);
    }

    public void RemoveGold()
    {
        productGoldIcon.gameObject.SetActive(false);
        productPriceTxt.text = string.Empty;
    }

    public void OnChangeLocale(Locale locale)
    {
        if (changeItemLocalCoroutine != null) StopCoroutine(changeItemLocalCoroutine);
        changeItemLocalCoroutine = StartCoroutine(ChangeLocaleItemRoutine(locale, tableName));
    }

    private IEnumerator ChangeLocaleItemRoutine(Locale locale, string Table)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(Table);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(nameKey)?.GetLocalizedString();
            productNameTxt.text = text;

            text = table.GetEntry(descKey)
                    ?.GetLocalizedString(new
                    {
                        value_01 = UIHelper.ColorTextWithType(valueList[0], typeList[0], UIHelper.ColorOrange),
                        value_02 = UIHelper.ColorTextWithType(valueList[1], typeList[1], UIHelper.ColorOrange),
                        value_03 = UIHelper.ColorTextWithType(valueList[2], typeList[2], UIHelper.ColorOrange),
                        value_04 = UIHelper.ColorTextWithType(valueList[3], typeList[3], UIHelper.ColorOrange),
                        value_05 = UIHelper.ColorTextWithType(valueList[4], typeList[4], UIHelper.ColorOrange),
                    });
            productDescTxt.text = text;
        }

        productWindow.SetActive(true);
    }
    private void ApplyTierVisual(Tier tier)
    {
        var visual = tierVisuals.Find(v => v.tier == tier);
        if (visual == null)
            return;

        backgroundImage.sprite = visual.backgroundSprite;
    }
    #region 프로퍼티

    public ISlotItem Item { get { return productItem; } }
    public Button SelectBtn { get { return productSelectBtn; } }
    public float FinalPrice { get { return finalPrice; } }
    public CanvasGroup CanvasGroup { get { return canvasGroup; } }

    #endregion
}
