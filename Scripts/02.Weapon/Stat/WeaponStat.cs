using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class WeaponStat : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] private int id;
    [SerializeField] private StatAttribute attack;
    [SerializeField] private float damage;
    [SerializeField] private StatAttribute criticalChancePercent;
    [SerializeField] private StatAttribute criticalMultiplier;
    [SerializeField] private StatAttribute range;
    [SerializeField] private StatAttribute extent;
    [SerializeField] private StatAttribute knockBack;
    [SerializeField] private StatAttribute moveSpeed;
    [SerializeField] private StatAttribute attackInterval;

    // Stat Attribute Dictionary
    private Dictionary<AttributeType, StatAttribute> attributeDict;

    private WeaponBase weapon;

    private void Awake()
    {
        weapon = GetComponent<WeaponBase>();
    }

    private void Start()
    {
        //Initialize();
    }

    public void InitStat(int ID_STAT)
    {
        // WeaponStatData 가져오기
        WeaponStatData data = DataManager.Get<WeaponStatData>(ID_STAT);

        if (data == null) { Debug.Log("WeaponStatData가 없습니다!"); return; }

        id = data.ID;
        attack = new StatAttribute(0, data.BASE_DAMAGE, 1);
        range = new StatAttribute(1, data.RANGE, 0);
        extent = new StatAttribute(2, data.EXTENT, 0);
        knockBack = new StatAttribute(3, data.KNOCKBACK, 0);
        moveSpeed = new StatAttribute(4, data.MOVE_SPEED, 0.1f);
        attackInterval = new StatAttribute(5, data.ATTACK_INTERVAL, 0.05f);

        attributeDict = new Dictionary<AttributeType, StatAttribute>();
        attributeDict.Add(AttributeType.Damage, attack);
        attributeDict.Add(AttributeType.CriticalChancePercent, criticalChancePercent);
        attributeDict.Add(AttributeType.CriticalMultiplier, criticalMultiplier);
        attributeDict.Add(AttributeType.Range, range);
        attributeDict.Add(AttributeType.Extent, extent);
        attributeDict.Add(AttributeType.KnockBack, knockBack);
        attributeDict.Add(AttributeType.MoveSpeed, moveSpeed);
        attributeDict.Add(AttributeType.AttackInterval, attackInterval);

        CalculateDamage();
    }

    public void LoadStat(int ID_STAT)
    {
        // WeaponStatData 가져오기
        WeaponStatData data = DataManager.Get<WeaponStatData>(ID_STAT);

        if (data == null) { Debug.Log("WeaponStatData가 없습니다!"); return; }

        id = data.ID;

        // 로드 한 스탯 적용
        Set(AttributeType.Damage, data.BASE_DAMAGE);
        Set(AttributeType.Range, data.RANGE);
        Set(AttributeType.Extent, data.EXTENT);
        Set(AttributeType.KnockBack, data.KNOCKBACK);
        Set(AttributeType.MoveSpeed, data.MOVE_SPEED);
        Set(AttributeType.AttackInterval, data.ATTACK_INTERVAL);
    }

    public void Add(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Add(amount);

            if (type == AttributeType.Damage)
            {
                CalculateDamage();
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void Sub(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Sub(amount);

            if (type == AttributeType.Damage)
            {
                CalculateDamage();
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void Set(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Set(amount);

            if (type == AttributeType.Damage)
            {
                CalculateDamage();
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void SetFixedValue(AttributeType type, float amount)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.SetFixedValue(amount);

            if (type == AttributeType.Damage)
            {
                CalculateDamage();
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void DisableFixedValue(AttributeType type)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.DisableFixedValue();

            if (type == AttributeType.Damage)
            {
                CalculateDamage();
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void AddMultiplier(AttributeType type, float value)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.AddMultiplier(value);

            if (type == AttributeType.Damage)
            {
                CalculateDamage();
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void RemoveMultiplier(AttributeType type, float value)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.RemoveMultiplier(value);

            if (type == AttributeType.Damage)
            {
                CalculateDamage();
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void ResetModifier(AttributeType type)
    {
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.ResetModifier();

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public float Get(AttributeType type)
    {
        float value = 0;
        if (attributeDict.TryGetValue(type, out StatAttribute attribute))
        {
            value = attribute.Value;
        }
        return value;
    }

    private void CalculateDamage()
    {
        float itemValue = Attack.Additive + (Attack.BaseValue * (Attack.Multiplier - 1));
        if (weapon == null)
            weapon = GetComponent<WeaponBase>();
        damage = Attack.BaseValue + weapon.Trait.Scale * itemValue;
    }

    #region 프로퍼티

    public Dictionary<AttributeType, StatAttribute> AttributeDict { get { return attributeDict; } }
    public int ID { get { return id; } }
    public StatAttribute Attack { get { return attack; } }
    public float Damage { get { return damage; } }
    public StatAttribute CriticalChancePercent { get { return criticalChancePercent; } }
    public StatAttribute CriticalMultiplier { get { return criticalMultiplier; } }
    public StatAttribute Range { get { return range; } }
    public StatAttribute KnockBack { get { return knockBack; } }
    public StatAttribute MoveSpeed { get { return moveSpeed; } } 
    public StatAttribute AttackInterval { get { return attackInterval; } }

    #endregion
}
