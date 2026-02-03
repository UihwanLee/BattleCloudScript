using Org.BouncyCastle.Asn1.Tsp;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;
using WhereAreYouLookinAt.Enum;

// LogInfoUI 클래스
// 이벤트 형식으로 UI를 처리한다.
public class LogInfoUI : MonoBehaviour
{
    [Header("Window 컴포넌트")]
    [SerializeField] private CanvasGroup logCanvasGroup;
    [SerializeField] private GameObject weaponCountWindow;

    [Header("Log 컴포넌트")]
    [SerializeField] private GameObject itemInfo;
    [SerializeField] private GameObject statInfo;
    [SerializeField] private TextMeshProUGUI statNameTxt;
    [SerializeField] private TextMeshProUGUI itemNameTxt;
    [SerializeField] private TextMeshProUGUI itemDescTxt;
    [SerializeField] private TextMeshProUGUI itemLvTxt;
    [SerializeField] private TextMeshProUGUI itemPirceTxt;

    [Header("WeaponCount 컴포넌트")]
    [SerializeField] private TextMeshProUGUI weaponCountTxt;

    [Header("UI 애니메이션 데이터")]
    [Header("Log UI 지속 시간")]
    [SerializeField] private float logUiDuration = 0.5f;
    [Header("Log UI Fade 연출 시간")]
    [SerializeField] private float fadeDuration = 3f;

    private WaitForSeconds forSeconds;
    private Coroutine logUIFadeCoroutine;
    private Coroutine changeItemLocalCoroutine;
    private Coroutine changeLocaleInfoRoutine;

    private string nameKey;
    private string descKey;
    private string interactKey;
    private List<float> valueList;
    private List<ItemApplyType> typeList;
    private string tableName;

    private void Reset()
    {
        logCanvasGroup = GameObject.Find("Log").GetComponent<CanvasGroup>();
        weaponCountWindow = GameObject.Find("WeaponCountWindow");

        itemNameTxt = transform.FindChild<TextMeshProUGUI>("ItemName");
        itemDescTxt = transform.FindChild<TextMeshProUGUI>("ItemDesc");
        itemLvTxt = transform.FindChild<TextMeshProUGUI>("ItemLv");
        itemPirceTxt = transform.FindChild<TextMeshProUGUI>("ItemPrice");

        weaponCountTxt = transform.FindChild<TextMeshProUGUI>("WeaponCount");
    }

    private void OnEnable()
    {
        EventBus.OnDropItemGain += OnDropItemGain;
        EventBus.OnMouseOnSlot += OnMouseOnSlot;
        EventBus.OnMouseOnInfo += OnMouseOnInfo;
        EventBus.OnMouseOffSlot += OnMouseOffSlot;
        EventBus.OnPlayerSlotUIOpen += OnPlayerSlotUIOpen;
        EventBus.OnPlayerSlotUIClose += OnPlayerSlotUIClose;
        EventBus.OnChangeWeaponSlotCount += UpdateWeaponInfo;
    }

    private void OnDisable()
    {
        EventBus.OnDropItemGain -= OnDropItemGain;
        EventBus.OnMouseOnSlot -= OnMouseOnSlot;
        EventBus.OnMouseOnInfo -= OnMouseOnInfo;
        EventBus.OnMouseOffSlot -= OnMouseOffSlot;
        EventBus.OnPlayerSlotUIOpen -= OnPlayerSlotUIOpen;
        EventBus.OnPlayerSlotUIClose -= OnPlayerSlotUIClose;
        EventBus.OnChangeWeaponSlotCount -= UpdateWeaponInfo;
    }

    private void Start()
    {
        forSeconds = new WaitForSeconds(logUiDuration);
        GameManager.Instance.LogInfoUI = this;
    }

    public void OnDropItemGain(ISlotItem item, Attribute lv)
    {
        // LogUI 설정
        SetLogUI(item, lv);

        // 드롭 아이템 획득 시 코루틴 실행
        if(logUIFadeCoroutine != null) StopCoroutine(logUIFadeCoroutine);
        logUIFadeCoroutine = StartCoroutine(FadeLogInfoUI());
    }

    private void SetLogUI(ISlotItem item, Attribute lv)
    {
        statInfo.SetActive(false);
        itemInfo.SetActive(true);

        nameKey = item.GetName();
        descKey = item.GetDesc();
        this.tableName = (item.GetType() == DropItemType.Weapon) ? Define.TABLE_WEAPON_NAME :
            (item.GetType() == DropItemType.ExclusiveItem) ? Define.TABLE_ITEM_EXCLUSIVE_NAME : Define.TABLE_ITEM_NAME;
        valueList = item.GetValueList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT);
        typeList = item.GetValueTypeList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT);

        // 레벨 설정
        itemLvTxt.text = lv.Value.ToString();

        // 판매 가격 설정
        if(GameManager.Instance.TemporaryInventory != null)
            itemPirceTxt.text = ((int)GameManager.Instance.TemporaryInventory.GetFinalSellPrice(item)).ToString();

        Locale currentLocale = LocalizationSettings.SelectedLocale;

        if (changeItemLocalCoroutine != null) StopCoroutine(changeItemLocalCoroutine);
        changeItemLocalCoroutine = StartCoroutine(ChangeLocaleItemRoutine(currentLocale, tableName));
    }

    private void ResetLogUI()
    {
        itemNameTxt.text = string.Empty;
        itemDescTxt.text = string.Empty;
    }

    private IEnumerator ChangeLocaleItemRoutine(Locale locale, string Table)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(Table);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(nameKey)?.GetLocalizedString();
            itemNameTxt.text = text;

            text = table.GetEntry(descKey)
                    ?.GetLocalizedString(new 
                    {
                        value_01 = UIHelper.ColorTextWithType(valueList[0], typeList[0], UIHelper.ColorOrange),
                        value_02 = UIHelper.ColorTextWithType(valueList[1], typeList[1], UIHelper.ColorOrange),
                        value_03 = UIHelper.ColorTextWithType(valueList[2], typeList[2], UIHelper.ColorOrange),
                        value_04 = UIHelper.ColorTextWithType(valueList[3], typeList[3], UIHelper.ColorOrange),
                        value_05 = UIHelper.ColorTextWithType(valueList[4], typeList[4], UIHelper.ColorOrange),
                    });
            itemDescTxt.text = text;
        }

        changeItemLocalCoroutine = null;
    }
    
    private IEnumerator ChangeLocaleInfoRoutine(Locale locale, string Table)
    {
        var loadingOperation = LocalizationSettings.StringDatabase.GetTableAsync(Table);
        yield return loadingOperation;

        if (loadingOperation.Status == AsyncOperationStatus.Succeeded)
        {
            StringTable table = loadingOperation.Result;

            string text = table.GetEntry(nameKey)?.GetLocalizedString();
            statNameTxt.text = text;

            text = table.GetEntry(descKey)?.GetLocalizedString();
            itemDescTxt.text = text;
        }
    }

    private IEnumerator FadeLogInfoUI()
    {
        float time = 0f;

        // 로그 UI 활성화
        logCanvasGroup.gameObject.SetActive(true);
        logCanvasGroup.alpha = 0f;

        while (time < fadeDuration)
        {
            time += UnityEngine.Time.deltaTime;

            float t = (time / fadeDuration);

            logCanvasGroup.alpha = Mathf.Lerp(logCanvasGroup.alpha, 1f, t);

            yield return null;
        }

        logCanvasGroup.alpha = 1f;
        time = 0f;

        // n초 동안 지속
        yield return forSeconds;

        while (time < fadeDuration)
        {
            time += UnityEngine.Time.deltaTime;

            float t = (time / fadeDuration);

            logCanvasGroup.alpha = Mathf.Lerp(logCanvasGroup.alpha, 0f, t);

            yield return null;
        }

        // 로그 UI 비활성화
        logCanvasGroup.gameObject.SetActive(false);
        logCanvasGroup.alpha = 0f;

        ResetLogUI();

        logUIFadeCoroutine = null;
    }

    public void OnMouseOnSlot(ISlotItem item, Attribute lv, Vector3? position)
    {
        // 현재 코루틴이 실행중일때는 해당 코루틴 중지
        if (logUIFadeCoroutine != null)
        {
            StopCoroutine(logUIFadeCoroutine);
            logUIFadeCoroutine = null;
            logCanvasGroup.gameObject.SetActive(false);
        }

        // 슬롯에 마우스 되면 LogUI 표시

        // LogUI 설정
        SetLogUI(item, lv);

        // 로그 UI 활성화
        logCanvasGroup.gameObject.SetActive(true);
        logCanvasGroup.alpha = 1f;

        UpdateLogUIPosition(position);
    }

    public void OnMouseOnInfo(string titleKey, string _descKey, Vector3? position)
    {
        // 로그 설정
        statInfo.SetActive(true);
        itemInfo.SetActive(false);

        nameKey = titleKey;
        descKey = _descKey;

        Locale currentLocale = LocalizationSettings.SelectedLocale;

        if (changeLocaleInfoRoutine != null) StopCoroutine(changeLocaleInfoRoutine);
        changeLocaleInfoRoutine = StartCoroutine(ChangeLocaleInfoRoutine(currentLocale, "StatusTable"));

        // 로그 UI 활성화
        logCanvasGroup.gameObject.SetActive(true);
        logCanvasGroup.alpha = 1f;

        UpdateLogUIPosition(position);
    }

    private void UpdateLogUIPosition(Vector3? targetPosition)
    {
        if (targetPosition == null) return;

        // Canvas 정보 가져오기 (Canvas Scaler에 의한 스케일 값 필요)
        Canvas rootCanvas = logCanvasGroup.GetComponentInParent<Canvas>();
        float canvasScale = rootCanvas.scaleFactor;

        // Rect 정보 가져오기
        RectTransform contentRect = logCanvasGroup.transform.GetChild(0).GetComponent<RectTransform>();

        // 실제 화면에 렌더링되는 크기(Scaled Size) 계산
        float scaledWidth = contentRect.rect.width * canvasScale;
        float scaledHeight = contentRect.rect.height * canvasScale;

        // 오프셋도 스케일에 맞춰 조정
        float offsetX = scaledWidth * 0.6f;
        float offsetY = scaledHeight * 0.5f;

        // 좌표 계산
        Vector3 expectedPos = targetPosition.Value + new Vector3(offsetX, offsetY, 0);

        // 화면 경계 체크 (Screen.width/height는 픽셀 단위이므로 scaled 값과 비교)
        if (targetPosition.Value.x + scaledWidth > Screen.width)
        {
            expectedPos.x = targetPosition.Value.x - offsetX;
        }
        if (targetPosition.Value.y + scaledHeight > Screen.height)
        {
            expectedPos.y = targetPosition.Value.y - offsetY;
        }

        // 위치 적용
        logCanvasGroup.transform.position = expectedPos;
    }

    public void OnMouseOffSlot()
    {
        // 현재 코루틴이 실행중일때는 return
        if (logUIFadeCoroutine != null) return;

        ResetLogUI();

        // 로그 UI 비활성화
        logCanvasGroup.gameObject.SetActive(false);
        logCanvasGroup.alpha = 0f;
    }

    public void OnPlayerSlotUIOpen()
    {
        weaponCountWindow.SetActive(false);
    }

    public void OnPlayerSlotUIClose()
    {
        OnMouseOffSlot();
        weaponCountWindow.SetActive(false);
    }

    public void UpdateWeaponInfo(int currentWeaponCount, int maxWeaponCount)
    {
        weaponCountTxt.text = $"({currentWeaponCount}/{maxWeaponCount})";
    }

    #region 프로퍼티

    public CanvasGroup Window { get { return logCanvasGroup; } set { logCanvasGroup = value; } }

    #endregion
}
