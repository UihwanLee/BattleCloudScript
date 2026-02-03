using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class OldMonsterProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float lifeDistance = 10f;

    private Vector3 direction;
    private Vector3 startPos;
    private float damage;
    private float knockbackPower;
    private LayerMask targetLayer;
    private const string POOL_KEY = "MonsterProjectile";

    private Transform owner;
    private bool initialized = false;

    private void OnEnable()
    {
        // 풀 재사용 대비
        direction = Vector3.zero;
        initialized = false;
        owner = null;
    }

    public void Initialize(
        Vector3 dir,
        float damage,
        float knockbackPower,
        float lifeDistance,
        LayerMask targetLayer,
        Transform owner
    )
    {
        this.direction = dir.normalized;
        this.damage = damage;
        this.knockbackPower = knockbackPower;
        this.lifeDistance = lifeDistance;
        this.targetLayer = targetLayer;
        this.owner = owner;

        startPos = transform.position;
        initialized = true;
    }

    private void Update()
    {
        // 전투 종료 중이면 즉시 반환
        if (GameManager.Instance.IsEndingPhase)
        {
            ReturnToPool();
            return;
        }

        // Initialize 전에 Update 도는 것 방지
        if (!initialized)
            return;

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) >= lifeDistance)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        // 발사자 충돌 무시
        if (owner != null && other.transform.root == owner)
            return;

        // 타겟 레이어 아니면 무시
        if ((targetLayer & (1 << other.gameObject.layer)) == 0)
            return;

        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage, null);

            if (other.TryGetComponent<IKnockbackable>(out var kb))
                kb.ApplyKnockback(transform, knockbackPower);

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (PoolManager.Instance == null)
        {
            gameObject.SetActive(false);
            return;
        }

        PoolManager.Instance.ReleaseObject(POOL_KEY, gameObject);
    }

    public static void ReleaseAllByOwner(Transform owner)
    {
        if (PoolManager.Instance == null)
            return;

        var projectiles = GameObject.FindObjectsOfType<OldMonsterProjectile>();
        foreach (var p in projectiles)
        {
            if (!p.gameObject.activeInHierarchy)
                continue;

            if (p.owner == owner)
            {
                PoolManager.Instance.ReleaseObject("MonsterProjectile", p.gameObject);
            }
        }
    }
    public static void ReleaseAll()
    {
        if (PoolManager.Instance == null)
            return;

        var projectiles = GameObject.FindObjectsOfType<OldMonsterProjectile>();
        foreach (var p in projectiles)
        {
            if (!p.gameObject.activeInHierarchy)
                continue;

            PoolManager.Instance.ReleaseObject("MonsterProjectile", p.gameObject);
        }
    }
}
