using System.Collections;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public enum DespawnReason
{
    Death,      // 공격으로 죽음
    PhaseEnd    // 전투 페이즈 종료
}

public class OldMonsterController : BaseController
{
    #region 참조

    protected Transform player;
    public Transform Player => player;

    protected Monster monster;
    public Monster Monster => monster;

    private SpriteRenderer sr;

    private MaterialPropertyBlock mpb;

    [Header("풀 키")]
    protected MonsterPoolKey poolKeyComponent;

    protected Animator animator;

    private float gainGold;

    #endregion

    #region 공격

    protected float attackTimer = 0f;
    protected bool isAttacking = false;
    #endregion

    #region 상태

    protected bool isDead = false;
    private bool isSpawnProtected = false;
    protected bool isDespawning = false;
    private bool justSpawned = false;
    public bool IsSpawnProtected => isSpawnProtected;
    public bool IsDespawning => isDespawning;
    private bool isEndingHandled = false;
    private Coroutine forceDespawnCoroutine;

    #endregion

    #region 생명주기

    protected override void Awake()
    {
        base.Awake();

        monster = GetComponent<Monster>();
        poolKeyComponent = GetComponent<MonsterPoolKey>();

        // Animator 캐싱
        animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    protected override void Start()
    {
        baseSpeed = monster.Stat.MoveSpeed.Value;

        base.Start();

        if (GameManager.Instance != null && GameManager.Instance.Player != null)
            player = GameManager.Instance.Player.transform;
    }

    protected virtual void OnEnable()
    {
        justSpawned = true;

        isDead = false;

        // EndPhase / ForceDespawn 플래그 복구
        isDespawning = false;
        isEndingHandled = false;

        attackTimer = 0f;
        isAttacking = false; // 반드시초기화

        moveDirection = Vector2.zero;

        // EndPhase에서 멈춘 애니메이터 복구
        if (animator != null)
            animator.speed = 1f;

        // ForceDespawn에서 꺼둔 콜라이더 복구
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = true;

        if (sr != null)
        {
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat("_FadeAmount", 0f);
            sr.SetPropertyBlock(mpb);
        }

        //InitializeMoveLogic();

        StartCoroutine(DelayVisualEnable());
    }

    private IEnumerator DelayVisualEnable()
    {
        if (sr == null)
            yield break;

        sr.enabled = false;   // 렌더 차단
        yield return null;    // 한 프레임
        sr.enabled = true;    // 그 다음 프레임부터 표시
    }

    protected override void Update()
    {
        if (justSpawned)
        {
            justSpawned = false;
            return;
        }

        if (isDead || isDespawning)
            return;

        // 게임이 RUNNING 아닐 때는 몬스터 로직 전부 중단
        if (GameManager.Instance != null && GameManager.Instance.IsEndingPhase)
            return;

        if (player == null)
        {
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
                player = GameManager.Instance.Player.transform;
        }

        if (player == null)
            return;

        base.Update();

        attackTimer -= Time.deltaTime;

        UpdateMoveAnimation();
    }
    #endregion

    #region 이동
    protected override void Move()
    {
        float dist = Vector2.Distance(
            transform.position,
            Player.transform.position
            );

        if (dist < 0.5f)
            return;

        moveDirection = (player.transform.position - transform.position).normalized;

        base.Move();
    }
    #endregion

    #region 이동 애니메이션
    /// <summary>
    /// Idle ↔ Run 애니메이션 제어
    /// </summary>
    protected void UpdateMoveAnimation()
    {
        if (animator == null)
            return;

        bool isMoving = false;

        isMoving = moveDirection.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);
    }

    public void PlayAttackTelegraph()
    {
        MonsterAttackTelegraph telegraph = GetComponentInChildren<MonsterAttackTelegraph>();

        if (telegraph == null)
            return;

        if (spriteRenderer != null)
            telegraph.Bind(spriteRenderer);

        telegraph.Play();
    }
    #endregion

    #region 공격
    public void TryAttack()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        if (player == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > monster.Stat.Range.Value)
            return;

        if (isAttacking || attackTimer > 0f)
            return;

        isAttacking = true;
        attackTimer = monster.Stat.AttackInterval.Value;
        animator?.SetTrigger("Attack");
    }

    protected virtual void Attack()
    {
        if (player == null)
            return;

        PlayerCondition pc = player.GetComponent<PlayerCondition>();
        if (pc == null)
            return;

        pc.TakeDamage(monster.Stat.Damage.Value);
    }

    /// <summary>
    /// Attack 애니메이션 이벤트에서 호출
    /// </summary>
    public void OnAttackEvent()
    {
        if (isDead || isDespawning)
            return;

        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        if (player == null)
        {
            isAttacking = false;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        float range = monster.Stat.Range.Value;

        // 사거리 밖이면 공격 취소
        if (dist > range)
        {
            isAttacking = false;
            return;
        }

        Attack();
        isAttacking = false;
    }

    public bool CanAttack()
    {
        return attackTimer <= 0f;
    }
    #endregion

    #region 이동 방향 제어 (MoveLogic → Controller)
    public void SetMoveDirection(Vector2 dir)
    {
        moveDirection = dir;
    }

    #endregion

    #region 스탯 / 사망
    public void SetSpawnProtected(bool value)
    {
        isSpawnProtected = value;
    }

    public virtual void RefreshStat()
    {
        knockbackRegistance = monster.Stat.KnockBackResist.Value;
        baseSpeed = monster.Stat.MoveSpeed.Value;
        slowSources.Clear();
        CalculateSpeed();
    }

    public void ResetValue()
    {

    }

    public void AdditionalStatByDay(float finalDamage, float finaleHp)
    {
        // 하루에 한번 적용되도록 설정

        monster.Stat.ResetValue();
        monster.Condition.ResetValue();

        // Day 별 추가 적용
        monster.Stat.Set(WhereAreYouLookinAt.Enum.AttributeType.Damage, finalDamage);
        monster.Condition.Set(WhereAreYouLookinAt.Enum.AttributeType.MaxHp, finaleHp);

        // 체력 적용
        monster.Condition.Set(WhereAreYouLookinAt.Enum.AttributeType.Hp, monster.Condition.MaxHp.Value);
    }

    public void AdditionalGoldByDay(float gold)
    {
        // Day 별 골드 적용
        gainGold = gold;
    }

    public void ResetAdditionalStatMultiplier()
    {
        monster.Stat.ResetMultiplier();
        monster.Condition.ResetMultiplier();
    }

    protected override void Death()
    {
        if (isDead || isDespawning)
            return;

        isDead = true;
        monster.Condition.IsDead = true;

        OldMonsterProjectile.ReleaseAllByOwner(transform);
        ResetAdditionalStatMultiplier();

        RequestDespawn(DespawnReason.Death);
    }

    private IEnumerator DespawnSequence(DespawnReason reason)
    {
        switch (reason)
        {
            case DespawnReason.Death:
                // 번쩍 + 회전 + 축소
                if (TryGetComponent<DeathEffectHandler>(out var deathEffect))
                    yield return deathEffect.PlayDeathEffect();
                break;

            case DespawnReason.PhaseEnd:
                // 페이드아웃
                yield return FadeOut();
                break;
        }

        // 풀 반환 직전 단 한 번만 리셋
        ResetVisual();

        GameManager.Instance?.spawnManager?.UnregisterMonster(monster);

        PoolManager.Instance.ReleaseObject(
            poolKeyComponent.PoolKey,
            gameObject
        );
    }

    public void ForceDespawn()
    {
        if (isDead || isDespawning)
            return;

        monster.Condition.IsDead = true;
        RequestDespawn(DespawnReason.PhaseEnd);
    }

    private void RequestDespawn(DespawnReason reason)
    {
        if (isDespawning)
            return;

        isDespawning = true;
        isDead = true;

        // 이동 / 공격 완전 차단
        moveDirection = Vector2.zero;
        attackTimer = float.MaxValue;

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        if (animator != null && reason == DespawnReason.PhaseEnd)
        {
            animator.speed = 0f;
            animator.Rebind();
            animator.Update(0f);
        }

        StopAllCoroutines();
        StartCoroutine(DespawnSequence(reason));
    }

    private IEnumerator FadeOut()
    {
        if (sr == null)
            yield break;

        float duration = 1.5f;
        float t = 0f;

        sr.GetPropertyBlock(mpb);
        mpb.SetFloat("_FadeAmount", 0f);
        sr.SetPropertyBlock(mpb);

        while (t < duration)
        {
            t += Time.deltaTime;
            float fade = Mathf.Lerp(0f, 1f, t / duration);

            sr.GetPropertyBlock(mpb);
            mpb.SetFloat("_FadeAmount", fade);
            sr.SetPropertyBlock(mpb);

            yield return null;
        }
    }
    private void ResetVisual()
    {
        if (sr != null)
        {
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat("_FadeAmount", 0f);
            sr.SetPropertyBlock(mpb);
        }
    }
    #endregion

    #region 프로퍼티

    public float GainGold { get { return gainGold; } }

    #endregion
}