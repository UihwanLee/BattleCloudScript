using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GroundMonsterController : OldMonsterController
{
    //private NavMeshAgent agent;
    //private float stableMoveX = 1f;
    //private float flipLockTimer = 0f;
    //private const float FLIP_LOCK_TIME = 0.12f;
    //public bool ignoreAgentVelocity = false;


    //protected override void Start()
    //{
    //    baseSpeed = monster.Stat.MoveSpeed.Value;
    //    base.Start();

    //    //agent = GetComponent<NavMeshAgent>();
    //    //agent.updateRotation = false;
    //    //agent.updateUpAxis = false;
    //    //agent.updatePosition = false;

    //    //agent.speed = currentSpeed;
    //    //agent.angularSpeed = 999f;
    //    //agent.acceleration = 999f;
    //    //agent.stoppingDistance = 0f;

    //    // 원거리 몬스터일 때만 실제 이동 주체로 전환
    //    //if (ignoreAgentVelocity)
    //    //{
    //    //    agent.updatePosition = true;
    //    //}

    //    //if (!agent.isOnNavMesh)
    //    //{
    //    //    if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
    //    //    {
    //    //        agent.Warp(hit.position);
    //    //    }
    //    //}
    //}
    //private void ClampPositionToNavMesh()
    //{
        //if (agent == null || !agent.enabled)
        //    return;

        //if (agent.isOnNavMesh)
        //    return;

        //if (NavMesh.SamplePosition(
        //    transform.position,
        //    out var hit,
        //    0.6f,
        //    NavMesh.AllAreas))
        //{
        //    agent.Warp(hit.position);
        //}
    //}
    //protected override void HandleMove()
    //{
    //    if (agent == null)
    //        return;

    //    // NavMesh 이탈 시 강제 복귀
    //    if (!agent.isOnNavMesh)
    //    {
    //        if (NavMesh.SamplePosition(
    //            transform.position,
    //            out var hit,
    //            2.5f,
    //            NavMesh.AllAreas))
    //        {
    //            agent.Warp(hit.position);
    //            agent.ResetPath();      // ⭐ 중요
    //            agent.isStopped = false; // ⭐ 중요
    //        }

    //        SetMoveDirection(Vector2.zero);
    //        return;
    //    }

    //    // 근접 몬스터: 공격 중 → 이동 차단
    //    if (!ignoreAgentVelocity && agent.isStopped)
    //    {
    //        SetMoveDirection(Vector2.zero);
    //        return;
    //    }

    //    // NavMeshAgent가 이동 주체인 경우
    //    if (ignoreAgentVelocity)
    //    {
    //        // BaseController 이동 차단
    //        SetMoveDirection(Vector2.zero);

    //        // NavMesh 이동만 허용
    //        moveLogic?.Tick();
    //        return;
    //    }

    //    moveLogic?.Tick();

    //    if (!ignoreAgentVelocity && spriteRenderer != null && player != null)
    //    {
    //        float dx = player.position.x - transform.position.x;
    //        if (Mathf.Abs(dx) > 0.05f)
    //            spriteRenderer.flipX = dx < 0;
    //    }

        //ClampPositionToNavMesh();
    //}
    //protected override void OnKnockbackStart()
    //{
        //if (agent == null)
        //    return;

        //if (!agent.enabled || !agent.isOnNavMesh)
        //    return;

        //agent.isStopped = true;
    //}

    //protected override void OnKnockbackEnd()
    //{
        //if (agent == null)
        //    return;

        //if (!agent.enabled)
        //    return;

        //if (!agent.isOnNavMesh)
        //    return;

        //// 위치 동기화만 하고, Resume는 하지 않는다
        //agent.Warp(transform.position);

        //// 다음 프레임에서 풀어준다
        //StartCoroutine(ResumeAgentNextFrame());
    //}

    //private IEnumerator ResumeAgentNextFrame()
    //{
    //    yield return null;

    //    if (agent == null)
    //        yield break;

    //    if (!agent.enabled || !agent.isOnNavMesh)
    //        yield break;

    //    agent.isStopped = false;
    //}

    //public override void RefreshStat()
    //{
    //    base.RefreshStat();

    //    //if (agent != null)
    //    //    agent.speed = currentSpeed;
    //}

    //protected override void CalculateSpeed()
    //{
    //    base.CalculateSpeed();
    //    //if (agent != null)
    //    //    agent.speed = currentSpeed;
    //}
}
