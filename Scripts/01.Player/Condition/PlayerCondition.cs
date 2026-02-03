using System.Collections;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class PlayerCondition : BaseCondition
{
    [SerializeField] private StatAttribute lv;
    [SerializeField] private StatAttribute maxExp;
    [SerializeField] private StatAttribute exp;
    [SerializeField] private StatAttribute gold;

    [Header("레벨업 보상 UI 띄우는 속도")]
    [SerializeField] private float levelUpRewardOpenDuration = 1.0f;

    [Header("Hp Bar")]
    [SerializeField] PlayerHpBar hpBar;

    private float maxhp_coefficient;                               // 최대 체력 계수
    private float exp_coefficient;                              // 레벨업계수
    private float lv_stat_coefficient;                          // 레벨업 당 STAT 올라가는 계수

    // 필요한 데이터 -> Json 데이터 작업 요구
    private bool isInvincible = false;
    private float invincibleTime = 0.5f;
    private float lastDamagedTime = 0f;

    [Header("무적")]
    [SerializeField] private bool invincibility;

    [Header("Heal Effect")]
    [SerializeField] private HealEffectController healEffectBack;
    [SerializeField] private HealEffectController healEffectFront;

    [Header("Level Up Effect")]
    [SerializeField] private LevelUpEffectController levelUpEffect;

    private Player player;

    private bool isDead = false;
    private bool isLock = false;

    private float prevLv = 1;               // 이전 참조할 수 있는 레벨 데이터

    private void Reset()
    {
        healEffectBack = transform.FindChild<HealEffectController>("HealEffect Front");
        healEffectFront = transform.FindChild<HealEffectController>("HealEffect Front");
        levelUpEffect = transform.FindChild<LevelUpEffectController>("LevelUp Effect");
    }

    private void OnEnable()
    {
        EventBus.SetHUDHealthLock += SetHpLock;
    }

    private void OnDisable()
    {
        EventBus.SetHUDHealthLock -= SetHpLock;
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        if (invincibility) return;

        if (isInvincible)
        {
            lastDamagedTime += Time.deltaTime;
            if (lastDamagedTime >= invincibleTime)
            {
                isInvincible = false;
            }
        }
    }

    public void InitCondition(float _lv, float _maxHp, float _maxHp_cofficient, float _maxExp, float _exp_cofficient, float _lv_stat_cofficient)
    {
        InitCondition(_maxHp);

        lv = new StatAttribute(2, _lv, 1f);
        maxExp = new StatAttribute(3, _maxExp, 0f);
        exp = new StatAttribute(4, 0, 0f);
        gold = new StatAttribute(5, 0, 0f);

        conditionDict.Add(AttributeType.Lv, lv);
        conditionDict.Add(AttributeType.MaxExp, maxExp);
        conditionDict.Add(AttributeType.Exp, exp);
        conditionDict.Add(AttributeType.Gold, gold);

        maxhp_coefficient = _maxHp_cofficient;
        exp_coefficient = _exp_cofficient;
        lv_stat_coefficient = _lv_stat_cofficient;

        // 초기화 한 후 HUD 세팅
        EventBus.Publish(AttributeType.Hp, hp.Value);
        EventBus.Publish(AttributeType.Exp, exp.Value);
    }

    public override void Add(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Add(amount);

            // Hp가 최대 Hp를 초과하지 않도록 설정
            if (type == AttributeType.Hp)
            {
                if (attribute.Value >= maxHp.Value)
                {
                    attribute.Set(maxHp.Value);
                    attribute.ResetModifier();
                }
            }

            // Exp 체크
            if (type == AttributeType.Exp)
            {
                // 만약 경험치가 모두 찼을 경우 레벨업
                if (attribute.Value >= maxExp.Value)
                {
                    Add(AttributeType.MaxHp, 1);
                    Add(AttributeType.Hp, 1);

                    Add(AttributeType.Lv, 1);
                    levelUpEffect.Play();
                    // 경험치 초기화
                    attribute.Sub(MaxExp.Value);

                    // UI 갱신
                    EventBus.OnPlayerLevelUp(lv.Value);

                    // GameRule에서 Day별 경험치 계수를 가져와서 Max_Exp 증가
                    Add(AttributeType.MaxExp, GameManager.Instance.Player.exp_amount);
                }
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLevelUp);
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public override void Sub(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Sub(amount);

            // Hp 타입 체크
            if (type == AttributeType.Hp)
            {
                // Hp가 0 이하일 때 사망
                if (attribute.Value <= 0 && maxHp.Value > 0)
                {
                    isDead = true;
                    Set(AttributeType.Hp, 0);
                    EventBus.OnPlayerMoveStop?.Invoke();
                    GameManager.Instance.Player.AnimationHandler.OnDead();
                }
            }

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public override void Set(AttributeType type, float amount)
    {
        if (conditionDict.TryGetValue(type, out StatAttribute attribute))
        {
            attribute.Set(amount);

            // UI event Invoke
            if (!EventBus.Publish(type, attribute.Value))
            {
                Debug.Log($"{type}에 연결된 이벤트가 없습니다.");
            }
        }
    }

    public void CaculationEXP()
    {
        // 정산
        int rewardCount = (int)lv.Value - (int)prevLv;

        Debug.Log($"이번 웨이브에서는 [Lv.{prevLv}]->[Lv.{lv.Value}] 갱신");
        Debug.Log($"총 {rewardCount}번 만큼 레벨업 보상을 얻게 됩니다.");
        Debug.Log($"현재 경험치 정보: [{exp.Value}/{maxExp.Value}]");
        Debug.Log($"현재 경험치 정보: [{exp.Value}/{maxExp.Value}]");

        // 레벨업 보상 소환
        GameManager.Instance.LevelUpRewardUI.OpenLevelUpReward(rewardCount);

        // prevLv 업데이트
        prevLv = lv.Value;
    }

    private IEnumerator StartLateRewardUI(int rewardCount)
    {
        yield return new WaitForSeconds(levelUpRewardOpenDuration);

        // 레벨업 보상 소환
        GameManager.Instance.LevelUpRewardUI.OpenLevelUpReward(rewardCount);

        // prevLv 업데이트
        prevLv = lv.Value;
    }


    public override void TakeDamage(float damage, WeaponBase weaponBase = null, Color? color = null)
    {
        if (isInvincible || isDead || invincibility) return;

        isInvincible = true;
        lastDamagedTime = 0f;

        if (TryEvade())
        {
            FloatingTextPoolManager.Instance.SpawnText(
                FloatingTextType.Miss.ToString(),
                "Miss",
                transform,
                color);
            return;
        }

        Color c = (Color)((color == null) ? Color.red : color);

        float finalDamage = CalculateDefenseDamage(damage);

        base.TakeDamage(finalDamage, weaponBase, c);

        if (player == null)
            player = GameManager.Instance.Player;

        player.VisualEffect.PlayHitEffect();

        hpBar.OnHit(isLock);

        GameManager.Instance.ParticleManager.PlayHitEffect(transform);
        AudioManager.Instance.PlayClip(SFXType.PlayerHit);
    }

    // 회피 적용
    private bool TryEvade()
    {
        float rand = Random.Range(0f, 100f);
        return rand < GameManager.Instance.Player.Stat.Evasion.Value;
    }

    // 방어 적용
    private float CalculateDefenseDamage(float damage)
    {
        float reduction = Mathf.Clamp01(GameManager.Instance.Player.Stat.Defense.Value / 100f);
        return damage * (1f - reduction);
    }

    public void PlayHealEffect()
    {
        healEffectBack.Play();
        healEffectFront.Play();
    }

    public void SetHpLock(bool _lock)
    {
        this.isLock = _lock;
    }

    #region 프로퍼티

    public Attribute Lv => lv;
    public Attribute Gold => gold;
    public Attribute MaxExp => maxExp;
    public Attribute Exp => exp;
    public bool IsDead => isDead;
    #endregion
}
