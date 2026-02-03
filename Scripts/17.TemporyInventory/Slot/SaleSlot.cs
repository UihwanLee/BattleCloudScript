using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WhereAreYouLookinAt.Enum;

public class SaleSlot : Slot
{
    #region 마우스 포인터 로직

    public override void OnEndDrag(PointerEventData eventData)
    {
        ResetDragSlot();

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
        if (DragSlot.instance.dragSlot != null)
        {
            DragSlot.instance.dragSlot.isDropOnSlot = true;
            if(DragSlot.instance.dragSlot.Item != null)
            {
                if (DragSlot.instance.dragSlot is TemporarySlot temporarySlot)
                {
                    GameManager.Instance.TemporaryInventory.SellItem(temporarySlot.Item);
                }
                else if(DragSlot.instance.dragSlot is ItemSlot itemSlot)
                {
                    GameManager.Instance.TemporaryInventory.SellItem(itemSlot.Item);
                    itemSlot.DeleteItem(itemSlot.Item, itemSlot.Index);
                }
                else if (DragSlot.instance.dragSlot is WeaponSlot weaponSlot)
                {
                    GameManager.Instance.TemporaryInventory.SellItem(weaponSlot.Item);
                    weaponSlot.DeleteItem(weaponSlot.Item, weaponSlot.Index);
                }
            }
        }
    }

    #endregion
}
