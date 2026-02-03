using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class MonsterCondition : BaseCondition, IKillable
{
    private Monster monster;
    private PoolManager poolManager;
    private GameManager gameManager;

    private float hp_increase_rate;
    private float baseMaxHp;
    [SerializeField] float gold;


    #region Property

    public bool IsDead { get; set; }
    public float Gold => gold;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        monster = GetComponent<Monster>();
    }

    private void OnEnable()
    {
        // 풀 재사용 시: 기본 MaxHp로 먼저 원복한 뒤 현재 HP도 맞춤
        if (maxHp != null)
        {
            float restored = maxHp.Value;
            Set(AttributeType.MaxHp, restored);
            Set(AttributeType.Hp, restored);
        }

        IsDead = false;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        poolManager = PoolManager.Instance;
    }

    #endregion

    #region 초기화

    public void InitMonsterCondition(float _maxHp, float _hp_increase_rate)
    {
        hp_increase_rate = _hp_increase_rate;

        // 기본 maxHp(페이즈 배율 적용 전)
        baseMaxHp = _maxHp * hp_increase_rate;

        // InitCondition은 딱 1번만
        InitCondition(baseMaxHp);
    }

    public void AdditionalGoldByDay(float gold)
    {
        this.gold = gold;
    }

    #endregion

    #region 피격 처리

    public override void TakeDamage(float damage, WeaponBase owner = null, Color? color = null)
    {
        // 이미 죽은 몬스터면 차단
        if (IsDead)
            return;

        float usefulDamage = Mathf.Min(hp.Value, damage);
        hp.Set(hp.Value - damage);

        EventBus.OnTakeDamageByWeapon?.Invoke(owner, usefulDamage);

        // 효과음
        AudioManager.Instance.PlayClip(SFXType.MonsterHit);

        FloatingTextPoolManager.Instance.SpawnText(
            FloatingTextType.NormalDamage.ToString(),
            damage.ToString("0"),
            transform,
            color
        );

        monster.VisualEffect.PlayHitEffect();
        GameManager.Instance.ParticleManager.PlayHitEffect(transform);
        if (hp.Value <= 0f)
        {
            Die();
        }
    }

    #endregion


    #region 사망 처리

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        monster.Controller.SetEnabledCollider(false);
        EventBus.OnMonsterKilled?.Invoke(1f);

        // 드랍 호출
        MonsterDropHandler dropHandler = GetComponent<MonsterDropHandler>();
        if (dropHandler != null)
            dropHandler.Drop();

        monster.VisualEffect.PlayDeathEffect(() =>
            {
                gameManager.spawnManager.UnregisterMonster(monster);
                poolManager.ReleaseObject(monster.PoolKey.PoolKey, gameObject);
            }
        );
    }

    #endregion
}
