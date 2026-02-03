using Org.BouncyCastle.Asn1.Esf;
using UnityEngine;
//using UnityEngine.AI;

public class MoveGroundTracking : MonoBehaviour/*, IMonsterMove*/
{
    #region 참조
    private Monster monster;
    //private NavMeshAgent agent;
    #endregion

    #region 설정값
    [Header("근접 공격 쿨타임")]
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Separation")]
    [SerializeField] private float separationRadius = 0.6f;
    [SerializeField] private float separationForce = 0.4f;
    #endregion

    #region 상태
    private float damageTimer;
    [SerializeField] private float faceDeadZone = 0.25f;
    [SerializeField] private float horizontalBias = 1.2f; // 좌우 우세 비율
    private float lastFaceX = 1f;
    #endregion

    private void Awake()
    {
        monster = GetComponent<Monster>();
    }

    #region 초기화
    //public void Initialize(MonsterController controller)
    //{
    //    this.controller = controller;
    //agent = controller.GetComponent<NavMeshAgent>();

    // ⭐ 원거리 지상 몬스터와 동일한 이동 구조
    //if (controller is GroundMonsterController ground && agent != null)
    //{
    //    ground.ignoreAgentVelocity = true;
    //    agent.updatePosition = true;
    //    agent.stoppingDistance = 0f;
    //}
    //}
    #endregion

    #region 이동 / 공격 판단
    //public void Tick()
    //{
    //    if (!agent.enabled || !agent.isOnNavMesh)
    //        return;

    //    if (damageTimer > 0f)
    //        damageTimer -= Time.deltaTime;

    //    if (agent == null || !agent.isOnNavMesh || controller.Player == null)
    //        return;

    //    Transform player = controller.Player;

    //    Vector3 vel = agent.velocity;

    //    float dist = Vector2.Distance(agent.nextPosition, player.position);
    //    float attackRange = controller.Monster.Stat.Range.Value;
    //    float stopDist = 0.05f;

    //    // 이동 구간
    //    if (dist > stopDist)
    //    {
    //        Vector3 baseTarget = player.position;

    //        // separation → 목적지 오프셋
    //        Vector2 sepOffset = CalculateSeparationOffset();
    //        Vector3 targetPos = baseTarget + (Vector3)sepOffset;

    //        // NavMesh 위로 보정
    //        if (NavMesh.SamplePosition(targetPos, out var hit, 0.5f, NavMesh.AllAreas))
    //            targetPos = hit.position;
    //        else
    //            targetPos = baseTarget;

    //        agent.isStopped = false;
    //        agent.SetDestination(targetPos);

    //        Vector2 toPlayer = (Vector2)(player.position - controller.transform.position);

    //        float absX = Mathf.Abs(toPlayer.x);
    //        float absY = Mathf.Abs(toPlayer.y);

    //        // 좌우 차이가 충분히 클 것
    //        if (absX > faceDeadZone)
    //        {
    //            // 좌우가 상하보다 우세할 것
    //            if (absX > absY * 1.2f)
    //            {
    //                float newFace = Mathf.Sign(toPlayer.x);

    //                if (newFace != lastFaceX)
    //                {
    //                    lastFaceX = newFace;
    //                    controller.SetMoveDirection(new Vector2(lastFaceX, 0f));
    //                }
    //            }
    //        }
    //    }
    //    // 정지 / 공격 구간
    //    else
    //    {
    //        agent.isStopped = true;

    //        // 방향 유지 (SetMoveDirection 호출 안 함)
    //        if (dist <= attackRange)
    //            controller.TryAttack();
    //    }
    //}
    #endregion

    #region 근접 충돌 데미지
    private void OnTriggerStay2D(Collider2D other)
    {
        if (damageTimer > 0f)
            return;


        //if (!other.CompareTag("Player"))
        //    return;

        //PlayerCondition pc = other.GetComponent<PlayerCondition>();
        //if (pc == null)
        //    return;

        if (!other.TryGetComponent<PlayerCondition>(out var condition))
            return;

        condition.TakeDamage(monster.Stat.Damage.Value);
        damageTimer = damageCooldown;
    }
    #endregion

    #region Separation (NavMesh-safe)
    //private Vector2 CalculateSeparationOffset()
    //{
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(
    //        controller.transform.position,
    //        separationRadius
    //    );

    //    Vector2 pushDir = Vector2.zero;

    //    foreach (var hit in hits)
    //    {
    //        if (hit.gameObject == controller.gameObject)
    //            continue;

    //        if (hit.gameObject.layer != LayerMask.NameToLayer("Monster"))
    //            continue;

    //        Vector2 diff =
    //            (Vector2)(controller.transform.position - hit.transform.position);

    //        float dist = diff.magnitude;
    //        if (dist <= 0.001f)
    //            continue;

    //        pushDir += diff.normalized / dist;
    //    }

    //    if (pushDir.sqrMagnitude < 0.0001f)
    //        return Vector2.zero;

    //    return pushDir.normalized * separationForce;
    //}
    #endregion
}
