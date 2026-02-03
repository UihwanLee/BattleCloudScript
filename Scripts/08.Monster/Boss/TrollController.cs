using System.Linq;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class TrollController : BaseController
{
    [Header("여기부터 세팅")]
    [Header("이동 속도")]
    [SerializeField] float moveSpeed;

    [Header("패턴 - 실행 주기(초)")]
    [SerializeField] private float patternCooldown;
    private float nextPatternTime;
    [Header("패턴 - 전진 속도")]
    [SerializeField] private float dashSpeed;
    [Header("패턴 - 전진 근접 공격 데미지")]
    [SerializeField] float meleeDamage;

    private bool isDashing;
    //private Vector2 dashDirection;
    private float dashEndTime;

    [Header("원거리 공격 - 실행 주기(초)")]
    [SerializeField] private float rangedCooldown;
    [Header("원거리 공격 - 사거리")]
    [SerializeField] private float rangedRange;
    [Header("원거리 공격 - 투사체 퍼짐 각도")]
    [SerializeField] private float angle;
    [Header("원거리 공격 - 투사체 데미지")]
    [SerializeField] float rangedDamage;

    private float nextRangedTime;

    [Header("근접 공격 콜라이더 - 변경X")]
    [SerializeField] private MeleeAttackCollider meleeCollider;

    private bool isDead;

    private Player player;
    private ProjectilePoolManager poolManager;

    private Animator animator;

    private BehaviorTree tree;
    private bool movementEnabled = true;
    private float currentDashSpeed;

    #region 프로퍼티
    public float PatternTime => patternCooldown;
    public bool IsPatternReady => Time.time >= nextPatternTime;
    public bool IsInPattern { get; set; }
    public float RangedRange => rangedRange;
    public bool IsRangedAttackReady => Time.time >= nextRangedTime;
    public bool MeleeAttackFinished { get; set; }

    public WindUpAction WindUpAction { get; private set; }
    public DashAttackAction DashAttackAction { get; private set; }
    public bool WindUpFinished { get; set; }
    public bool DashAttackFinished { get; set; }

    public Player Player => player;
    public Animator Animator => animator;
    #endregion

    #region Life Cycle
    protected override void Reset()
    {
        patternCooldown = 10f;
        nextPatternTime = patternCooldown;

        isDashing = false;
        moveDirection = Vector2.zero;
        dashSpeed = 10f;
        dashEndTime = 0f;

        rangedCooldown = 3f;
        rangedRange = 6f;
        nextRangedTime = rangedCooldown;
        angle = 15f;

        moveSpeed = 2f;
        //baseSpeed = moveSpeed;
        meleeCollider = transform.FindChild<MeleeAttackCollider>("MeleeCollider");

        meleeDamage = 5f;
        rangedDamage = 3f;
    }

    protected override void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _collider.enabled = true;
    }

    protected override void Start()
    {
        tree = CreateBehaviorTree();
        poolManager = ProjectilePoolManager.Instance;

        base.Start();
    }

    protected override void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        if (isDead)
            return;

        if (player == null)
            player = GameManager.Instance.Player;

        tree?.Tick();

        if (isDashing)
        {
            UpdateDash();
        }
    }
    #endregion

    #region Behavior Tree
    private BehaviorTree CreateBehaviorTree()
    {
        return new BehaviorTree(
            new Selector(
                CreatePatternSequence(),
                CreateChasePattern()
                )
            );
    }

    private Node CreatePatternSequence()
    {
        WindUpAction = new WindUpAction(this);
        DashAttackAction = new DashAttackAction(this);

        return new Sequence(
            new CheckPatternCooldown(this),
            WindUpAction,
            DashAttackAction,
            new GroggyAction(this, 3f)
            );
    }

    private Node CreateChasePattern()
    {
        return new Selector(
            new Sequence(
                new CheckRangedAttackCooldown(this),
                new DetectPlayer(this),
                new CheckPlayerInRange(this, rangedRange),
                new RangedAttackAction(this)
                ),
            new Sequence(
                new DetectPlayer(this),
                new MoveToPlayer(this)
                )
            ); ;
    }
    #endregion

    #region Animation Event
    public void OnWindUpFinished()
    {
        WindUpFinished = true;
    }

    public void OnDashAttackFinished()
    {
        DashAttackFinished = true;
        meleeCollider.EndAttack();
    }

    public void OnMeleeAttackFinished()
    {
        MeleeAttackFinished = true;
    }

    public void OnGroggyFinished()
    {
        animator.Play("Idle", 0, 0f);
    }
    #endregion

    #region Movement
    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    public void MoveTowardsPlayer()
    {
        if (!movementEnabled)
            return;

        moveDirection = (player.transform.position - transform.position).normalized;
        Vector2 moveDelta = moveDirection * currentSpeed * Time.deltaTime;

        if (moveDelta.x != 0)
        {
            Vector2 moveX = new Vector2(moveDelta.x, 0f);

            if (!IsBlocked(moveX))
                transform.position += (Vector3)moveX;
        }

        if (moveDelta.y != 0)
        {
            Vector2 moveY = new Vector2(0, moveDelta.y);

            if (!IsBlocked(moveY))
                transform.position += (Vector3)moveY;
        }

        animator.Play("Walk");
    }

    private void UpdateDash()
    {
        Vector2 moveDelta = moveDirection * dashSpeed * Time.deltaTime;
        
        if (moveDelta.x != 0)
        {
            Vector2 moveX = new Vector2(moveDelta.x, 0f);

            if (!IsBlocked(moveX))
                transform.position += (Vector3)moveX;
        }

        if (moveDelta.y != 0)
        {
            Vector2 moveY = new Vector2(0, moveDelta.y);

            if (!IsBlocked(moveY))
                transform.position += (Vector3)moveY;
        }

        if (Time.time >= dashEndTime)
        {
            StopDashAttack();
        }
    }
    #endregion

    #region Cooldown
    public void ConsumePatternCooldown()
    {
        nextPatternTime = Time.time + patternCooldown;
    }

    public void ConsumeRangedCooldown()
    {
        nextRangedTime = Time.time + rangedCooldown;
    }
    #endregion

    public void FacePlayer()
    {
        Vector2 dir = (player.transform.position - transform.position).normalized;

        if (dir.x < 0)
            spriteRenderer.flipX = true;
        else if (dir.x > 0)
            spriteRenderer.flipX = false;
    }

    public void StartDashAttack()
    {
        moveDirection = (player.transform.position - transform.position).normalized;

        isDashing = true;

        AnimationClip clip = animator.runtimeAnimatorController
            .animationClips
            .First(c => c.name == "Attack");

        dashEndTime = Time.time + clip.length;

        SetMovementEnabled(false);
    }

    public void StopDashAttack()
    {
        isDashing = false;
    }

    public void ThrowBonesWithMeleeAttack()
    {
        meleeCollider.gameObject.SetActive(true);
        meleeCollider.StartAttack();

        Vector2 upDir = new Vector2(-moveDirection.y, moveDirection.x);
        Vector2 downDir = -upDir;

        ThrowBone(upDir, 2f);
        ThrowBone(downDir, 2f);
    }

    private void ThrowBone(Vector2 direction, float speed)
    {
        MonsterProjectile bone = poolManager.GetProjectile(ProjectileType.MonsterBone).GetComponent<MonsterProjectile>();
        bone.transform.position = transform.position;

        float newAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bone.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

        bone.Initialize(15f, rangedDamage, speed);
    }

    public void RangedAttack()
    {
        Vector2 dir = (player.transform.position - transform.position).normalized;

        ThrowBone(Rotate(dir, angle), 10f);
        ThrowBone(Rotate(dir, 0), 10f);
        ThrowBone(Rotate(dir, -angle), 10f);
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
            ).normalized;
    }

    //protected bool IsBlocked(Vector2 move)
    //{
    //    float distance = move.magnitude + skinWidth;

    //    RaycastHit2D hit = Physics2D.BoxCast(
    //        transform.position,
    //        colliderSize,
    //        0f,
    //        move.normalized,
    //        distance,
    //        wallMask
    //        );

    //    return hit.collider != null;
    //}

    public new void Death()
    {
        if (isDead) return;

        isDead = true;

        tree = null;
        SetMovementEnabled(false);

        _collider.enabled = false;
        slowIcon.gameObject.SetActive(false);

        animator.Play("Dead");
    }

    protected override void CalculateSpeed()
    {
        float speed = moveSpeed;
        float newDashSpeed = dashSpeed;

        foreach (float ratio in slowSources.Values)
        {
            speed *= (1f - ratio);
            newDashSpeed *= (1f - ratio);
        }

        // 최소 속도 제한
        currentSpeed = Mathf.Max(speed, 0.1f);
        currentDashSpeed = Mathf.Max(newDashSpeed, 0.1f);
    }
}
