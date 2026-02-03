using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ReinforcementNPC : MonoBehaviour, IInteractable
{
    [Header("컴포넌트")]
    [SerializeField] GameObject model;

    private InfoUIPoolManager infoUIPoolManager;
    private GameObject currentInfoUI;

    private Player player;
    private bool isCanInteract;

    private void Reset()
    {
        model = GameObject.Find("ReinforcementNPCModel");
    }

    private void Start()
    {
        infoUIPoolManager = InfoUIPoolManager.Instance;
        ResetReinforcementNPC();

        player = GameManager.Instance.Player;
        GameManager.Instance.ReinforcementNPC = this;
    }

    public void ResetReinforcementNPC()
    {
        isCanInteract = false;
        model.SetActive(false);
    }

    public void SetReinforcementNPC()
    {
        isCanInteract = true;
        model.SetActive(true);
    }

    #region 인터페이스 구현

    public Vector3 GetPosition()
    {
        return this.transform.position;
    }

    public int GetPriority()
    {
        return Define.INTERACT_PRIORITY_02;
    }

    public void OpenInteractUI(Player player)
    {
        if (isCanInteract == false) return;

        // InteractUI 띄우기
        currentInfoUI = infoUIPoolManager.GetInfoUI(InfoUIType.Interact, transform.position);

        if (currentInfoUI != null)
        {
            InteractUI interactUI = currentInfoUI.GetComponent<InteractUI>();
        }
    }

    public void CloseInteractUI(Player player)
    {
        if (isCanInteract == false) return;

        // 플레이어 PlayerSlotUI 끄기
        player.IsReinforcementMode = false;
        player.PlayerSlotUI.ResetReinforcementCostItemSlot();
        player.PlayerSlotUI.SetPlayerSlotUI(false);
        player.PlayerSlotUI.IsReinforcementMode = false;

        // InteractUI 반환
        if (currentInfoUI != null)
        {
            infoUIPoolManager.Release(InfoUIType.Interact, currentInfoUI);
        }
    }

    public void Interact(Player player)
    {
        if (isCanInteract == false) return;

        // 강화모드 키고 다시 Slot 열기
        player.IsReinforcementMode = true;
        player.PlayerSlotUI.UpdateReinforcementCostItemSlot();
        player.PlayerSlotUI.SetPlayerSlotUI(true);
        player.PlayerSlotUI.IsReinforcementMode = true;
    }

    #endregion
}
