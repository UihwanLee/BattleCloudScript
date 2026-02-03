using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class MonsterStat : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private int id;
    [SerializeField] private StatAttribute moveSpeed;
    [SerializeField] private StatAttribute damage;
    [SerializeField] private StatAttribute attackInterval;
    [SerializeField] private StatAttribute range;
    [SerializeField] private StatAttribute knockBackResist;
    [SerializeField] private string specialTag;
    [SerializeField] private StatAttribute specialValue;
    [SerializeField] private StatAttribute specialWeight;
    [SerializeField] private string onHitEffect;

    // Stat Attribute Dictionary
    private Dictionary<AttributeType, StatAttribute> attributeDict;

    private void Start()
    {
        //Initialize();
    }

    public void InitStat(int ID_STAT)
    {
        // MonsterStatData 가져오기
        MonsterStatData data = DataManager.Get<MonsterStatData>(ID_STAT);

        if (data == null) { Debug.Log("MonsterStatData 없습니다!"); return; }

        id = data.ID;
        moveSpeed = new StatAttribute(0, data.MOVE_SPEED, 0.1f);
        damage = new StatAttribute(1, data.BASE_DAMAGE, 1);
        attackInterval = new StatAttribute(2, data.ATTACK_INTERVAL, 0.01f);
        range = new StatAttribute(3, data.RANGE, 0);
        knockBackResist = new StatAttribute(4, data.KNOCKBACK_RESIST, 0);
        specialTag = data.SPECIAL_TAG;
        specialValue = new StatAttribute(5, data.SPECIAL_VALUE, 0);
        specialWeight = new StatAttribute(6, data.SPECIAL_WEIGHT, 0);
        onHitEffect = data.ON_HIT_EFFECT;

        attributeDict = new Dictionary<AttributeType, StatAttribute>();
        attributeDict.Add(AttributeType.MoveSpeed, moveSpeed);
        attributeDict.Add(AttributeType.Damage, damage);
        attributeDict.Add(AttributeType.AttackInterval, attackInterval);
        attributeDict.Add(AttributeType.Range, range);
        attributeDict.Add(AttributeType.KnockBackResist, knockBackResist);
        attributeDict.Add(AttributeType.SpecialValue, specialValue);
        attributeDict.Add(AttributeType.SpecialWeight, specialWeight);
    }

    public void LoadStat(int ID_STAT)
    {
        // MonsterStatData 가져오기
        MonsterStatData data = DataManager.Get<MonsterStatData>(ID_STAT);

        if (data == null) { Debug.Log("MonsterStatData 없습니다!"); return; }

        id = data.ID;
        specialTag = data.SPECIAL_TAG;
        onHitEffect = data.ON_HIT_EFFECT;

        // 로드 한 스탯 적용
        Set(AttributeType.MoveSpeed, data.MOVE_SPEED);
        Set(AttributeType.Damage, data.BASE_DAMAGE);
        Set(AttributeType.AttackInterval, data.ATTACK_INTERVAL);
        Set(AttributeType.Range, data.RANGE);
        Set(AttributeType.KnockBackResist, data.KNOCKBACK_RESIST);
        Set(AttributeType.SpecialValue, data.SPECIAL_VALUE);
        Set(AttributeType.SpecialWeight, data.SPECIAL_WEIGHT);
    }

    public void Add(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Add(amount);
        }
    }

    public void Sub(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Sub(amount);
        }
    }

    public void Set(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Set(amount);
        }
    }

    public void AddMultiplier(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.AddMultiplier(amount);
        }
    }

    public void ResetValue()
    {
        foreach (var attribute in attributeDict.Values)
        {
            attribute.ResetValue();
        }
    }

    public void ResetMultiplier()
    {
        foreach(var attribute in attributeDict.Values)
        {
            attribute.ResetMultiplier();
        }
    }

    #region 프로퍼티

    public int ID { get { return id; } }
    public Attribute Damage { get { return damage; } }
    public Attribute MoveSpeed { get { return moveSpeed; } }
    public Attribute Range { get { return range; } }
    public Attribute KnockBackResist { get { return knockBackResist; } }
    public Attribute AttackInterval { get { return attackInterval; } }
    public Attribute SpecialValue { get { return specialValue; } }
    public Attribute SpecialWeight { get { return specialWeight; } }
    public string SpecialTag { get { return specialTag; } }
    public string OnHitEffect { get { return onHitEffect; } }

    #endregion
}
