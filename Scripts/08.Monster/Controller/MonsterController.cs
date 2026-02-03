using UnityEngine;
using WhereAreYouLookinAt.Enum;

public abstract class MonsterController : BaseController
{
    [Header("Target")]
    [SerializeField] protected Transform playerTr;

    protected GameManager gameManager;
    protected PoolManager poolManager;

    protected Monster monster;
    protected MonsterStat stat;

    [Header("플레이어 유지 거리")]
    [SerializeField] protected float stopDistance = 0.3f;
    [Header("겹침 방지 거리")]
    [SerializeField] protected float separationRadius = 0.6f;
    [Header("겹쳤을 때 밀어내는 힘")]
    [SerializeField] protected float separationForce = 0.8f;
    [SerializeField] protected LayerMask monsterMask;

    #region Property
    public Monster Monster => monster;
    public Transform PlayerTr => playerTr;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        monster = GetComponent<Monster>();
        stat = monster.Stat;

        if (GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            gameManager = GameManager.Instance;
            playerTr = GameManager.Instance.Player.transform;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetEnabledCollider(true);
    }

    protected override void Start()
    {
        base.Start();

        poolManager = PoolManager.Instance;
    }

    protected override void Update()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (playerTr == null)
            playerTr = gameManager.Player.transform;

        if (gameManager.State != GameState.RUNNING)
            return;

        if (monster.Condition.IsDead)
            return;

        UpdateAI();

        base.Update();
    }

    /// <summary>
    /// 몬스터 종류별 AI 구현
    /// </summary>
    protected abstract void UpdateAI();

    protected override void Flip()
    {
        if (spriteRenderer == null) return;

        Vector2 toPlayer = DirToPlayer();

        if (Mathf.Abs(toPlayer.x) < 0.01f)
            return;

        if (toPlayer.x < 0)
            spriteRenderer.flipX = true;
        else if (toPlayer.x > 0)
            spriteRenderer.flipX = false;
    }

    protected Vector2 DirToPlayer()
    {
        Vector2 toPlayer = playerTr.position - transform.position;

        return toPlayer.normalized;
    }

    /// <summary>
    /// 플레이어가 있는 방향을 구하는 메서드, 너무 가까우면 멈춤
    /// </summary>
    /// <returns>플레이어가 있는 방향</returns>
    protected Vector2 DirToPlayerWithStop()
    {
        Vector2 toPlayer = playerTr.position - transform.position;
        float dist = toPlayer.magnitude;

        if (dist <= stopDistance)
            return Vector2.zero;

        return toPlayer.normalized;
    }

    protected Vector2 CalculateSeparation()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            separationRadius,
            monsterMask
            );

        Vector2 force = Vector2.zero;

        foreach (var hit in hits)
        {
            if (hit.transform == transform)
                continue;

            if (hit.TryGetComponent<Monster>(out var mon))
            {
                if (mon.Type != monster.Type)
                    continue;
            }

            Vector2 dir = (Vector2)(transform.position - hit.transform.position);
            float dist = dir.magnitude;

            if (dist > 0f)
                force += dir.normalized / dist;
        }

        return force * separationForce;
    }

    protected Vector2 GetMoveDirection()
    {
        Vector2 toPlayer = DirToPlayerWithStop();
        Vector2 separation = CalculateSeparation();

        Vector2 result = toPlayer + separation;

        return result.sqrMagnitude > 0f ? result.normalized : Vector2.zero;
    }

    protected bool IsPlayerInRange(float range)
    {
        if (playerTr == null)
            return false;

        return Vector2.Distance(transform.position, playerTr.position) <= range;
    }

    public void Refresh()
    {
        knockbackRegistance = stat.KnockBackResist.Value;
        baseSpeed = stat.MoveSpeed.Value;
        slowSources.Clear();
        CalculateSpeed();
    }

    public virtual void Despawn()
    {
        slowIcon.gameObject.SetActive(false);
        SetEnabledCollider(false);

        monster.AnimationHandler.Pause();
        monster.Condition.IsDead = true;

        monster.VisualEffect.PlayFadeOut(() =>
        {
            gameManager.spawnManager.UnregisterMonster(monster);
            poolManager.ReleaseObject(monster.PoolKey.PoolKey, gameObject);
            monster.AnimationHandler.Play();
        }
        );
    }

    public void SetEnabledCollider(bool enabled)
    {
        _collider.enabled = enabled;
    }
}
