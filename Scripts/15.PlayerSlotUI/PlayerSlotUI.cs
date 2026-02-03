using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerSlotUI : MonoBehaviour
{
    [Header("슬롯 정보")]
    [SerializeField] private PlayerSlotWindow slotWindow;
    [SerializeField] private List<WeaponSlot> weaponSlots = new List<WeaponSlot>();

    [field: Header("Player 슬롯 데이터")]
    [field: SerializeField] public int MaxWeaponSlotCount {  get; set; }            // 최대 WeaponSlot 개수
    [field: SerializeField] public int MaxItemSlotCount { get; set; }               // 최대 ItemSlot 개수
    [field: SerializeField] public int ActiveWeaponSlotCount { get; set; }          // 해금된 weaponSlot 개수
    [field: SerializeField] public int ActiveItemSlotCount { get; set; }            // 해금된 itemSlot 개수
    [field: SerializeField] public int CurrentActiveWeaponSlotCount { get; set; }   // 현재 활성화 된 weaponSlot 개수
    [field: SerializeField] public int CurrentActiveItemSlotCount { get; set; }     // 현재 활성화 된 itemSlot 개수

    [Header("애니메이션 정보")]
    [Header("Weapon Slot Spread 속도")]
    [SerializeField] private float weaponSlotSpreadDuration = 0.5f;
    [SerializeField] private float itemSlotSpreadDuration = 0.3f;

    [Header("책자 삐져나오는 정도")]
    [SerializeField] private float offsetAnimationInInteractBG = 0.5f;
    private WeaponSlot currentInteractBG = null;

    [field: Header("강화 설정")]
    [field: SerializeField] public bool IsReinforcementMode { get; set; } = false;

    private Slot currentDragSlot;   // 현재 드래그하고 있는 슬롯

    private void Reset()
    {
        slotWindow = transform.FindChild<PlayerSlotWindow>("PlayerSlotWindow");
        weaponSlots = transform.GetComponentsInChildren<WeaponSlot>().ToList();
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.Reset();
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        for(int i=0; i<weaponSlots.Count; i++)
        {
            int index = i;
            weaponSlots[i].SetIndex(index);
            weaponSlots[i].InitSlot();
            weaponSlots[i].InitIcon();
            weaponSlots[i].SetInteractBG(offsetAnimationInInteractBG);
            weaponSlots[i].InitAnimationData();
            weaponSlots[i].ResetReinforcementCostItemSlot();
        }

        // 처음에는 Weapon/Item Slot 3개/18개 씩 세팅하고 간다.
        MaxWeaponSlotCount = 6;
        MaxItemSlotCount = 18;
        ActiveWeaponSlotCount = 6;
        ActiveItemSlotCount = 18;
        CurrentActiveItemSlotCount = 0;
        CurrentActiveWeaponSlotCount = 0;

        slotWindow.Init(this);

        slotWindow.gameObject.SetActive(true);
    }

    private void Start()
    {
        
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            weaponSlots[i].InitPlayer(GameManager.Instance.Player, this);
        }

        EventBus.OnChangeWeaponSlotCount?.Invoke(CurrentActiveWeaponSlotCount, ActiveWeaponSlotCount);
    }

    #region SaveData 로드

    public void Load(PlayerSlotData playerSlotData, List<WeaponSlotData> _weaponSlots)
    {
        MaxItemSlotCount = playerSlotData.MaxItemSlotCount;
        MaxWeaponSlotCount = playerSlotData.MaxWeaponSlotCount;
        ActiveWeaponSlotCount = playerSlotData.ActiveWeaponSlotCount;
        ActiveItemSlotCount = playerSlotData.ActiveItemSlotCount;

        // 기존에 있던 모든 Slot 삭제 및 적용
        ResetAllSlot();

        if(_weaponSlots.Count == 0)
        {
            Debug.Log("로드할 데이터가 없습니다!");
            return;
        }

        // 로드
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            weaponSlots[i].LoadSlot(_weaponSlots[i]);
        }
    }

    #endregion

    #region Player 슬롯 설정


    public void ResetAllSlot()
    {
        for(int i=0; i<weaponSlots.Count;i++)
        {
            weaponSlots[i].ResetAllSlot();
        }
    }

    public void ResetDragSlot()
    {
        if (currentDragSlot)
        {
            currentDragSlot.ResetDragSlot();
            currentDragSlot = null;
        }
    }

    public void SetDragSlot(Slot slot)
    {
        currentDragSlot = slot;
    }

    public void SetPlayerSlotUI(bool active)
    {
        if(active)
        {
            StartSpreadOut();
        }
        else
        {
            StartSpreadIn();
        }
    }

    public void SetPlayerSlotWindow(bool active)
    {
        slotWindow.gameObject.SetActive(active);
    }


    // 현재 가용 가능한 WeaponSlot이 있는지 체크
    public bool IsCanPickUpWeapon()
    {
        return ActiveWeaponSlotCount > CurrentActiveWeaponSlotCount;
    }

    // 현재 가용 가능한 ItemSlot이 있는지 체크
    public bool IsCanPickUpItem()
    {
        return ActiveItemSlotCount > CurrentActiveItemSlotCount;
    }

    public bool IsFullWeaponSlot()
    {
        return (ActiveWeaponSlotCount >= MaxWeaponSlotCount);
    }

    public bool IsFullItemSlot()
    {
        return (ActiveItemSlotCount >= MaxItemSlotCount);
    }

    // Weapon 슬롯 개수 증가
    public bool AddWeaponSlot()
    {
        if (IsFullWeaponSlot()) return false;

        ActiveWeaponSlotCount++;
        EventBus.OnChangeWeaponSlotCount?.Invoke(CurrentActiveWeaponSlotCount, ActiveWeaponSlotCount);
        UpdateWeaponSlot();
        return true;
    }

    // Item 슬롯 개수 증가
    public bool AddItemSlot()
    {
        if(IsFullItemSlot()) return false;

        ActiveItemSlotCount++;
        UpdateItemSlot();
        return true;
    }

    // Weapon 추가
    public int AddWeapon(ISlotItem item)
    {
        if (ActiveWeaponSlotCount <= CurrentActiveWeaponSlotCount)
        {
            Debug.Log("더이상 아이템을 소지할 수 없습니다.");
            return Define.NO_FIND_NUMBER;
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i].Item == null)
            {
                // 비워있는 WeaponSlot에 item 추가
                weaponSlots[i].AddItem(item, weaponSlots[i].Index, true);
                return weaponSlots[i].Index;
            }
        }

        return Define.NO_FIND_NUMBER;
    }

    public int AddItem(ISlotItem item)
    {
        if (ActiveItemSlotCount <= CurrentActiveItemSlotCount)
        {
            Debug.Log("더이상 아이템을 소지할 수 없습니다.");
            return Define.NO_FIND_NUMBER;
        }

        int itemIndex = Define.NO_FIND_NUMBER;

        // 아이템 추가는 해금된 Weapon의 해금된 Item 순으로 순차적 추가
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            if((weaponSlot.AddItemAtEmtpyItemSlot(item)))
            {
                return weaponSlot.Index;
            }
        }

        return itemIndex;
    }

    public void UpdateWeaponSlot()
    {
        // 만약 가득 차 있다면 
        if(ActiveWeaponSlotCount <= CurrentActiveWeaponSlotCount)
        {
            // WeaponSlot이 모두 가득 차면 Highlight 활성화
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                weaponSlots[i].SetHighlight(!weaponSlots[i].IsActive);
                weaponSlots[i].UpdateLvUI();
            }
        }
        else
        {
            // 아니면 모두 Highlight 해제
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                weaponSlots[i].SetHighlight(false);
                weaponSlots[i].UpdateLvUI();
            }
        }
    }

    public void UpdateItemSlot()
    {
        // ItemSlot이 모두 가득 차면 Highlight 활성화
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            weaponSlots[i].UpdateItemSlot();
        }
    }

    public void WeaponDragHighlight()
    {
        // Weapon 드래그 시 Item 슬롯은 모두 하이라이트 키고 Weapon 슬롯은 하이라이트 끄기
        foreach(WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.WeaponDragHighlight();
        }
    }

    public void ItemDragHighlight()
    {
        // Item 드래그 시 Weapon 슬롯은 모두 하이라이트 키고 Item 슬롯은 하이라이트 끄기
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.ItemDragHighlight();
        }
    }

    public void CloseAllHighlight()
    {
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.CloseAllHighlight();
        }
    }

    public void HighlightByWeaponIndex(int index)
    {
        CloseAllHighlight();
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            if(weaponSlot.Index == index)
            {
                weaponSlot.AllHighlight();
                return;
            }
        }
    }

    public void CloseLevelUpUI()
    {
        // Level UI 비활성화
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.CloseLvUI();
        }
    }

    public void UpdateLeveUpUI()
    {
        // 장착되어 있는 슬롯 Level UI 활성화
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.UpdateLvUI();
        }
    }

    public void UpdateWeaponLevel(WeaponBase weapon)
    {
        // Weapon LevelUp 증가
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.UpdateWeaponLevelUp(weapon);
        }
    }

    public void UpdateReinforcementCostItemSlot()
    {
        // 아이템 강화 비용 키기
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.UpdateReinforcementCostItemSlot();
        }
    }

    public void ResetReinforcementCostItemSlot()
    {
        // 아이템 강화 비용 끄기
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.ResetReinforcementCostItemSlot();
        }
    }

    /// <summary>
    /// 무기 모두 경험치 증가
    /// </summary>
    /// <param name="value"></param>
    public void GainExpAllWeapon(float value)
    {
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            weaponSlot.ResetReinforcementCostItemSlot();
        }
    }

    /// <summary>
    /// 무기 모두 레벨 증가
    /// </summary>
    public void LevelUpAllWeapon()
    {

    }

    // 슬롯 옆으로 책자처럼 삐져나오게 하기
    public void HighlightInteractBG(int index)
    {
        for(int i=0; i<weaponSlots.Count; i++)
        {
            if (!weaponSlots[i].IsHighlightBG)
                weaponSlots[i].HighlightInteractBG(false);
            else
                Debug.Log($"{i}번째 슬롯은 클릭되어서 안넘어감");
        }

        weaponSlots[index].HighlightInteractBG(true);
    }

    public void SetHighlightInteract(int index)
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            weaponSlots[i].SetInteractBGValue(false);
        }

        weaponSlots[index].SetInteractBGValue(true);

        HighlightInteractBG(index);
    }

    public void HighlightOffInteractBG(int index)
    {
        if (!weaponSlots[index].IsHighlightBG)
            weaponSlots[index].HighlightInteractBG(false);
    }

    public void HighlightResetInteractBG(int index)
    {
        weaponSlots[index].SetInteractBGValue(false);
        weaponSlots[index].HighlightInteractBG(false);
    }

    public void HighlightAllResetInteractBG()
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            weaponSlots[i].SetInteractBGValue(false);
            weaponSlots[i].HighlightInteractBG(false);
        }
    }


    #endregion

    #region 애니메이션 연출

    public void StartSpreadOut()
    {
        SetPlayerSlotWindow(true);
        foreach (var slot in weaponSlots)
        {
            slot.gameObject.SetActive(true);
            slot.StartSpreadOutIncludeItem(weaponSlotSpreadDuration, itemSlotSpreadDuration);
        }
    }

    public void StartSpreadIn()
    {
        if (slotWindow.gameObject.activeSelf == false) return;

        foreach (var slot in weaponSlots)
        {
            slot.StartSpreadInIncludeItem(weaponSlotSpreadDuration, itemSlotSpreadDuration);
        }
    }

    #endregion

    #region 프로퍼티
    public List<WeaponSlot> WeaponSlots => weaponSlots;
    public PlayerSlotWindow PlayerSlotWindow => slotWindow;
    #endregion
}
