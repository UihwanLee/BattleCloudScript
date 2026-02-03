using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using WhereAreYouLookinAt.Enum;
using static UnityEditor.Progress;

public class TemporaryInventory : MonoBehaviour
{
    [Header("판매 가격 비율 (기본 25%)")]
    [SerializeField] private float saleRate;

    [Header("임시 인벤토리 슬롯")]
    [SerializeField] private List<TemporarySlot> temporarySlotList = new List<TemporarySlot>();

    private int currentSlotCount = 0;

    private void Start()
    {
        GameManager.Instance.TemporaryInventory = this;
    }

    private void OnEnable()
    {
        EventBus.OnCheckDropItem += CheckGainDropItem;
    }

    private void OnDisable()
    {
        EventBus.OnCheckDropItem -= CheckGainDropItem;
    }

    public void CheckGainDropItem(List<HUDItemSlot> list)
    {
        List<ISlotItem> items = new List<ISlotItem>();

        foreach (HUDItemSlot slot in list)
        {
            if (slot.gameObject.activeSelf)
            {
                items.Add(slot.Item);
                slot.gameObject.SetActive(false);
            }
        }

        if (items.Count <= 0)
        {
            // 없다면 Return
            return;
        }

        // 인벤토리에 바로 들어가기 전 Slot으로 뿌리기

        foreach(var item in items)
        {
            //AddItem(item);
        }
    }

    public void ClearAllSlot()
    {
        foreach (var slot in temporarySlotList)
        {
            if(slot.Item != null)
            {
                slot.DeleteItem(slot.Item, slot.Index);
            }
            slot.ResetLvUI();
            currentSlotCount = 0;
        }
    }

    #region 인벤토리 아이템 추가/삭제

    // 상점이나 레벨업 보상(보따리)로 얻은 경우 이펙트 추가
    public void AddItemFromShopOrReward(ISlotItem item)
    {
        TemporarySlot emptySlot = temporarySlotList.Find(slot => slot.Item == null);

        if (emptySlot != null)
        {
            // 비어있는 슬롯에 아이템 추가
            emptySlot.AddItem(item, emptySlot.Index, true);

            // 연출 추가
            emptySlot.PlayFlash();

            currentSlotCount++;

            UpdateLvUI();
        }
        else
        {
            // 인벤토리가 가득 찬 경우 -> 자동판매
            GameManager.Instance.Player.Condition.Add(AttributeType.Gold, item.GetPrice());
        }
    }

    public void AddItem(ISlotItem item)
    {
        TemporarySlot emptySlot = temporarySlotList.Find(slot => slot.Item == null);

        if (emptySlot != null)
        {
            // 비어있는 슬롯에 아이템 추가
            emptySlot.AddItem(item, emptySlot.Index, true);

            currentSlotCount++;

            UpdateLvUI();
        }
        else
        {
            // 인벤토리가 가득 찬 경우 -> 자동판매
            GameManager.Instance.Player.Condition.Add(AttributeType.Gold, item.GetPrice());
        }
    }

    public void DeleteItem(ISlotItem item)
    {
        for(int i=temporarySlotList.Count-1; i>=0; i--)
        {
            if(temporarySlotList[i].Item == item)
            {
                temporarySlotList[i].DeleteItem(temporarySlotList[i].Item, temporarySlotList[i].Index);

                // Slot Index 감소
                currentSlotCount = Mathf.Max(currentSlotCount - 1, 0);
            }
        }

        UpdateLvUI();
    }

    public void DeleteItem(TemporarySlot slot)
    {
        if (slot == null) return;

        // 해당 슬롯 아이템 지우기
        slot.DeleteItem(slot.Item, slot.Index);

        // Slot Index 감소
        currentSlotCount = Mathf.Max(currentSlotCount - 1, 0);

        UpdateLvUI();
    }

    public void UpdateLvUI()
    {
        foreach(var slot in temporarySlotList)
        {
            if(slot.Item != null)
            {
                slot.UpdateLv(slot.Item.GetLv());
                slot.UpdateLvUI();
            }
            else
            {
                slot.ResetLvUI();
            }
        }
    }

    public void UpdateInventorySlot()
    {
        // 인벤토리 슬롯 업데이트 : 인벤토리 UI 갱신
        List<TemporarySlot> activeSlot = new List<TemporarySlot>();

        // 인벤토리에 아이템이 저장되어 있는 슬롯 찾기
        for (int i = 0; i < temporarySlotList.Count; i++)
        {
            if (temporarySlotList[i].Item != null)
            {
                activeSlot.Add(temporarySlotList[i]);
            }
        }

        // 인벤토리 아이템들 앞으로 땡기기
        for (int i = 0; i < temporarySlotList.Count; i++)
        {
            int index = i;
            if (index < activeSlot.Count)
            {
                temporarySlotList[index].AddItem(activeSlot[index].Item, activeSlot[index].Index, false);
                temporarySlotList[index].UpdateLv(activeSlot[index].Item.GetLv());
                temporarySlotList[index].UpdateLvUI();
                currentSlotCount++;
            }
            else
            {
                temporarySlotList[index].ResetSlot();
                temporarySlotList[index].ResetLvUI();
            }
        }
    }

    #endregion

    // 아이템 판매
    public void SellItem(ISlotItem item)
    {
        // 아이템 최종 판매 가격 계산하기
        float basePirce = item.GetPrice();                  // 구매 가격
        float additionalPirce = item.GetAdditionalPrice();  // 누적 강화 비용
        float finalPrice = Mathf.Round((basePirce + additionalPirce) * saleRate);

        Debug.Log($"판매 최종 가격: ({basePirce} + {additionalPirce} * {saleRate} = {finalPrice}");

        GameManager.Instance.Player.Condition.Add(AttributeType.Gold, finalPrice);
        DeleteItem(item);
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UIItemSell, WhereAreYouLookinAt.Enum.SFXPlayType.Single);
    }

    public float GetFinalSellPrice(ISlotItem item)
    {
        // 아이템 최종 판매 가격 계산하기
        float basePirce = item.GetPrice();                  // 구매 가격
        float additionalPirce = item.GetAdditionalPrice();  // 누적 강화 비용
        float finalPrice = Mathf.Round((basePirce + additionalPirce) * saleRate);

        return finalPrice;
    }


    public void SellAllItems()
    {
        // 아이템 리스트에 들어있는 아이템 개수대로 판매금 갱신
        foreach (TemporarySlot slot in temporarySlotList)
        {
            if (slot.Item != null)
            {
                SellItem(slot.Item);
            }
        }

        ClearAllSlot();
    }
}
