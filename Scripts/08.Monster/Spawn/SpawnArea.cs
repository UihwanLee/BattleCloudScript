using UnityEngine;
//using UnityEngine.AI;

public class SpawnArea : MonoBehaviour
{
    #region Inspector

    [Header("스폰 영역 (사각형)")]
    [SerializeField] private Vector2 size;

    [Header("시도 횟수")]
    [SerializeField] private int maxTryCount = 20;

    [Header("플레이어 제외 반경")]
    [SerializeField] private float excludeRadius = 2.5f;

    [Header("NavMesh 사용 여부")]
    [SerializeField] private bool useNavMesh = true;

    [Header("장애물 검사")]
    [SerializeField] private LayerMask blockLayer;

    [Header("몬스터 겹침 검사")]
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private float monsterCheckRadius = 0.5f;

    #endregion

    #region 스폰 위치 계산

    public Vector3 GetSpawnPosition(Vector3 playerPos)
    {
        for (int i = 0; i < maxTryCount; i++)
        {
            Vector3 pos = GetRandomBoxPosition();

            if (excludeRadius > 0f &&
                Vector3.Distance(pos, playerPos) < excludeRadius)
                continue;

            if (IsBlocked(pos))
                continue;

            if (IsMonsterOverlapped(pos))
                continue;

            //if (useNavMesh)
            //{
            //    // Z를 NavMesh 기준으로 보정
            //    Vector3 navCheckPos = new Vector3(pos.x, pos.y, transform.position.z);

            //    if (!UnityEngine.AI.NavMesh.SamplePosition(navCheckPos, out var hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
            //        continue;

            //    // 다시 2D 좌표로 반환
            //    return new Vector3(hit.position.x, hit.position.y, 0f);
            //}

            return pos;
        }

        return Vector3.zero;
    }

    private Vector3 GetRandomBoxPosition()
    {
        Vector2 half = size * 0.5f;

        return transform.position + new Vector3(
            Random.Range(-half.x, half.x),
            Random.Range(-half.y, half.y),
            0f
        );
    }

    #endregion

    #region 내부 검사

    private bool IsBlocked(Vector2 pos)
    {
        return Physics2D.OverlapCircle(pos, 0.3f, blockLayer) != null;
    }

    private bool IsMonsterOverlapped(Vector2 pos)
    {
        return Physics2D.OverlapCircle(pos, monsterCheckRadius, monsterLayer) != null;
    }

    #endregion

    #region Gizmo

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, size);
    }

    #endregion
}
