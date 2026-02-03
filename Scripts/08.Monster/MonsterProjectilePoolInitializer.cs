using UnityEngine;

public class MonsterProjectilePoolInitializer : MonoBehaviour
{
    private const string POOL_KEY = "MonsterProjectile";

    [Header("통합 몬스터 투사체 프리팹")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("초기 풀 사이즈")]
    [SerializeField] private int initialSize = 50;

    private void Start()
    {
        if (PoolManager.Instance == null)
        {
            Debug.LogError("[MonsterProjectilePoolInitializer] PoolManager.Instance가 null입니다.");
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("[MonsterProjectilePoolInitializer] projectilePrefab이 설정되지 않았습니다.");
            return;
        }

        // 이미 풀 있으면 중복 생성 방지
        if (PoolManager.Instance.HasPool(POOL_KEY))
            return;

        PoolManager.Instance.CreatePool(
            POOL_KEY,
            projectilePrefab,
            initialSize
        );

        Debug.Log($"[MonsterProjectilePoolInitializer] Pool Created : {POOL_KEY} (Size={initialSize})");
    }
}
