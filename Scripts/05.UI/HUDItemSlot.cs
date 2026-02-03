using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDItemSlot : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private Image icon;
    [SerializeField] private ISlotItem item;

    private void Reset()
    {
        icon = transform.FindChild<Image>("Icon");
    }

    public void Set(ISlotItem slotItem)
    {
        this.item = slotItem;

        // HUD에는 이미지 사진이 안보임 -> 기본 이미지
        icon.sprite = DataManager.BaseDropItemSprite;
    }

    #region 프로퍼티

    public Image Icon { get { return icon; } }
    public ISlotItem Item { get { return item; } }

    #endregion
}
