using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReinforcementSlot : Slot
{
    private ReinforcementView reinforcementView;

    public void SetReinforcementView(ReinforcementView _reinforcementView)
    {
        reinforcementView = _reinforcementView;
    }


    #region 마우스 관련 로직
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && IsActive)
        {
            //// DragSlot 설정
            //DragSlot.instance.dragSlot = this;
            //DragSlot.instance.SetDragItem(this.icon.sprite, iconRect, aura);
            //DragSlot.instance.transform.position = eventData.position;

            //this.icon.color = new Color(1, 1, 1, 0);

            //// Lv UI 비활성화
            //lv.gameObject.SetActive(false);

            //// DropSlot 변수 설정
            //isDropOnSlot = false;

            //// UI 설정
            //if (GameManager.Instance.LogInfoUI.Window.gameObject.activeSelf)
            //{
            //    GameManager.Instance.LogInfoUI.Window.gameObject.SetActive(false);
            //}
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            //DragSlot.instance.transform.position = eventData.position;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        ResetDragSlot();

        //UpdateLvUI();

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

        if (this.Item != null)
            DeleteItem(Item, Index);

        if (DragSlot.instance.dragSlot is WeaponSlot weaponSlot)
        {
            this.AddItem(sourceItem, Index, false);
            if(reinforcementView != null)
                reinforcementView.CurrentSlot = weaponSlot;
        }
        else if (DragSlot.instance.dragSlot is ItemSlot itemSlot)
        {
            this.AddItem(sourceItem, Index, false);
            if (reinforcementView != null)
                reinforcementView.CurrentSlot = itemSlot;
        }
        else if (DragSlot.instance.dragSlot is TemporarySlot temporarySlot)
        {
            this.AddItem(sourceItem, Index, false);
            if (reinforcementView != null)
                reinforcementView.CurrentSlot = temporarySlot;
        }
        else
        {
            this.AddItem(sourceItem, Index, false);
            if (reinforcementView != null)
                reinforcementView.CurrentSlot = sourceSlot;
        }

        if(reinforcementView != null)
        {
            reinforcementView.SetView(sourceItem);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {

    }

    #endregion
}
