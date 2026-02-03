using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class DropItemPool : MonoBehaviour
{
    [Header("DropItem 정보")]
    [SerializeField] private string key;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxCount;

    [Header("충돌정보")]
    [SerializeField] private List<Tilemap> groundTilemaps = new List<Tilemap>();
    [SerializeField] private LayerMask obstacleLayer;

    private PoolManager poolManager;
    
    private static DropItemPool instance;

    private Player player;

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
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        poolManager = PoolManager.Instance;
        player = GameManager.Instance.Player;
        Initialize();
    }

    private void Initialize()
    {
        // Damage 생성
        poolManager.CreatePool(key, prefab, maxCount, transform);
    }

    public GameObject GetDropPrefab()
    {
        return poolManager.GetObject(key);
    }

    public void SpawnDropItemWithFall(ISlotItem item, Vector3 startPosition, float height, float duration)
    {
        GameObject go = poolManager.GetObject(key);
        if (go.TryGetComponent<DropItem>(out DropItem dropItem))
        {
            dropItem.OpenItem(item);
            dropItem.SetDropItem(item);
            dropItem.SetPosition(startPosition);
            dropItem.StartFallAnimation(height, duration);
        }
    }

    public void SpawnDropItem(ISlotItem item)
    {
        // 드롭 아이템 생성
        GameObject go = poolManager.GetObject(key);
        if(go.TryGetComponent<DropItem>(out DropItem dropItem))
        {
            dropItem.OpenItem(item);
            dropItem.SetDropItem(item);
            dropItem.SetAttract(false);
            Vector3 spawnPos = GetNonOverlappingPosition(player.transform.position);
            dropItem.SetPosition(spawnPos);
        }
    }

    private Vector3 GetNonOverlappingPosition(Vector3 center)
    {
        float checkRadius = 0.4f;
        float searchRadius = 0.5f;
        int maxAttempts = 15;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * searchRadius;
            Vector3 candidatePos = center + new Vector3(randomCircle.x, randomCircle.y, 0);

            // 바닥 타일 리스트 체크
            bool isOnGround = false;
            foreach (Tilemap tm in groundTilemaps)
            {
                // 각 타일맵의 좌표계에 맞춰 변환 후 타일 존재 여부 확인
                Vector3Int cellPos = tm.WorldToCell(candidatePos);
                if (tm.HasTile(cellPos))
                {
                    isOnGround = true;
                    break;
                }
            }

            // Ground 체크
            if (!isOnGround)
            {
                searchRadius += 0.2f;
                continue;
            }

            // Layer 체크 
            Collider2D hit = Physics2D.OverlapCircle(candidatePos, checkRadius, obstacleLayer);

            if (hit == null)
            {
                return candidatePos;
            }

            searchRadius += 0.2f;
        }

        // 모든 시도가 실패하면 center 반환 
        return center;
    }

    public void SpawnDropItemByDropAnimation(ISlotItem item, Vector3 position, Vector3 targetPosition, float dropDuration)
    {
        // 드롭 아이템 생성
        GameObject go = poolManager.GetObject(key);
        if (go.TryGetComponent<DropItem>(out DropItem dropItem))
        {
            dropItem.OpenItem(item);
            dropItem.SetDropItem(item);
            dropItem.SetPosition(position);
            dropItem.StartDropAnimation(targetPosition, dropDuration);
        }
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
        // 현재 하위에 있는 오브젝트 가져오기
        DropItem[] dropItems = transform.GetComponentsInChildren<DropItem>();

        foreach(DropItem items in dropItems)
        {
            // 활성화되어 있는 obj 모두 제거
            if(items.gameObject.activeSelf)
                poolManager.ReleaseObject(key, items.gameObject);
        }
    }

    #region 프로퍼티

    public string Key { get { return key; } }

    #endregion
}
