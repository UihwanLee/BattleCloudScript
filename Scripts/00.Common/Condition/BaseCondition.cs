using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// 기본 캐릭터에서 사용할 BaseCondition 클래스
// Condition에서 관리할 Atrribute는 다음과 같다.
//   maxHp
//   hp
public class BaseCondition : MonoBehaviour, IDamageable
{
    [SerializeField] protected StatAttribute maxHp;
    [SerializeField] protected StatAttribute hp;

    protected Dictionary<AttributeType, StatAttribute> conditionDict = new Dictionary<AttributeType, StatAttribute>();

    public virtual void InitCondition(float _maxHp)
    {
        maxHp = new StatAttribute(0, _maxHp, 0f);
        hp = new StatAttribute(1, _maxHp, 0f);

        conditionDict.Clear();
        conditionDict.Add(AttributeType.MaxHp, maxHp);
        conditionDict.Add(AttributeType.Hp, hp);
    }

    public virtual void Add(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Add(amount);
        }
    }

    public virtual void Sub(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Sub(amount);
        }
    }

    public virtual void Set(AttributeType type, float value)
    {
        if (conditionDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Set(value);
        }
    }

    public void ResetValue()
    {
        foreach (var attribute in conditionDict.Values)
        {
            attribute.ResetValue();
        }
    }

    public virtual void AddMultiplier(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.AddMultiplier(amount);
        }
    }

    public void ResetMultiplier()
    {
        foreach (var attribute in conditionDict.Values)
        {
            attribute.ResetMultiplier();
        }
    }

    public virtual void TakeDamage(Transform other, float damage, Color? color = null)
    {
        //damage 적용
        Sub(AttributeType.Hp, damage);

        FloatingTextPoolManager.Instance.SpawnText(FloatingTextType.NormalDamage.ToString(), damage.ToString("0"), other.transform, color);

        if (hp.Value <= 0)
        {
            // death
        }
    }

    /// <summary>
    /// 일반적인 데미지 호출
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="color">기본값 = 흰색, 색 변경이 필요할 경우 기입</param>
    public virtual void TakeDamage(float damage, WeaponBase weaponBase = null, Color? color = null)
    {
        //damage 적용
        Sub(AttributeType.Hp, damage);

        FloatingTextPoolManager.Instance.SpawnText(FloatingTextType.NormalDamage.ToString(), damage.ToString("0"), transform, color);

        if (hp.Value <= 0)
        {
            // death
        }
    }

    #region
    public Attribute MaxHp => maxHp;
    public Attribute Hp => hp;

    public Transform Transform => transform;

    public Collider2D Collider => GetComponent<Collider2D>();
    #endregion
}
