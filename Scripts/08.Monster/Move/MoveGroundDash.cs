using System.Collections;
using UnityEngine;
//using UnityEngine.AI;
using WhereAreYouLookinAt.Enum;

public class MoveGroundDash : MonoBehaviour
{
    #region 참조
    private OldMonsterController controller;
    //private NavMeshAgent agent;
    #endregion

    #region 상태
    private bool isDashing;
    private bool hasHitPlayer;

    private bool isRecovering;
    private float recoverTimer;

    private float dashTravelDistance;
    private float lastFaceX = 1f;

    private Vector3 dashTargetPos;
    #endregion

    #region 연출
    [SerializeField] private float dashTelegraphTime = 1.0f;
    private bool isTelegraphing;
    #endregion

    #region 설정값
    [Header("돌진 감지/트리거 비율")]
    [SerializeField] private float dashDetectRatio = 4.5f;
    [SerializeField] private float dashTriggerRatio = 3.0f;

    [Header("돌진 수치")]
    [SerializeField] private float dashPassDistance = 4f;
    [SerializeField] private float dashHitDistance = 0.8f;

    [Header("돌진 속도")]
    [SerializeField] private float dashSpeedMultiplier = 2.4f;
    [SerializeField] private float dashEndSpeedRatio = 0.6f;

    [Header("돌진 감속 곡선")]
    [SerializeField]
    private AnimationCurve dashEaseOut =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("돌진 후 딜레이")]
    [SerializeField] private float recoverDuration = 0.4f;

    [Header("NavMesh 안전 보정")]
    [SerializeField] private float navmeshRecoverRadius = 2.5f;
    [SerializeField] private float navmeshSampleRadiusForDashTarget = 2.0f;
    #endregion

    #region 초기화
    //public void Initialize(MonsterController controller)
    //{
    //    this.controller = controller;
    //    //agent = controller.GetComponent<NavMeshAgent>();
    //}
    #endregion

    #region Tick
    //public void Tick()
    //{
    //    if (!agent.enabled || !agent.isOnNavMesh)
    //        return;

    //    if (GameManager.Instance.State != GameState.RUNNING)
    //        return;

    //    if (controller == null || agent == null || controller.Player == null)
    //        return;

    //    // BaseController(transform 이동) 완전 차단: 항상 0 유지
    //    controller.SetMoveDirection(Vector2.zero);

    //    // agent 자체가 비정상이면 아무 것도 하지 않음
    //    if (!agent.enabled)
    //        return;

    //    // NavMesh 이탈 감지 시: 즉시 복구 + 대시 취소
    //    if (!agent.isOnNavMesh)
    //    {
    //        RecoverToNavMeshOrStop();
    //        return;
    //    }

    //    Transform player = controller.Player;

    //    // 회복 상태
    //    if (isRecovering)
    //    {
    //        recoverTimer -= Time.deltaTime;
    //        if (recoverTimer > 0f)
    //            return;

    //        isRecovering = false;
    //    }

    //    float distToPlayer = Vector2.Distance(
    //        controller.transform.position,
    //        player.position
    //    );

    //    // 대시 중 (NavMesh 기반)
    //    if (isDashing)
    //    {
    //        // path가 없는/끊긴 경우 즉시 종료
    //        if (!agent.hasPath && !agent.pathPending)
    //        {
    //            EndDash();
    //            return;
    //        }

    //        float remaining = agent.remainingDistance;

    //        // remainingDistance가 무한대/이상치면 종료
    //        if (float.IsInfinity(remaining) || float.IsNaN(remaining))
    //        {
    //            EndDash();
    //            return;
    //        }

    //        // 진행도 기반 감속
    //        agent.isStopped = false;
    //        agent.speed = controller.CurrentSpeed * dashSpeedMultiplier;

    //        // 플립은 agent.velocity 기준
    //        if (Mathf.Abs(agent.velocity.x) > 0.01f)
    //            lastFaceX = Mathf.Sign(agent.velocity.x);
    //        ApplyFlip();

    //        // 히트 판정 (1회)
    //        if (!hasHitPlayer &&
    //            Vector2.Distance(
    //                controller.transform.position,
    //                player.position
    //            ) <= dashHitDistance)
    //        {
    //            hasHitPlayer = true;
    //        }

    //        // 종료 조건
    //        if (!agent.pathPending &&
    //            agent.remainingDistance <= agent.stoppingDistance + 0.05f)
    //        {
    //            EndDash();
    //        }

    //        return;
    //    }

    //    float range = controller.Monster.Stat.Range.Value;
    //    float detectDist = range * dashDetectRatio;
    //    float triggerDist = range * dashTriggerRatio;

    //    // 감지 연출(플립만)
    //    if (distToPlayer <= detectDist)
    //    {
    //        Vector2 faceDir =
    //            (player.position - controller.transform.position).normalized;

    //        if (Mathf.Abs(faceDir.x) > 0.01f)
    //            lastFaceX = Mathf.Sign(faceDir.x);
    //        ApplyFlip();
    //    }

    //    // 대시 트리거
    //    if (distToPlayer <= triggerDist)
    //    {
    //        if (!isTelegraphing && !isRecovering)
    //        {
    //            controller.StartCoroutine(DashTelegraphRoutine());
    //        }
    //        return;
    //    }

    //    // 평소 추적 (NavMesh) - 이동은 agent만, BaseController는 계속 0
    //    agent.isStopped = false;
    //    agent.SetDestination(player.position);

    //    // flip은 desiredVelocity로만(연출)
    //    Vector3 desired = agent.desiredVelocity;
    //    if (Mathf.Abs(desired.x) > 0.01f)
    //        lastFaceX = Mathf.Sign(desired.x);
    //    ApplyFlip();
    //}
    //#endregion

    //#region 대시 처리
    //private void StartDash(Vector3 playerPos, float currentDist)
    //{
    //    if (agent == null || !agent.enabled)
    //        return;

    //    // NavMesh 위가 아니면 대시 시작 금지
    //    if (!agent.isOnNavMesh)
    //    {
    //        RecoverToNavMeshOrStop();
    //        return;
    //    }

    //    isDashing = true;
    //    hasHitPlayer = false;

    //    Vector3 startPos = controller.transform.position;

    //    // 목표 방향(현재 플레이어 위치 기준)
    //    Vector3 dashDir = (playerPos - startPos).normalized;
    //    if (dashDir.sqrMagnitude < 0.0001f)
    //        dashDir = Vector3.right;

    //    Vector3 rawTarget =
    //        startPos + dashDir * (currentDist + dashPassDistance);

    //    // 대시 타겟은 반드시 NavMesh 위
    //    if (NavMesh.SamplePosition(
    //            rawTarget,
    //            out var hit,
    //            navmeshSampleRadiusForDashTarget,
    //            NavMesh.AllAreas))
    //    {
    //        dashTargetPos = hit.position;
    //    }
    //    else
    //    {
    //        // 샘플 실패하면: 현재 위치에서 대시 취소(안전)
    //        dashTargetPos = startPos;
    //        isDashing = false;
    //        isRecovering = true;
    //        recoverTimer = recoverDuration;
    //        return;
    //    }

    //    dashTravelDistance =
    //        Vector3.Distance(startPos, dashTargetPos);

    //    agent.isStopped = false;
    //    agent.ResetPath();
    //    agent.SetDestination(dashTargetPos);
    //}

    //private void EndDash()
    //{
    //    isDashing = false;

    //    if (agent != null && agent.enabled && agent.isOnNavMesh)
    //    {
    //        agent.speed = controller.CurrentSpeed;
    //        agent.isStopped = false;
    //        agent.ResetPath();
    //    }

    //    isRecovering = true;
    //    recoverTimer = recoverDuration;
    //}

    //private IEnumerator DashTelegraphRoutine()
    //{
    //    isTelegraphing = true;

    //    // 텔레그래프 중에도 BaseController 이동 차단은 Tick에서 계속 보장됨
    //    controller.PlayAttackTelegraph();
    //    yield return new WaitForSeconds(dashTelegraphTime);

    //    // 텔레그래프 후 NavMesh 상태 재검증
    //    if (agent == null || !agent.enabled || !agent.isOnNavMesh)
    //    {
    //        RecoverToNavMeshOrStop();
    //        isTelegraphing = false;
    //        yield break;
    //    }

    //    Transform player = controller.Player;
    //    if (player == null)
    //    {
    //        isTelegraphing = false;
    //        yield break;
    //    }

    //    Vector3 dashTarget = player.position;
    //    float dist = Vector2.Distance(
    //        controller.transform.position,
    //        dashTarget
    //    );

    //    StartDash(dashTarget, dist);

    //    isTelegraphing = false;
    //}
    //#endregion

    //#region NavMesh 안전 복구
    //private void RecoverToNavMeshOrStop()
    //{
    //    // 이탈 상태에서는 대시/텔레그래프 즉시 종료
    //    isDashing = false;
    //    isTelegraphing = false;

    //    if (agent == null || !agent.enabled)
    //        return;

    //    if (NavMesh.SamplePosition(
    //            controller.transform.position,
    //            out var hit,
    //            navmeshRecoverRadius,
    //            NavMesh.AllAreas))
    //    {
    //        agent.Warp(hit.position);
    //        agent.isStopped = false;
    //        agent.ResetPath();

    //        // 복구 직후 잠깐 회복 처리(떨림 방지)
    //        isRecovering = true;
    //        recoverTimer = recoverDuration;
    //    }
    //    else
    //    {
    //        // 근처에 NavMesh가 없으면 더 이상 진행 불가
    //        agent.isStopped = true;
    //        agent.ResetPath();
    //    }
    //}
    //#endregion

    //#region 플립
    //private void ApplyFlip()
    //{
    //    Vector3 scale = controller.transform.localScale;
    //    scale.x = Mathf.Abs(scale.x) * (lastFaceX < 0 ? -1 : 1);
    //    controller.transform.localScale = scale;
    //}
    //#endregion

    #region 충돌 데미지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        if (!isDashing || hasHitPlayer)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasHitPlayer = true;

        PlayerCondition pc = other.GetComponent<PlayerCondition>();
        if (pc == null)
            return;

        pc.TakeDamage(controller.Monster.Stat.Damage.Value);
    }
    #endregion
    #endregion
}