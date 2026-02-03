using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// WeaponCondition 클래스
// BaseCondition은 HP를 받고 있으므로 상속하지 않는다.
public class WeaponCondition : MonoBehaviour
{
    [SerializeField] private Attribute lv;                      // 레벨
    [SerializeField] private Attribute maxExp;                  // 최대 경험치
    [SerializeField] private Attribute exp;                     // 현재 경험치

    private float exp_coefficient;                              // 레벨업계수
    private WeaponType weaponType;                              // 타입

    private WeaponBase weapon;

    protected Dictionary<AttributeType, Attribute> conditionDict = new Dictionary<AttributeType, Attribute>();

    [Header("Level Up Effect")]
    [SerializeField] private LevelUpEffectController levelUpBack;
    [SerializeField] private LevelUpEffectController levelUpFront;

    private void Reset()
    {
        levelUpBack = transform.FindChild<LevelUpEffectController>("LevelUp Back");
        levelUpFront = transform.FindChild<LevelUpEffectController>("LevelUp Front");
    }

    public void InitCondition(float _lv, float _maxExp, float _exp_cofficient, WeaponType type)
    {
        lv = new Attribute(0, _lv, 1f);
        maxExp = new Attribute(1, _maxExp, 0f);
        exp = new Attribute(2, 0, 0f);

        conditionDict.Add(AttributeType.Lv, lv);
        conditionDict.Add(AttributeType.MaxExp, maxExp);
        conditionDict.Add(AttributeType.Exp, exp);

        exp_coefficient = _exp_cofficient;
        weaponType = type;

        weapon = GetComponent<WeaponBase>();

        ApplyLv();
    }

    private void ApplyLv()
    {
        Debug.Log("레벨 스탯 적용: " + lv.Value);
        // 레벨 별 스탯 적용
        for(int i=1; i<lv.Value; i++)
        {
            Debug.Log("레벨 스탯 적용: [" + i + "]");
            ApplyStatByLv(lv);
        }

        EventBus.OnWeaponLevelUp?.Invoke((int)lv.Value, weapon);
    }

    public void Add(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out Attribute attribute))
        {
            attribute.Add(amount);

            // Exp 체크
            if (type == AttributeType.Exp)
            {
                // 만약 경험치가 모두 찼을 경우 레벨업
                if (attribute.Value >= maxExp.Value)
                {
                    //if (lv.Value >= Define.MAX_WEAPONLEVEL) return;

                    //// 경험치 초기화
                    //attribute.Set(0f);
                    //lv.Add(1);

                    //EventBus.OnWeaponLevelUp?.Invoke((int)lv.Value, weapon);

                    //// Max 경험치 증가
                    //maxExp.Add(exp_coefficient);

                    //// 레벨 별 스탯 적용
                    //ApplyStatByLv(lv);

                    //// 레벨업 올랐다는 것을 전달
                    //GameManager.Instance.Player.PlayerSlotUI.UpdateWeaponLevel(weapon);

                    //levelUpBack.Play();
                    //levelUpFront.Play();
                }
            }
        }
    }

    private void ApplyStatByLv(Attribute _lv)
    {
        // Lv별 증가 스탯 가져오기
        float nextValue = GameManager.Instance.GameRule.GetWeaponLvIncreaseDamageStatByType(weaponType, _lv.Value);
        float nextInterval = GameManager.Instance.GameRule.GetWeaponLvIncreaseAttackIntervalStatByType(weaponType, _lv.Value);

        Debug.Log("증가값?: " + nextValue);
        // 레벨 스탯 적용
        // 최대 레벨일 때는 고정 값 적용
        weapon.Stat.Set(AttributeType.Damage, nextValue);
        Debug.Log($"레벨 {lv.Value} 달성 공격력 고정값 {nextValue} 픽스");
        weapon.Stat.Set(AttributeType.AttackInterval, nextInterval);
        Debug.Log($"레벨 {lv.Value} 달성 공격 속도 고정값 {nextInterval} 픽스");
    }

    public void Sub(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out Attribute attribute))
        {
            attribute.Sub(amount);
        }
    }

    public void Set(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out Attribute attribute))
        {
            attribute.Set(amount);
        }
    }

    #region 프로퍼티
    public Dictionary<AttributeType, Attribute> ConditionDict { get { return conditionDict; } }
    public Attribute Lv => lv;
    public Attribute MaxExp => maxExp;
    public Attribute Exp => exp;
    public WeaponType WeaponType => weaponType;
    #endregion
}
