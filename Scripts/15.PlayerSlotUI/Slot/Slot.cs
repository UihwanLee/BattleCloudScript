using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

// Item 정보를 담은 Slot 클래스
public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Slot 정보")]
    [SerializeField] private int index;                         // 슬롯 인덱스
    [SerializeField] protected ISlotItem item;                  // 슬롯에 담을 아이템
    [SerializeField] protected TextMeshProUGUI lv;              // 슬롯 아이템 레벨
    [SerializeField] protected Image lvContainer = null;        // 슬롯 아이템 레벨 UI

    [Header("컴포넌트")]
    [SerializeField] protected Image bg;            // 슬롯 배경
    [SerializeField] protected Image highlight;     // 슬롯 하이라이트
    [SerializeField] protected Image icon;          // 슬롯 아이콘
    [SerializeField] protected Image aura;          // 슬롯 아우라

    [SerializeField] private bool isActive;         // 활성 상태 변수
    protected RectTransform iconRect;                 // 슬롯 아이콘 RectTransform

    private InfoUIPoolManager infoUIPoolManager;    // InfoUI 매니저 스크립트
    private GameObject currentInfoUI;

    public bool isDropOnSlot;                    // 슬롯에 드롭했는지 체크하는 변수
    protected DropItemPool dropItemPool;            // 드롭 아이템 Pool
    protected Player player;
    protected PlayerSlotUI playerSlotUI;

    private bool isInfoUIOpen;                      // InfoUI가 현재 띄어진 상태인지 판단하는 변수
    protected Coroutine spreadCoroutine;            // Spread 애니메이션 코루틴

    // 애니메이션 저장 데이터
    protected Vector3 originLocalPosition;
    protected Color bgColor;
    protected Color highlightColor;
    protected Color iconColor;

    [Header("Meterials")]
    [SerializeField] private List<Material> iconMaterials = new List<Material>();
    [SerializeField] private List<Material> auraMaterials = new List<Material>();

    public virtual void Reset()
    {
        bg = transform.FindChild<Image>("BG");
        highlight = transform.FindChild<Image>("BG_Highlight");
        icon = transform.FindChild<Image>("Icon");
        lv = transform.FindChild<TextMeshProUGUI>("Lv");
        //iconLock = transform.FindChild<Image>("Icon_Lock");
    }

    private void Start()
    {
        iconRect = icon.transform.GetComponent<RectTransform>();
        infoUIPoolManager = InfoUIPoolManager.Instance;
        dropItemPool = DropItemPool.Instance;

        isInfoUIOpen = false;
    }

    public void SetPlayer(Player player, PlayerSlotUI playerSlotUI)
    {
        this.player = player;
        this.playerSlotUI = playerSlotUI;
    }

    #region 슬롯 세팅

    public void ResetSlot()
    {
        if (item != null) DeleteItem(item, Index);
    }

    public virtual void SetIndex(int index)
    {
        this.index = index;
    }

    public virtual void InitAnimationData()
    {
        originLocalPosition = transform.localPosition;
        bgColor = bg.color;
        highlightColor = highlight.color;
        iconColor = icon.color;
    }

    public void SetActive(bool active)
    {
        this.isActive = active;
    }

    public virtual void InitIcon()
    {
        icon.gameObject.SetActive(false);
    }

    public void SetHighlight(bool active)
    {
        highlight.gameObject.SetActive(active);
    }

    public virtual void AddItem(ISlotItem item, int index, bool isLog, bool? isSwap = null)
    {
        this.item = item;
        icon.gameObject.SetActive(true);
        icon.sprite = item.GetIcon();

        // 활성화
        isActive = true;
    }

    public virtual void DeleteItem(ISlotItem item, int index, bool? isSwap = null)
    {
        isActive = false;
        this.item = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
    }

    public void SetLvUI(bool active)
    {
        lv.gameObject.SetActive(active);
    }

    public virtual void UpdateLvUI()
    {
        if (item != null) UpdateLv(item.GetLv());
        lv.gameObject.SetActive(isActive);
        if(lvContainer!=null) lvContainer.gameObject.SetActive(isActive);
    }

    public void ResetLvUI()
    {
        lv.gameObject.SetActive(false);
        lvContainer.gameObject.SetActive(false);
    }

    public virtual void UpdateLv(Attribute _Lv)
    {
        // 현재 아이템에도 레벨 적용
        item.SetLv(_Lv.Value);
        lv.text = $"Lv. {_Lv.Value}";

        UpdateIconMaterial(_Lv);
    }

    public void UpdateIconMaterial(Attribute _Lv)
    {
        if (iconMaterials.Count == 0 || auraMaterials.Count == 0)
            return;

        int level = (int)_Lv.Value;

        if (icon != null)
            icon.material = iconMaterials[level - 1];
        if (aura != null)
            aura.material = auraMaterials[level - 1];
    }

    #endregion

    #region 마우스 포인터 관련 로직

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && isActive)
        {
            // DragSlot 설정
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.SetDragItem(this.icon.sprite, iconRect, aura);
            DragSlot.instance.transform.position = eventData.position;

            this.icon.color = new Color(1, 1, 1, 0);

            // Lv UI 비활성화
            lv.gameObject.SetActive(false);

            if(lvContainer != null)
                lvContainer.gameObject.SetActive(false);

            // DropSlot 변수 설정
            isDropOnSlot = false;

            // PlayerSlotUI에게 전달
            playerSlotUI.SetDragSlot(this);

            // 아우라 비활성화
            if(aura != null)
                aura.gameObject.SetActive(false);

            // UI 설정
            if (GameManager.Instance.LogInfoUI.Window.gameObject.activeSelf)
            {
                GameManager.Instance.LogInfoUI.Window.gameObject.SetActive(false);
            }
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (playerSlotUI.IsReinforcementMode) return;
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (playerSlotUI.IsReinforcementMode) return;

        ResetDragSlot();

        // PlayerSlotUI에게 전달
        playerSlotUI.SetDragSlot(null);

        UpdateLvUI();

        //// 밖에다 버리면
        //if (isDropOnSlot == false)
        //{
        //    // 아이템 드랍
        //    DropItem();
        //}
    }

    public virtual void ResetDragSlot()
    {
        if (DragSlot.instance.dragSlot != null) DragSlot.instance.dragSlot.icon.color = new Color(1, 1, 1, 1);
        this.icon.color = new Color(1, 1, 1, 1);

        DragSlot.instance.HideDragItem();
        DragSlot.instance.dragSlot = null;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            DragSlot.instance.dragSlot.isDropOnSlot = true;
        }
    }

    public virtual void ChangeSlot()
    {
        Slot sourceSlot = DragSlot.instance.dragSlot;
        ISlotItem sourceItem = sourceSlot.item;

        ISlotItem targetItem = this.item;

        this.AddItem(sourceItem, Index, false);

        if (targetItem != null)
        {
            sourceSlot.AddItem(targetItem, Index, false);
        }
        else
        {
            sourceSlot.DeleteItem(sourceItem, Index);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {

    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            //UnClick();
        }
        else
        {
            // LogInfoUI 끄기
            EventBus.OnMouseOffSlot?.Invoke();
        }
    }

    private void DropItem()
    {
        if (item == null) return;

        // 아이템 드랍
        dropItemPool.SpawnDropItem(Item);

        // 현재 슬롯은 비우기
        DeleteItem(Item, Index);
    }

    #endregion

    #region 애니메이션 연출

    public virtual void StartSpreadOut(float duration)
    {
        if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);
        spreadCoroutine = StartCoroutine(SpreadOutCoroutine(duration));
    }

    private IEnumerator SpreadOutCoroutine(float duration)
    {
        // 원래 위치 저장
        Vector3 targetPosition = originLocalPosition;

        // 원점에서 출발
        transform.localPosition = Vector3.zero;

        // 색상 설정
        float targetBgAlpha = bgColor.a;
        float targetHighlightAlpha = highlightColor.a;
        float targetIconAlpha = iconColor.a;

        float time = 0.0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, (time / duration));

            float bgAlpha = Mathf.Lerp(0f, targetBgAlpha, (time / duration));
            float highlighAlpha = Mathf.Lerp(0f, targetHighlightAlpha, (time / duration));
            float iconAlpha = Mathf.Lerp(0f, targetIconAlpha, (time / duration));

            SetColorWithChangedAlpha(bg, bgColor, bgAlpha);
            SetColorWithChangedAlpha(highlight, highlightColor, highlighAlpha);
            SetColorWithChangedAlpha(icon, iconColor, iconAlpha);

            yield return null;
        }

        SetColorWithChangedAlpha(bg, bgColor, targetBgAlpha);
        SetColorWithChangedAlpha(highlight, highlightColor, targetHighlightAlpha);
        SetColorWithChangedAlpha(icon, iconColor, targetIconAlpha);

        transform.localPosition = targetPosition;
    }

    public virtual void StartSpreadIn(float duration)
    {
        if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);
        spreadCoroutine = StartCoroutine(SpreadInCoroutine(duration));
    }

    private IEnumerator SpreadInCoroutine(float duration)
    {
        // 원래 위치가 타켓
        Vector3 targetPosition = Vector3.zero;

        // 색상 설정
        float originBgAlpha = bgColor.a;
        float originHighlightAlpha = highlightColor.a;
        float originIconAlpha = iconColor.a;

        float time = 0.0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);

            float moveT = t;
            float alphaT = Mathf.Clamp01(time / (duration / 5));

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, moveT);

            float bgAlpha = Mathf.Lerp(originBgAlpha, 0f, alphaT);
            float highlighAlpha = Mathf.Lerp(originHighlightAlpha, 0f, alphaT);
            float iconAlpha = Mathf.Lerp(originIconAlpha, 0f, alphaT);

            SetColorWithChangedAlpha(bg, bgColor, bgAlpha);
            SetColorWithChangedAlpha(highlight, highlightColor, highlighAlpha);
            SetColorWithChangedAlpha(icon, iconColor, iconAlpha);

            yield return null;
        }


        SetColorWithChangedAlpha(bg, bgColor, 0f);
        SetColorWithChangedAlpha(highlight, highlightColor, 0f);
        SetColorWithChangedAlpha(icon, iconColor, 0f);

        transform.localPosition = targetPosition;
    }

    protected void SetColorWithChangedAlpha(Image img, Color originColor, float alpha)
    {
        Color color = new Color(originColor.r, originColor.g, originColor.b, alpha);
        img.color = color;
    }

    #endregion

    #region 프로퍼티

    public int Index { get { return index; } }
    public bool IsActive { get { return isActive; } }
    public Image Icon { get { return icon; } set { icon = value; } }
    public ISlotItem Item { get { return item; } }

    #endregion
}
