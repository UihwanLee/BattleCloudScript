using UnityEngine;
//using UnityEngine.AI;
using System.Collections;

public class MoveGroundRanged : MonoBehaviour
{
    [Header("플립 안정")]
    [SerializeField] private float faceEpsilon = 0.05f;
    [Header("공격 대기 모션")]
    [SerializeField] private float attackTelegraphTime = 1.0f;
    private bool isTelegraphing;

    private OldMonsterController controller;
    //private NavMeshAgent agent;

    private float lastFaceX = 1f;


    //public void Initialize(MonsterController controller)
    //{
    //    this.controller = controller;
    //    agent = controller.GetComponent<NavMeshAgent>();

    //    // 원거리 몬스터는 NavMesh가 이동 담당
    //    //if (controller is GroundMonsterController ground)
    //    //{
    //    //    ground.ignoreAgentVelocity = true;
    //    //    agent.updatePosition = true;
    //    //}
    //}
    private IEnumerator AttackTelegraphRoutine()
    {
        isTelegraphing = true;

        controller.PlayAttackTelegraph();

        yield return new WaitForSeconds(attackTelegraphTime);

        if (controller == null || controller.Player == null)
        {
            isTelegraphing = false;
            yield break;
        }

        float range = controller.Monster.Stat.Range.Value;
        float dist = Vector3.Distance(
            controller.transform.position,
            controller.Player.position
        );

        if (dist > range)
        {
            // Telegraph 도중 사거리 벗어남 → 취소
            isTelegraphing = false;
            yield break;
        }

        controller.TryAttack();
        isTelegraphing = false;
    }
    //public void Tick()
    //{
    //    if (!agent.enabled || !agent.isOnNavMesh)
    //        return;

    //    if (controller == null || controller.Player == null || agent == null)
    //        return;

    //    Transform player = controller.Player;

    //    float attackRange = controller.Monster.Stat.Range.Value; 
    //    float attackAllowRange = attackRange * 1.1f; 

    //    Vector3 toPlayer = player.position - controller.transform.position;
    //    float dist = toPlayer.magnitude;

    //    if (dist > attackRange)
    //    {
    //        if (!agent.enabled)
    //            agent.enabled = true;

    //        agent.isStopped = false;
    //        agent.SetDestination(player.position);

    //        // 추격 중에도 거의 사거리면 공격 허용
    //        if (dist <= attackAllowRange && !isTelegraphing)
    //        {
    //            controller.StartCoroutine(AttackTelegraphRoutine());
    //        }

    //        Vector3 vel = agent.velocity;
    //        if (vel.sqrMagnitude > 0.01f && Mathf.Abs(vel.x) > faceEpsilon)
    //            lastFaceX = Mathf.Sign(vel.x);

    //        ApplyFlip();
    //        return;
    //    }

    //    // 사거리 안 → 미세 이동 + 공격
    //    agent.isStopped = false;

    //    Vector3 sideDir = Vector3.Cross(Vector3.up, toPlayer.normalized);
    //    float sideOffset = Mathf.Sin(Time.time * 1.5f) * 0.5f;

    //    float desiredDistance = attackRange * 0.9f;

    //    Vector3 basePos =
    //        player.position - toPlayer.normalized * desiredDistance;

    //    Vector3 targetPos =
    //        basePos + sideDir * sideOffset;

    //    agent.SetDestination(targetPos);

    //    // 플립
    //    UpdateFlip(toPlayer);
    //    if (!isTelegraphing && controller.CanAttack())
    //    {
    //        controller.StartCoroutine(AttackTelegraphRoutine());
    //    }
    //}
    //private void UpdateFlip(Vector3 toPlayer)
    //{
    //    // 이동 거의 없으면 플립 갱신 안 함
    //    if (agent.velocity.sqrMagnitude < 0.05f)
    //        return;

    //    float targetFaceX = Mathf.Sign(toPlayer.x);

    //    // 너무 작은 변화는 무시
    //    if (Mathf.Abs(targetFaceX - lastFaceX) < 0.1f)
    //        return;

    //    lastFaceX = targetFaceX;
    //    ApplyFlip();
    //}
    // 플립 전용 처리
    //private void ApplyFlip()
    //{
    //    Vector3 scale = controller.transform.localScale;
    //    scale.x = Mathf.Abs(scale.x) * (lastFaceX < 0 ? -1 : 1);
    //    controller.transform.localScale = scale;
    //}
}
