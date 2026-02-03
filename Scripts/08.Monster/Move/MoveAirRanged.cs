using UnityEngine;
using System.Collections;

public class MoveAirRanged : MonoBehaviour/*, IMonsterMove*/
{
    private OldMonsterController controller;

    [Header("기본 거리")]
    [SerializeField] private float keepDistance = 2.6f;

    [Header("최소 유지 거리")]
    [SerializeField] private float minDistance = 2.2f;

    [Header("유영 반경")]
    [SerializeField] private float wanderRadius = 0.3f;

    [Header("유영 갱신 주기")]
    [SerializeField] private float wanderInterval = 2.5f;

    [Header("방향 전환 속도")]
    [SerializeField] private float turnSmooth = 2.0f;

    [Header("공격 중 이동 속도 비율")]
    [SerializeField] private float attackOrbitSpeed = 0.6f;

    [Header("공격 대기 모션")]
    [SerializeField] private float attackTelegraphTime = 1.0f;
    private bool isTelegraphing;

    private Vector2 currentMoveDir;
    private Vector2 wanderAnchor;
    private Vector2 stableBaseDir;
    private float wanderTimer;

    public void Initialize(OldMonsterController controller)
    {
        this.controller = controller;
        currentMoveDir = Vector2.zero;
        stableBaseDir = Vector2.zero;

        if (controller.Player != null)
            PickNewWanderAnchor(true);
    }

    private IEnumerator AttackTelegraphRoutine()
    {
        isTelegraphing = true;

        controller.PlayAttackTelegraph();
        yield return new WaitForSeconds(attackTelegraphTime);

        controller.TryAttack();
        isTelegraphing = false;
    }

    public void Tick()
    {
        if (controller == null || controller.Player == null)
            return;

        Transform player = controller.Player;
        float attackRange = controller.Monster.Stat.Range.Value;
        float distToPlayer = Vector2.Distance(transform.position, player.position);


         // 1. 공격 중 : 궤도 이동

        if (distToPlayer <= attackRange)
        {
            if (!isTelegraphing && controller.CanAttack())
            {
                controller.StartCoroutine(AttackTelegraphRoutine());
            }

            Vector2 baseDir =
                ((Vector2)transform.position - (Vector2)player.position).normalized;

            // 수직 벡터 (궤도)
            Vector2 orbitDir = new Vector2(-baseDir.y, baseDir.x);

            currentMoveDir = Vector2.Lerp(
                currentMoveDir,
                orbitDir,
                Time.deltaTime * 2f
            );

            controller.SetMoveDirection(currentMoveDir * attackOrbitSpeed);
            return;
        }


         //2. 너무 가까우면 밀어냄
        if (distToPlayer < minDistance)
        {
            Vector2 pushDir =
                ((Vector2)transform.position - (Vector2)player.position).normalized;

            currentMoveDir = Vector2.Lerp(
                currentMoveDir,
                pushDir,
                Time.deltaTime * 3f
            );

            controller.SetMoveDirection(currentMoveDir);
            return;
        }


        //3. 비공격 상태 : 유영
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
            PickNewWanderAnchor(false);

        Vector2 toAnchor = wanderAnchor - (Vector2)transform.position;
        if (toAnchor.magnitude < 0.15f)
        {
            controller.SetMoveDirection(Vector2.zero);
            return;
        }

        Vector2 desiredDir = toAnchor.normalized;

        currentMoveDir = Vector2.MoveTowards(
            currentMoveDir,
            desiredDir,
            Time.deltaTime * turnSmooth
        );

        controller.SetMoveDirection(currentMoveDir);
    }

    private void PickNewWanderAnchor(bool force)
    {
        wanderTimer = wanderInterval;

        Vector2 playerPos = controller.Player.position;

        if (force || stableBaseDir == Vector2.zero)
        {
            stableBaseDir = ((Vector2)transform.position - playerPos).normalized;
            if (stableBaseDir == Vector2.zero)
                stableBaseDir = Random.insideUnitCircle.normalized;
        }

        wanderAnchor =
            playerPos +
            stableBaseDir * keepDistance +
            Random.insideUnitCircle * wanderRadius;
    }
}
