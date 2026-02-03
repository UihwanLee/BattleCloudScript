
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public interface ISlotItem
{
    public void Load(int _id);
    public string GetName();
    public string GetDesc();
    public Sprite GetIcon();
    public GameObject GetPrefab();
    public List<float> GetValueList(int length);
    public List<float> GetNextValueList(int length);
    public List<ItemApplyType> GetValueTypeList(int length);    
    public int GetID();
    public Attribute GetLv();
    public float GetPrice();
    public Tier GetTier();
    public Color GetTierColor();
    public void AddAdditionalPrice(float amount);
    public float GetAdditionalPrice();
    public void SetPrice(float price);
    public void AddLv(float value, bool isEquip);
    public void InitLv(float value);
    public void SetLv(float value);
    public DropItemType GetType();
}
