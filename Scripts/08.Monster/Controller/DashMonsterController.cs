using UnityEngine;

public class DashMonsterController : MonsterController
{
    private enum State
    {
        Chase,
        DashReady,
        Dash,
        Cooldown
    }

    [Header("대시 속도")]
    [SerializeField] private float dashSpeed = 10f;
    [Header("대시 준비 시간(빨간색 유지 시간)")]
    [SerializeField] private float dashReadyDuration = 0.4f;
    [Header("대시 지속 시간(대시 거리 = 대시 속도 X 대시 지속 시간)")]
    [SerializeField] private float dashDuration = 1f;

    private State state = State.Chase;

    private Vector2 dashDir;
    private float timer;

    private bool isDashing = false;
    private PlayerCondition contactPlayer;

    private readonly int ATTACK_READY = Animator.StringToHash("AttackReady");
    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly int ATTACK_END = Animator.StringToHash("AttackEnd");

    protected override void Start()
    {
        base.Start();

        timer = stat.AttackInterval.Value;
    }

    protected override void UpdateAI()
    {
        timer -= Time.deltaTime;

        if (contactPlayer != null)
        {
            DealDamage();
        }

        switch (state)
        {
            case State.Chase:
                moveDirection = GetMoveDirection();

                if (IsPlayerInRange(stat.Range.Value))
                {
                    state = State.DashReady;
                    timer = dashReadyDuration;
                    monster.VisualEffect.PlayAttackEffect(dashReadyDuration);

                    PlayAttackReady();
                }
                break;

            case State.DashReady:
                moveDirection = Vector2.zero;

                if (timer <= 0f)
                {
                    state = State.Dash;
                    timer = dashDuration;
                    dashDir = DirToPlayer();
                    isDashing = true;

                    PlayAttack();
                }
                break;

            case State.Dash:
                UpdateDash();

                if (timer <= 0f)
                {
                    state = State.Cooldown;
                    timer = stat.AttackInterval.Value;
                    isDashing = false;

                    PlayAttackEnd();
                }
                break;

            case State.Cooldown:
                moveDirection = Vector2.zero;

                if (timer <= 0f)
                {
                    state = State.Chase;
                }

                break;
        }
    }

    protected override void Flip()
    {
        if (isDashing)
            return;

        base.Flip();
    }

    private void UpdateDash()
    {
        Vector2 moveDelta = dashDir * dashSpeed * Time.deltaTime;

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
    }

    #region Handle Animation
    protected virtual void PlayAttackReady()
    {
        monster.AnimationHandler.PlayAnimationWithTrigger(ATTACK_READY);
    }

    protected virtual void PlayAttack()
    {
        monster.AnimationHandler.PlayAnimationWithTrigger(ATTACK);
    }

    protected virtual void PlayAttackEnd()
    {
        monster.AnimationHandler.PlayAnimationWithTrigger(ATTACK_END);
    }
    #endregion

    #region Deal

    private void DealDamage()
    {
        if (contactPlayer == null)
            return;

        contactPlayer.TakeDamage(stat.Damage.Value);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerCondition>(out var playerCondition))
            return;

        contactPlayer = playerCondition;
        playerCondition.TakeDamage(stat.Damage.Value);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerCondition>(out var playerCondition))
            return;

        contactPlayer = null;
    }

    #endregion
}
