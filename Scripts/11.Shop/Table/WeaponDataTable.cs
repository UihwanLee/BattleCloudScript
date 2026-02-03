using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class WeaponDataTable : MonoBehaviour
{
    [Header("조언자 List")]
    private List<int> weaponDataTable = new List<int>();
    private Dictionary<Tier, List<WeaponTraitData>> weaponTierDataDict;

    private void Start()
    {
        InitItemDataTable();

        GameManager.Instance.WeaponDataTable = this;
    }

    private void InitItemDataTable()
    {
        // 데이터 가져오기
        weaponTierDataDict = DataManager.WeaponTierDataDict;

        // 데이터 설정
        foreach (var tierPair in weaponTierDataDict)
        {
            List<WeaponTraitData> weaponList = tierPair.Value;

            foreach (var weaponData in weaponList)
            {
                int id = weaponData.ID;

                weaponDataTable.Add(id);
            }
        }
    }

    public Weapon GetRandomWeapon()
    {
        int randomIndex;
        int randomItemId;

        randomIndex = Random.Range(0, weaponDataTable.Count);
        randomItemId = weaponDataTable[randomIndex];

        Weapon weapon = new Weapon();
        weapon.SetWeapon(randomItemId);

        return weapon;
    }

    public Weapon GetRandomWeaponByTier()
    {
        int day = GameManager.Instance.DayManager.CurrentDay;
        Tier itemTier = GameManager.Instance.GameRule.GetTierByDay(day);
        int randomIndex;
        randomIndex = weaponTierDataDict[itemTier][Random.Range(0, weaponTierDataDict[itemTier].Count)].ID;

        Weapon weapon;
        weapon = new Weapon();
        weapon.SetWeapon(randomIndex);

        return weapon;
    }

    public Weapon GetRandomWeapon(int i)
    {
        int randomInex = Random.Range(0, weaponDataTable.Count);
        Weapon weapon = new Weapon();
        int randomID = weaponDataTable[i];
        weapon.SetWeapon(randomID);

        return weapon;
    }

    #region 프로퍼티

    public List<int> WeaponTable { get { return weaponDataTable; } }

    #endregion
}
