using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class Item : ISlotItem
{
    [SerializeField] private int id;
    [SerializeField] private string itmeName;
    [SerializeField] private string desc;
    [SerializeField] private Sprite image;
    [SerializeField] private List<Effect> effects;
    [SerializeField] private Attribute lv;
    [SerializeField] private ItemTraitType traitType = ItemTraitType.Base;
    [SerializeField] private List<IWeaponEffectInstance> effectInstances;
    [SerializeField] private float price;
    [SerializeField] private Tier tier;
    [SerializeField] private ItemApplyTarget applyTarget;

    private float additionalPrice;            // 누적 강화 비용

    private bool isOnEquipped = false;
    private int temp_index = -1;

    public void SetItem(int id)
    {
        if (DataManager.Instance == null) Debug.Log("Data 초기화 안됨");

        // ItemData 가져오기
        ItemData data = DataManager.Get<ItemData>(id);

        if (data == null) { Debug.Log("ItemData가 없습니다!"); return; }

        this.id = data.ID;
        this.itmeName = data.NAME;
        this.desc = data.DESC;

        // 효과 지정
        TryInitEffect(data);

        // 레벨 지정
        lv = new Attribute(0, 1, 1f);

        // 가격 지정
        this.price = data.PRICE;

        // Sprite 초기화
        this.image = DataManager.GetImage(data.IMAGE);

        // Tier 지정
        this.tier = DataManager.GetTier(data.TIER);

        this.applyTarget = DataManager.GetItemApplyTarget(data.APPLYTARGET);

        InitEffectInstances();
    }

    // 효과지정 시 OnHit, OnKill 같은 특정 이벤트는 따로 처리할 수 있도록 하는 메서드
    private void TryInitEffect(ItemData data)
    {
        string[] splits = data.TRIGGER.Split('|');

        // TRIGGER 첫번째 속성에 따른 분리
        if (splits[0].Contains("ON_EQUIPPED"))
        {
            isOnEquipped = true;
            InitEffects(data);
        }
        else
        {
            isOnEquipped = false;
            InitOtherEffects(data);
        }    
    }

    private void InitEffectInstances()
    {
        effectInstances = new List<IWeaponEffectInstance>();

        if (effects == null) return;

        foreach (Effect effect in effects)
        {
            var instance = WeaponEffectFactory.Create(effect, id);

            if (instance != null)
                effectInstances.Add(instance);
        }
    }

    private void InitEffects(ItemData data)
    {
        effects = new List<Effect>();

        ItemEffectType effectType;
        ItemTriggerType triggerType;
        ItemApplyTarget applyTarget;
        ItemApplyType applyType;
        float value;
        float value_multiplier;

        string[] splits = data.EFFECT.Split('|');

        // effectType 개수만큼 Effect 생성
        for (int i = 0; i < splits.Length; i++)
        {
            Effect effect = new Effect();
            effects.Add(effect);
        }

        // EffectType 지정
        for (int i = 0; i < splits.Length; i++)
        {
            effectType = GetEffectType(splits[i]);
            effects[i].SetEffectType(effectType);

            // 만약 전용 조언인 경우 전용 조언 속성 추가
            if (IsExclusive()) traitType = ItemTraitType.Exclusive;
        }

        // TriggerType 지정
        splits = data.TRIGGER.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            triggerType = GetTriggerType(splits[i]);
            effects[i].SetTriggerType(triggerType);
        }

        // ApplyTarget 지정
        splits = data.APPLYTARGET.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            applyTarget = GetAppltTarget(splits[i]);
            effects[i].SetApplyTarget(applyTarget);
        }

        // ApplyType 지정
        splits = data.APPLYTYPE.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            if(i < effects.Count)
            {
                effects[i].ResetValue();
                applyType = GetAppltType(splits[i]);
                effects[i].SetApplyType(applyType);
            }
        }

        // Value 지정
        splits = data.VALUE.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            if(float.TryParse(splits[i], out value))
            {
                if(i < effects.Count)
                {
                    effects[i].SetValue(value);
                }
            }
        }

        // Value Multiplier 지정
        splits = data.VALUE_MUTIPLIER.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            if (float.TryParse(splits[i], out value_multiplier))
            {
                if (i < effects.Count)
                    effects[i].SetValueMultlier(value_multiplier);
            }
        }
    }

    private void InitOtherEffects(ItemData data)
    {
        effects = new List<Effect>();

        ItemEffectType effectType;
        ItemTriggerType triggerType;
        ItemApplyTarget applyTarget;
        ItemApplyType applyType;
        float value;
        float value_multiplier;

        string[] splits = data.EFFECT.Split('|');

        // effectType 개수만큼 Effect 생성
        for (int i = 0; i < splits.Length; i++)
        {
            Effect effect = new Effect();
            effects.Add(effect);
        }

        // EffectType 지정
        for (int i = 0; i < splits.Length; i++)
        {
            effectType = GetEffectType(splits[i]);
            effects[i].SetEffectType(effectType);
        }

        // TriggerType 지정
        splits = data.TRIGGER.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            triggerType = GetTriggerType(splits[i]);
            effects[i].SetTriggerType(triggerType);
        }

        // ApplyTarget 지정
        splits = data.APPLYTARGET.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            applyTarget = GetAppltTarget(splits[i]);
            effects[i].SetApplyTarget(applyTarget);
        }

        // ApplyType 지정
        splits = data.APPLYTYPE.Split('|');
        effects[0].ResetValue();
        for (int i = 0; i < splits.Length; i++)
        {
            applyType = GetAppltType(splits[i]);
            effects[0].SetApplyType(applyType);
        }

        // Value 지정
        splits = data.VALUE.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            if (float.TryParse(splits[i], out value))
            {
                // 한 곳에만 지정
                effects[0].SetValue(value);
            }
        }

        // Value Multiplier 지정
        splits = data.VALUE_MUTIPLIER.Split('|');
        for (int i = 0; i < splits.Length; i++)
        {
            if (float.TryParse(splits[i], out value_multiplier))
            {
                effects[0].SetValueMultlier(value_multiplier);
            }
        }
    }

    public void ApplyItem(Player player, bool add, int index)
    {
        if (effects == null) return;

        temp_index = index;

        //foreach(var effect in effects)
        //{
        //    effect.Apply(player, add, index);
        //}

        if (add)
            player.WeaponManager.GetWeapon(index)?.EffectController.Equip(this);
        else
            player.WeaponManager.GetWeapon(index)?.EffectController.Unequip(this);
    }

    /// <summary>
    /// 조언 레벨 업 시 효과 증가
    /// </summary>
    public void ApplyItemMultiplier(bool isEquip)
    {
        if (effects == null) return;

        // 끼고 있는 상태이면 바로 증가 전 빼기
        if(isEquip) ApplyItem(GameManager.Instance.Player, false, temp_index);

        foreach (var effect in effects)
        {
            effect.ApplyMultiplier();
        }

        // 끼고 있는 상태이면 증가 후 효과 적용
        if(isEquip) ApplyItem(GameManager.Instance.Player, true, temp_index);
    }

    public Item Clone()
    {
        Item copy = new Item();
        // 필요한 모든 필드를 복사
        copy.id = this.id;
        copy.itmeName = this.itmeName;
        copy.desc = this.desc;
        copy.image = this.image;
        copy.effects = this.effects;
        copy.lv = this.lv;
        return copy;
    }

    /// <summary>
    /// 전용 조언의 경우 Weapon 타입과 일치 하는지 반환
    /// </summary>
    /// <returns></returns>
    public bool IsItemEffectMatchWithWeapon(WeaponType type)
    {
        if (effects.Count <= 0) return false;

        return effects[0].CheckExclusiveWeaponMatch(type);
    }

    /// <summary>
    /// 전용 조언이 맞는지 체크하는 함수
    /// </summary>
    /// <returns></returns>
    public bool IsExclusive()
    {
        if (effects.Count <= 0) return false;

        return effects[0].CheckExclusive();
    }

    #region ISlotItem 인터페이스 구현
    public void Load(int _id)
    {
        SetItem(_id);
    }

    public string GetName()
    {
        return itmeName;
    }

    public string GetDesc()
    {
        return desc;
    }

    public Sprite GetIcon()
    {
        return image;
    }

    public GameObject GetPrefab()
    {
        return null;
    }

    public List<float> GetValueList(int length)
    {
        List<float> list = new List<float>();

        if(isOnEquipped)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < effects.Count)
                {
                    list.Add(effects[i].Value);
                }
                else
                {
                    list.Add(0f);
                }
            }

            return list;
        }
        else
        {
            List<float> valueList = effects[0].ValueList;
            for (int i = 0; i < length; i++)
            {
                if (i < valueList.Count)
                {
                    list.Add(valueList[i]);
                }
                else
                {
                    list.Add(0f);
                }
            }

            return list;
        }
    }

    public List<float> GetNextValueList(int length)
    {
        List<float> list = new List<float>();

        if (isOnEquipped) {
            for (int i = 0; i < length; i++) {
                if (i < effects.Count)  list.Add(effects[i].Value + effects[i].ValueMultiplier);
                else list.Add(0f);
            }

            return list;
        }
        else
        {
            List<float> valueList = effects[0].ValueList;
            List<float> valueMultiList = effects[0].ValueMultiList;
            for (int i = 0; i < length; i++) {
                if (i < valueList.Count) list.Add(valueList[i] + valueMultiList[i]);
                else list.Add(0f);
            }

            return list;
        }
    }

    public List<ItemApplyType> GetValueTypeList(int length)
    {
        List<ItemApplyType> list = new List<ItemApplyType>();

        if (isOnEquipped)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < effects.Count)
                {
                    list.Add(effects[i].ApplyType);
                }
                else
                {
                    list.Add(ItemApplyType.UNDEF);
                }
            }

            return list;
        }
        else
        {
            List<ItemApplyType> valueList = effects[0].ApplyTypeList;
            for (int i = 0; i < length; i++)
            {
                if (i < valueList.Count)
                {
                    list.Add(valueList[i]);
                }
                else
                {
                    list.Add(0f);
                }
            }

            return list;
        }
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


    public void SetPrice(float price)
    {
        this.price = price;
    }

    public void AddLv(float value, bool isEquip)
    {
        lv.Add(value);
        ApplyItemMultiplier(isEquip);
    }

    public void InitLv(float value)
    {
        lv.Set(value);

        ApplyLvInEffect();
    }

    private void ApplyLvInEffect()
    {
        // 적용된 레벨 만큼 효과 증대
        for(int i=0; i < lv.Value; i++)
        {
            foreach (var effect in effects)
            {
                effect.ApplyMultiplier();
            }
        }
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

    DropItemType ISlotItem.GetType()
    {
        if (traitType == ItemTraitType.Exclusive) return DropItemType.ExclusiveItem;
        return DropItemType.Item;
    }


    #endregion

    #region 타입 파싱

    private ItemEffectType GetEffectType(string value)
    {
        if (Enum.TryParse<ItemEffectType>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 ItemEffectType가 지정되지 않았습니다");
        return ItemEffectType.UNDEF;
    }

    private ItemTriggerType GetTriggerType(string value)
    {
        if (Enum.TryParse<ItemTriggerType>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 ItemTriggerType가 지정되지 않았습니다");
        return ItemTriggerType.UNDEF;
    }

    private ItemApplyTarget GetAppltTarget(string value)
    {
        if (Enum.TryParse<ItemApplyTarget>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 ItemApplyTarget가 지정되지 않았습니다");
        return ItemApplyTarget.UNDEF;
    }

    private ItemApplyType GetAppltType(string value)
    {
        if (Enum.TryParse<ItemApplyType>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 ItemApplyTarget가 지정되지 않았습니다");
        return ItemApplyType.UNDEF;
    }

    #endregion

    #region 프로퍼티

    public int ID { get { return id; } }
    public string Name { get { return itmeName; } }
    public string Desc { get { return desc; } }
    public Attribute Lv { get { return lv; } }
    public float Price { get { return price; } }
    public Sprite Image { get { return image; } }
    public List<Effect> Effects { get { return effects; } }
    public ItemTraitType ItemTraitType { get { return traitType; } }
    public List<IWeaponEffectInstance> EffectInstances { get { return effectInstances; } }
    public Tier Tier { get { return tier; } }

    #endregion
}
