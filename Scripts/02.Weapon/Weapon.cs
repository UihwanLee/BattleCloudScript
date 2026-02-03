using System;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class Weapon : ISlotItem
{
    [SerializeField] private int id;
    [SerializeField] private string weaponName;
    [SerializeField] private string desc;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int id_stat;
    [SerializeField] private Attribute lv;
    [SerializeField] private WeaponType type;
    [SerializeField] private float price;
    [SerializeField] private Tier tier;
    [SerializeField] private float scale;
    [SerializeField] private ItemApplyTarget applyTarget;

    private float additionalPrice;            // 누적 강화 비용

    private float damage;
    private float attackInterval;

    public void SetWeapon(int id)
    {
        if (DataManager.Instance == null) Debug.Log("Data 초기화 안됨");

        // ItemData 가져오기
        WeaponTraitData data = DataManager.Get<WeaponTraitData>(id);

        if (data == null) { Debug.Log("WeaponData가 없습니다!"); return; }

        this.id = data.ID;
        this.weaponName = data.NAME;
        this.desc = data.DESC;
        this.prefab = DataManager.GetPrefab(data.PREFAB);
        this.id_stat = data.ID_STAT;
        this.type = DataManager.GetWeaponType(data.TYPE);
        this.price = data.PRICE;
        this.tier = DataManager.GetTier(data.TIER);
        this.scale = data.SCALE;
        this.applyTarget = DataManager.GetItemApplyTarget(data.ItemApplyTarget);

        // WeaponStatData 가져오기
        WeaponStatData statData = DataManager.Get<WeaponStatData>(data.ID_STAT);
        damage = statData.BASE_DAMAGE;
        attackInterval = statData.ATTACK_INTERVAL;

        lv = new Attribute(0, 1, 1);
    }

    public Weapon Clone()
    {
        Weapon copy = new Weapon();
        // 필요한 모든 필드를 복사
        copy.id = this.id;
        copy.weaponName = this.weaponName;
        copy.desc = this.desc;
        copy.prefab = this.prefab;
        copy.id_stat = this.id_stat;
        copy.lv = this.lv;
        copy.type = this.type;
        copy.price = this.price;
        return copy;
    }

    public void ApplyLv()
    {
        damage = GameManager.Instance.GameRule.GetWeaponLvIncreaseDamageStatByType(type, lv.Value);
        attackInterval = GameManager.Instance.GameRule.GetWeaponLvIncreaseAttackIntervalStatByType(type, lv.Value);
    }

    #region ISlotItem 인터페이스 구현

    public void Load(int _id)
    {
        SetWeapon(_id);
    }

    public string GetName()
    {
        return weaponName;
    }

    public string GetDesc()
    {
        return desc;
    }

    public Sprite GetIcon()
    {
        if (prefab == null) Debug.Log("prefab이 없음");
        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        return spriteRenderer.sprite;
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public int GetID()
    {
        return id;
    }

    public Attribute GetLv()
    {
        return lv;
    }
    public float GetPrice()
    {
        return price;
    }

    public void SetPrice(float price)
    {
        this.price = price;
    }

    public Tier GetTier()
    {
        return tier;
    }

    public Color GetTierColor()
    {
        return tier switch
        {
            Tier.COMMAN => Define.TIER_COLOR_COMMON,
            Tier.RARE => Define.TIER_COLOR_RARE,
            Tier.EPIC => Define.TIER_COLOR_EPIC,
            Tier.LEGEND => Define.TIER_COLOR_LEGEND,
            Tier.EXCLUSIVE => Define.TIER_COLOR_EXCLUSIVE,
            _ => Define.TIER_COLOR_COMMON,
        };
    }

    public void AddLv(float value, bool isEquip)
    {
        lv.Add(value);
        ApplyLv();
    }

    public void InitLv(float value)
    {
        lv.Set(value);
    }

    public void SetLv(float value)
    {
        lv.Set(value);
    }

    public void AddAdditionalPrice(float amount)
    {
        this.additionalPrice += amount;
    }

    public float GetAdditionalPrice()
    {
        return additionalPrice;
    }

    public List<float> GetValueList(int length)
    {
        List<float> list = new List<float>();

        for(int i=0; i<length; i++)
        {
            list.Add(0f);
        }

        return list;
    }

    public List<float> GetNextValueList(int length)
    {
        List<float> list = new List<float>();

        for (int i = 0; i < length; i++)
        {
            list.Add(0f);
        }

        return list;
    }

    public List<ItemApplyType> GetValueTypeList(int length)
    {
        List<ItemApplyType> list = new List<ItemApplyType>();

        for (int i = 0; i < length; i++)
        {
            list.Add(ItemApplyType.UNDEF);
        }

        return list;
    }

    DropItemType ISlotItem.GetType()
    {
        return DropItemType.Weapon;
    }

    #endregion

    #region 프로퍼티

    public WeaponType Type { get { return type; } }
    public Attribute Lv { get {  return lv; } }
    public float Price { get { return price; } }
    public float AdditionalPrice { get { return additionalPrice; } }
    public Tier Tier { get { return tier; } }
    public float Damage { get { return damage; } }
    public float AttackInterval { get { return attackInterval; } }
    public float Scale { get { return scale; } }

    #endregion
}
