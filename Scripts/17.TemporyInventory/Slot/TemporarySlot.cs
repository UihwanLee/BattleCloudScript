using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TemporarySlot : Slot
{
    [SerializeField] private Image highlight_BG;
    private Coroutine flashCoroutine;

    public int DataIndex { get; set; }

    public override void UpdateLv(Attribute _Lv)
    {
        // 현재 아이템에도 레벨 적용
        item.SetLv(_Lv.Value);
        lv.text = (_Lv.Value < Define.MAX_WEAPON_LV) ? $"Lv. {_Lv.Value}" : Define.MAX_LV_LABEL;
        lvContainer.color = item.GetTierColor();

        UpdateIconMaterial(_Lv);
    }

    #region 마우스 관련 로직

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && IsActive)
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

            // UI 설정
            if (GameManager.Instance.LogInfoUI.Window.gameObject.activeSelf)
            {
                GameManager.Instance.LogInfoUI.Window.gameObject.SetActive(false);
            }
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        ResetDragSlot();

        UpdateLvUI();

        //// 밖에다 버리면
        //if (isDropOnSlot == false)
        //{
        //    // 아이템 드랍
        //    DropItem();
        //}
    }

    public override void ResetDragSlot()
    {
        if (DragSlot.instance.dragSlot != null) DragSlot.instance.dragSlot.Icon.color = new Color(1, 1, 1, 1);
        this.icon.color = new Color(1, 1, 1, 1);

        DragSlot.instance.HideDragItem();
        DragSlot.instance.dragSlot = null;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);

        // 인벤토리 슬롯에 Advice를 놓으면 미적용
        bool isChange = false;
        if (DragSlot.instance.dragSlot != null)
        {
            ISlotItem item = DragSlot.instance.dragSlot.Item;
            if (item != null)
            {
                // 자기 슬롯에 놓은것이 아닌지 확인
                if (DragSlot.instance.dragSlot != this)
                {
                    isChange = true;
                }
            }
        }

        if (DragSlot.instance.dragSlot != null && isChange)
        {
            ChangeSlot();
        }
    }

    public override void ChangeSlot()
    {
        Slot sourceSlot = DragSlot.instance.dragSlot;
        ISlotItem sourceItem = sourceSlot.Item;

        ISlotItem targetItem = this.item;

        this.AddItem(sourceItem, Index, false);

        if (DragSlot.instance.dragSlot is WeaponSlot weaponSlot)
        {
            // WeaponSlot에서 삭제
            WeaponSlotData weaponData = weaponSlot.GetWeaponData();
            weaponSlot.DeleteItem(weaponSlot.Item, weaponSlot.Index);

            if(targetItem != null)
            {
                weaponSlot.AddItem(targetItem, weaponSlot.Index, false);
            }
        }
        else if (DragSlot.instance.dragSlot is ItemSlot itemSlot)
        {
            // 아이템 슬롯에서 삭제
            ItemSlotData itemData = itemSlot.GetItemData();
            itemSlot.DeleteItem(itemSlot.Item, itemSlot.Index); 

            if(targetItem != null)
            {
                itemSlot.AddItem(targetItem, itemSlot.Index, false);
            }
        }
        else
        {
            if (targetItem != null)
            {
                if (sourceSlot.Item != null) sourceSlot.DeleteItem(sourceSlot.Item, sourceSlot.Index);
                sourceSlot.AddItem(targetItem, sourceSlot.Index, false);
            }
            else
            {
                sourceSlot.DeleteItem(sourceItem, sourceSlot.Index);
            }
        }

        GameManager.Instance.TemporaryInventory.UpdateLvUI();
        GameManager.Instance.Player.PlayerSlotUI.UpdateItemSlot();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            if (DragSlot.instance.dragSlot != this)
            {
                //Click();
            }
        }
        else
        {
            if (item == null) return;

            // LogInfoUI 켜기
            EventBus.OnMouseOnSlot?.Invoke(item, item.GetLv(), transform.position);
        }
    }

    #endregion

    #region 애니메이션

    public void PlayFlash()
    {
        if(flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        //  플래시 
        highlight_BG.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.05f);
        highlight_BG.color = new Color(1, 1, 1, 0);
    }

        #endregion
    }
