using UnityEngine;

public class PlayerSlotWindow : MonoBehaviour
{
    private PlayerSlotUI slotUI;

    private void OnEnable()
    {
        if(slotUI != null)
        {
            EventBus.OnPlayerSlotUIOpen?.Invoke();

            // 킬때 마다 하이라이트 업데이트
            slotUI.UpdateWeaponSlot();
            slotUI.UpdateItemSlot();
        }
    }

    private void OnDisable()
    {
        if (slotUI != null)
        {
            if(EventBus.OnPlayerSlotUIClose != null) EventBus.OnPlayerSlotUIClose();
            slotUI.ResetDragSlot();
        }
    }

    public void Init(PlayerSlotUI slotUI)
    {
        this.slotUI = slotUI;   
    }
}
