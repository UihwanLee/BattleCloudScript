using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ItemDataTable : MonoBehaviour
{
    [Header("아이템 List")]
    private List<int> itemDataTable = new List<int>();
    private Dictionary<Tier, List<ItemData>> itemTierDataDict;

    private void Start()
    {
        InitItemDataTable();

        GameManager.Instance.ItemDataTable = this;
    }

    private void InitItemDataTable()
    {
        // 데이터 가져오기
        itemTierDataDict = DataManager.ItemTierDataDict;

        // 데이터 설정
        foreach (var tierPair in itemTierDataDict)
        {
            List<ItemData> itemList = tierPair.Value;

            foreach (var itemData in itemList)
            {
                int id = itemData.ID;

                itemDataTable.Add(id);
            }
        }
    }

    public Item GetRandomItem()
    {
        int randomIndex;
        int randomItemId;
        do
        {
            randomIndex = Random.Range(0, itemDataTable.Count);
            randomItemId = itemDataTable[randomIndex];
        } while (CheckExclusiveItem(randomItemId));

        Item item;
        item = new Item();
        item.SetItem(randomItemId);

        return item;
    }

    public Item GetRandomItemByTier()
    {
        int day = GameManager.Instance.DayManager.CurrentDay;
        Tier itemTier = GameManager.Instance.GameRule.GetTierByDay(day);
        int randomIndex;
        do
        {
            randomIndex = itemTierDataDict[itemTier][Random.Range(0, itemTierDataDict[itemTier].Count)].ID;
        } while (CheckExclusiveItem(randomIndex));

        Item item;
        item = new Item();
        item.SetItem(randomIndex);

        return item;
    }

    public Item GetRandomItem(int idx)
    {
        Item item;
        int radnomID = itemDataTable[idx];
        item = new Item();
        item.SetItem(radnomID);

        return item;
    }

    private bool CheckExclusiveItem(int id)
    {
        // 전용 ID 값이 아닐경우 false 반환
        if (id < Define.ITEM_EXCLUSIVE_ID)
        {
            return false;
        }

        string effect = DataManager.ExclusiveItemDataDict[id].EFFECT;
        return !RecordManager.Instance.CanWeapon(effect);
    }

    #region 프로퍼티

    public List<int> ItemTable { get { return itemDataTable; } }

    #endregion
}
