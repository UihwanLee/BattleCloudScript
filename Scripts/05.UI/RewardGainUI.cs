using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardGainUI : MonoBehaviour
{
    [Header("리소스")]
    [SerializeField] private int maxSlotCount = 4;
    [SerializeField] private GameObject productPrefab;

    [Header("컴포넌트")]
    [SerializeField] private Transform prefabTransform;
    [SerializeField] private GameObject rewardGainWindow;

    private List<ProductSlot> gainItemList = new List<ProductSlot>();
    private int currentActiveSlot;

    private void Awake()
    {
        InitSlot();
    }

    private void OnEnable()
    {
        EventBus.OnCheckDropItem += CheckGainDropItem;
    }

    private void OnDisable()
    {
        EventBus.OnCheckDropItem -= CheckGainDropItem;
    }

    private void InitSlot()
    {
        currentActiveSlot = 0;

        for (int i = 0; i < maxSlotCount; i++)
        {
            GameObject go = Instantiate(productPrefab, prefabTransform);
            if (go.TryGetComponent<ProductSlot>(out ProductSlot slot))
            {
                gainItemList.Add(slot);
            }
            go.SetActive(false);
        }
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

        currentActiveSlot = items.Count;

        rewardGainWindow.SetActive(true);

        // 인벤토리에 바로 들어가기 전 Slot으로 뿌리기
        foreach (var item in items)
        {
            ProductSlot slot = GetActiveSlot();
            slot.InitSlot();
            slot.SetProduct(item);
            slot.RemoveGold();
            slot.SelectBtn.onClick.RemoveAllListeners();
            slot.SelectBtn.onClick.AddListener(() => OnClickItem());
        }
    }

    private ProductSlot GetActiveSlot()
    {
        foreach (var slot in gainItemList)
        {
            if (slot.gameObject.activeSelf) continue;

            slot.gameObject.SetActive(true);
            return slot;
        }

        // 다 돌았는데 없다면 새로 생성해서 보내기
        GameObject go = Instantiate(productPrefab, prefabTransform);
        if (go.TryGetComponent<ProductSlot>(out ProductSlot newSlot))
        {
            gainItemList.Add(newSlot);
            return newSlot;
        }

        return null;
    }

    public void OnClickItem()
    {
        ProductSlot currentSlot = GetTopProductSlot();

        // 다 가져갔는지 체크
        if (currentSlot == null)
        {
            return;
        }

        var effect = currentSlot.GetComponent<ProductSlotEffect>();

        effect.Play(() =>
        {
            // 임시 인벤토리에 추가
            GameManager.Instance.TemporaryInventory.AddItemFromShopOrReward(currentSlot.Item);

            // 클릭한 지우기
            currentSlot.gameObject.SetActive(false);

            currentActiveSlot--;

            // 다 가져갔는지 체크
            if (currentActiveSlot <= 0)
            {
                rewardGainWindow.SetActive(false);
                return;
            }
        });
    }

    private ProductSlot GetTopProductSlot()
    {
        for(int i= gainItemList.Count - 1; i >= 0; i--)
        {
            if( gainItemList[i].gameObject.activeSelf) return gainItemList[i];
        }
        return null;
    }
}
