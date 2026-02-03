using System.Threading;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class Monster : MonoBehaviour
{
    [SerializeField] private MonsterController controller;
    [SerializeField] private MonsterStat stat;
    [SerializeField] private MonsterCondition condition;
    [SerializeField] private MonsterVisualEffect visualEffect;
    [SerializeField] private MonsterAnimationHandler animationHandler;
    [SerializeField] private MonsterPoolKey poolKey;
    [SerializeField] private MonsterType type;

    public MonsterController Controller => controller;
    public MonsterStat Stat => stat;    
    public MonsterCondition Condition => condition;
    public MonsterVisualEffect VisualEffect => visualEffect;
    public MonsterAnimationHandler AnimationHandler => animationHandler;
    public MonsterPoolKey PoolKey => poolKey;
    public MonsterType Type => type;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<MonsterController>();

        if (stat == null)
            stat = GetComponent<MonsterStat>();

        if (condition == null)
            condition = GetComponent<MonsterCondition>();

        if (visualEffect == null)
            visualEffect = GetComponentInChildren<MonsterVisualEffect>();

        if (animationHandler == null)
            animationHandler = GetComponentInChildren<MonsterAnimationHandler>();

        if (poolKey == null)
            poolKey = GetComponent<MonsterPoolKey>();
    }

    public void AdditionalStatByDay(float finalDamage, float finaleHp)
    {
        // 하루에 한번 적용되도록 설정
        Stat.ResetValue();
        Condition.ResetValue();

        // Day 별 추가 적용
        Stat.Set(AttributeType.Damage, finalDamage);
        Condition.Set(AttributeType.MaxHp, finaleHp);

        // 체력 적용
        Condition.Set(AttributeType.Hp, Condition.MaxHp.Value);
    }
}
