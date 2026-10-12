using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DropItemPool : MonoBehaviour
{
    [Header("DropItem 정보")]
    [SerializeField] private string key;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxCount;

    [Header("충돌정보")]
    [SerializeField] private List<Tilemap> groundTilemaps = new List<Tilemap>();
    [SerializeField] private LayerMask obstacleLayer;

    [Header("겹치지 않는 위치 탐색")]
    [SerializeField] private float checkRadius = 0.4f;          // 장애물 겹침 검사 반경
    [SerializeField] private float startSearchRadius = 0.5f;    // 첫 탐색 반경
    [SerializeField] private float searchRadiusStep = 0.2f;     // 실패할 때마다 늘릴 반경
    [SerializeField] private int maxAttempts = 15;

    private PoolManager poolManager;

    private static DropItemPool instance;

    public static DropItemPool Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<DropItemPool>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // 씬이 다시 로드되면 기존 인스턴스는 이전 씬의 Tilemap을 들고 있으므로 새 씬의 참조로 교체
            instance.groundTilemaps = groundTilemaps;
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        poolManager = PoolManager.Instance;
        Initialize();
    }

    private void Initialize()
    {
        // DropItem Pool 생성 (DontDestroyOnLoad 오브젝트이므로 씬이 바뀌어도 Pool 유지)
        poolManager.CreatePool(key, prefab, maxCount, transform, persistent: true);
    }

    public GameObject GetDropPrefab()
    {
        return poolManager.GetObject(key);
    }

    public void SpawnDropItemWithFall(ISlotItem item, Vector3 startPosition, float height, float duration)
    {
        if (!TrySpawn(item, out DropItem dropItem)) return;

        dropItem.SetPosition(startPosition);
        dropItem.StartFallAnimation(height, duration);
    }

    public void SpawnDropItem(ISlotItem item)
    {
        if (!TrySpawn(item, out DropItem dropItem)) return;

        dropItem.SetAttract(false);
        Vector3 spawnPos = GetNonOverlappingPosition(GameManager.Instance.Player.transform.position);
        dropItem.SetPosition(spawnPos);
    }

    public void SpawnDropItemByDropAnimation(ISlotItem item, Vector3 position, Vector3 targetPosition, float dropDuration)
    {
        if (!TrySpawn(item, out DropItem dropItem)) return;

        dropItem.SetPosition(position);
        dropItem.StartDropAnimation(targetPosition, dropDuration);
    }

    /// <summary>
    /// Pool에서 DropItem을 꺼내 아이템 정보를 적용
    /// </summary>
    private bool TrySpawn(ISlotItem item, out DropItem dropItem)
    {
        dropItem = null;

        GameObject go = poolManager.GetObject(key);
        if (go == null) return false;

        if (!go.TryGetComponent(out dropItem))
        {
            Debug.LogError($"[DropItemPool] {key} 프리팹에 DropItem 컴포넌트가 없습니다.");
            poolManager.ReleaseObject(key, go);
            return false;
        }

        dropItem.OpenItem(item);
        dropItem.SetDropItem(item);
        return true;
    }

    /// <summary>
    /// center 주변에서 바닥 타일 위이면서 장애물과 겹치지 않는 위치 탐색
    /// 실패할 때마다 탐색 반경을 넓히고, 끝까지 못 찾으면 center 반환
    /// </summary>
    private Vector3 GetNonOverlappingPosition(Vector3 center)
    {
        float searchRadius = startSearchRadius;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * searchRadius;
            Vector3 candidatePos = center + new Vector3(randomCircle.x, randomCircle.y, 0);

            if (IsOnGround(candidatePos) &&
                Physics2D.OverlapCircle(candidatePos, checkRadius, obstacleLayer) == null)
            {
                return candidatePos;
            }

            searchRadius += searchRadiusStep;
        }

        return center;
    }

    private bool IsOnGround(Vector3 position)
    {
        foreach (Tilemap tm in groundTilemaps)
        {
            if (tm == null) continue;

            // 각 타일맵의 좌표계에 맞춰 변환 후 타일 존재 여부 확인
            if (tm.HasTile(tm.WorldToCell(position)))
                return true;
        }
        return false;
    }

    public void Release(GameObject obj)
    {
        poolManager.ReleaseObject(key, obj);
    }

    /// <summary>
    /// 기존에 뿌려진 DropItem 오브젝트 모두 반환
    /// </summary>
    public void ReleaseAllDropItem()
    {
        poolManager.ReleaseAllObject(key);
    }

    #region 프로퍼티

    public string Key { get { return key; } }

    #endregion
}
