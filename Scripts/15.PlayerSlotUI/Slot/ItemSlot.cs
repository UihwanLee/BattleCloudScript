
// Item 정보를 담은 Slot
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : Slot
{
    [Header("ItemSlot 컴포넌트")]
    [SerializeField] private Button itemSlotBtn;
    [SerializeField] private TextMeshProUGUI reinforcementCost;

    private float currentReinforcementCost = -1;

    public override void Reset()
    {
        base.Reset();

        reinforcementCost = transform.FindChild<TextMeshProUGUI>("ReinforcementCost");
        itemSlotBtn = GetComponent<Button>();
    }

    public void LoadSlot(ItemSlotData itemSlot, int index)
    {
        if(itemSlot != null)
        {
            if(itemSlot.Item != null)
            {
                itemSlot.Item.Load(itemSlot.ItemID);
                AddItem(itemSlot.Item, index, false);
            }
        }
    }

    public void SetReinforcementCost()
    {
        // 현재 아이템 없으면 return
        if (IsActive == false) return;

        // 아이템 레벨 별 강화비용 표시하기
        currentReinforcementCost = GameRule.Instance.GetItemReinforcementCost(Item.GetLv());

        reinforcementCost.gameObject.SetActive(true);
        reinforcementCost.text = $"-{currentReinforcementCost}";
    }

    public void ResetReinforcementCost()
    {
        reinforcementCost.text = string.Empty;
        reinforcementCost.gameObject.SetActive(false);
    }

    public override void AddItem(ISlotItem item, int index, bool isLog, bool? isSwap = null)
    {
        base.AddItem(item, index, isLog);

        // 효과적용
        if(item != null && isSwap == null)
        {
            Item newItem = (Item)item;
            newItem.ApplyItem(player, true, Index);
            playerSlotUI.CurrentActiveItemSlotCount++;
            if(isLog) EventBus.OnDropItemGain?.Invoke(item, item.GetLv());
            base.UpdateLv(item.GetLv());
        }

        // DetailView 갱신
        if (GameManager.Instance.DetailViewUI.DetailViewWindow.activeSelf)
        {
            if (GameManager.Instance.DetailViewUI.CurrentViewWeaponIndex == index)
                GameManager.Instance.DetailViewUI.SetUI(index);
        }

        playerSlotUI.UpdateItemSlot();
    }

    public override void DeleteItem(ISlotItem item, int index, bool? isSwap = null)
    {
        base.DeleteItem(item, index);

        // 효과 빼기
        if (item != null && isSwap == null)
        {
            Item newItem = (Item)item;
            newItem.ApplyItem(player, false, Index);
            playerSlotUI.CurrentActiveItemSlotCount--;
        }

        // DetailView 갱신
        if (GameManager.Instance.DetailViewUI.DetailViewWindow.activeSelf)
        {
            if (GameManager.Instance.DetailViewUI.CurrentViewWeaponIndex == index)
                GameManager.Instance.DetailViewUI.SetUI(index);
        }

        playerSlotUI.UpdateItemSlot();
    }

    public override void UpdateLv(Attribute _Lv)
    {
        // 현재 아이템에도 레벨 적용
        item.SetLv(_Lv.Value);
        lv.text = (_Lv.Value < Define.MAX_ITEM_LV) ? $"Lv. {_Lv.Value}" : Define.MAX_LV_LABEL;
        lvContainer.color = item.GetTierColor();

        UpdateIconMaterial(_Lv);
    }

    #region 마우스 포인터 관련 로직

    public override void OnDrag(PointerEventData eventData)
    {
        if (playerSlotUI.IsReinforcementMode) return;

        if (item != null)
        {
            // 아이템 슬롯 드래그 중에서는 이동할 수 있는 모든 슬롯을 보여준다.
            DragSlot.instance.transform.position = eventData.position;
            playerSlotUI.WeaponDragHighlight();
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (playerSlotUI.IsReinforcementMode) return;

        base.OnDrop(eventData);

        // 인벤토리 슬롯에 Advice를 놓으면 미적용
        bool isChange = false;
        if (DragSlot.instance.dragSlot != null)
        {
            ISlotItem item = DragSlot.instance.dragSlot.Item;
            if (item != null)
            {
                // Item Type이 Item인지 확인
                if (item.GetType() == WhereAreYouLookinAt.Enum.DropItemType.Item ||
                    item.GetType() == WhereAreYouLookinAt.Enum.DropItemType.ExclusiveItem)
                {
                    // 자기 슬롯에 놓은것이 아닌지 확인
                    if (DragSlot.instance.dragSlot != this)
                    {
                        // DragSlotType이 임시 인벤토리인지 확인
                        if (DragSlot.instance.dragSlot is TemporarySlot temporarySlot)
                        {
                            SwapTemporarySlot(temporarySlot);
                            return;
                        }

                        isChange = true;
                    }
                }
            }
        }

        if (DragSlot.instance.dragSlot != null && isChange)
        {
            ChangeSlot();
        }
    }

    private void SwapTemporarySlot(TemporarySlot temporarySlot)
    {
        if (this.Item == null)
        {
            AddItem(temporarySlot.Item, Index, false);
            temporarySlot.DeleteItem(temporarySlot.Item, temporarySlot.Index);
        }
        else
        {
            ItemSlotData itemSlotData = GetItemData();

            DeleteItem(Item, Index);
            AddItem(temporarySlot.Item, Index, false);

            temporarySlot.DeleteItem(temporarySlot.Item, temporarySlot.Index);
            temporarySlot.AddItem(itemSlotData.Item, Index, false);
        }

        GameManager.Instance.TemporaryInventory.UpdateLvUI();
    }

    public ItemSlotData GetItemData()
    {
        ItemSlotData data = new ItemSlotData
        {
            Item = this.Item,
            IsActvie = this.IsActive,
        };

        return data;
    }

    // 다른 ItemSlot과 Swap
    public void SetItemData(ItemSlotData otherItemSlot)
    {
        // 일단 다 삭제
        DeleteItem(Item, Index);

        // WeaponSlot 적용
        if (otherItemSlot.Item != null)
        {
            AddItem(otherItemSlot.Item, Index, false);
        }
    }

    public override void ChangeSlot()
    {
        ItemSlot sourceSlot = (ItemSlot)DragSlot.instance.dragSlot;
        ItemSlot targetSlot = this;

        // ItemData 추출
        ItemSlotData sourceData = sourceSlot.GetItemData();
        ItemSlotData targetData = targetSlot.GetItemData();

        // ItemData 적용
        sourceSlot.SetItemData(targetData);
        targetSlot.SetItemData(sourceData);

        // DetailView 갱신
        if (GameManager.Instance.DetailViewUI)
        {
            // 만약 켜져 있는 상태에서 Swap 시 그 Swap된 Slot으로 다시 Set하기
            if (GameManager.Instance.DetailViewUI.DetailViewWindow.activeSelf)
            {
                GameManager.Instance.DetailViewUI.SetUI(targetSlot.Index);

                // Interact 설정
                playerSlotUI.SetHighlightInteract(targetSlot.Index);
            }
        }

        EventBus.OnMouseOffSlot?.Invoke();

        playerSlotUI.UpdateItemSlot();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        playerSlotUI.HighlightInteractBG(Index);

        if (DragSlot.instance.dragSlot == null)
        {
            if (item == null) return;

            // LogInfoUI 켜기
            EventBus.OnMouseOnSlot?.Invoke(item, item.GetLv(), transform.position);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot == null)
        {
            if (item == null) return;

            // LogInfoUI 끄기
            EventBus.OnMouseOffSlot?.Invoke();
        }
    }

    #endregion

    #region 버튼 로직

    public void OnClickItemSlot()
    {
        //if (!IsActive) return;

        //if (Item == null) return;

        if(GameManager.Instance.DetailViewUI != null)
        {
            // DetailViewUI 슬롯 열기
            GameManager.Instance.DetailViewUI.SetUI(Index);

            // Interact 설정
            playerSlotUI.SetHighlightInteract(Index);
        }
    }

    #endregion
}
