using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDItem : MonoBehaviour
{
    [Header("HUDItem Slot Prefab")]
    [SerializeField] private string key = "HUDItem";
    [SerializeField] private int maxSlotCount = 4;
    [SerializeField] private GameObject slotPrefab;

    [Header("얻은 아이템 리스트")]
    [SerializeField] private List<HUDItemSlot> slotItems = new List<HUDItemSlot>();

    private int currentActiveSlot;

    private void Awake()
    {
        InitSlot();
    }

    private void OnEnable()
    {
        EventBus.OnGainItem += OnGainItem;
        EventBus.OnPhaseDone += OnPhaseDone;
        EventBus.OnDayChanged += OnDayDone;
    }

    private void OnDisable()
    {
        EventBus.OnGainItem -= OnGainItem;
        EventBus.OnPhaseDone -= OnPhaseDone;
        EventBus.OnDayChanged -= OnDayDone;
    }

    private void InitSlot()
    {
        currentActiveSlot = 0;

        for(int i=0; i<maxSlotCount; i++)
        {
            GameObject go = Instantiate(slotPrefab, transform);
            if(go.TryGetComponent<HUDItemSlot>(out HUDItemSlot slot))
            {
                slotItems.Add(slot);
            }
            go.SetActive(false);
        }
    }

    public void OnGainItem(ISlotItem item)
    {
        // 아이템 얻을 시 Slot 만들고 갱신
        HUDItemSlot slot = GetActiveSlot();
        slot.Set(item);
    }

    private HUDItemSlot GetActiveSlot()
    {
        foreach(var slot in slotItems)
        {
            if (slot.gameObject.activeSelf) continue;

            slot.gameObject.SetActive(true);
            return slot;
        }

        // 다 돌았는데 없다면 새로 생성해서 보내기
        GameObject go = Instantiate(slotPrefab, transform);
        if (go.TryGetComponent<HUDItemSlot>(out HUDItemSlot newSlot))
        {
            slotItems.Add(newSlot);
            return newSlot;
        }

        return null;
    }

    private void ResetSlot()
    {
        foreach (var slot in slotItems)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void OnPhaseDone()
    {
        // 페이즈 정산
        EventBus.OnCheckDropItem?.Invoke(slotItems);
    }

    public void OnDayDone(int day)
    {
        // 이상현상 정산
        EventBus.OnCheckDropItem?.Invoke(slotItems);
    }

    #region 프로퍼티

    public List<HUDItemSlot> HUDItemSlots { get { return slotItems; } }

    #endregion
}
