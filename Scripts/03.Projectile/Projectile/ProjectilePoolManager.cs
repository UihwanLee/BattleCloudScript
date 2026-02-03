using System;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

[Serializable]
public class ProjectilePair
{
    [SerializeField] private ProjectileType type;
    [SerializeField] private BasicProjectile prefab;
    [SerializeField] private int poolSize;

    public ProjectileType Type => type;
    public BasicProjectile Prefab => prefab;
    public int PoolSize => poolSize;
}

[Serializable]
public class ImpactPair
{
    [SerializeField] private ImpactType type;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize;

    public ImpactType Type => type;
    public GameObject Prefab => prefab;
    public int PoolSize => poolSize;
}

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance;

    [Header("Prefabs")]
    [SerializeField] private List<ProjectilePair> projectilePairs = new List<ProjectilePair>();
    [SerializeField] private List<ImpactPair> impactPairs = new List<ImpactPair>();

    private PoolManager poolManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        poolManager = PoolManager.Instance;
        InitializePool();
    }

    private void InitializePool()
    {
        if (projectilePairs.Count == 0)
        {
            Debug.Log("Prefab이 비어있습니다.");
            return;
        }

        // pool 생성
        foreach (var pair in projectilePairs)
        {
            if (pair == null)
            {
                Debug.Log($"projectilePreafab에 값이 없습니다.");
                continue;
            }
            poolManager.CreatePool(
                pair.Type.ToString(),
                pair.Prefab.gameObject,
                pair.PoolSize, 
                transform);
        }

        foreach (var pair in impactPairs)
        {
            if (pair == null)
            {
                Debug.Log($"impactPreafab에 값이 없습니다.");
                continue;
            }

            poolManager.CreatePool(
                pair.Type.ToString(),
                pair.Prefab.gameObject,
                pair.PoolSize,
                transform);
        }
    }

    public BasicProjectile GetProjectile(ProjectileType type)
    {
        BasicProjectile projectile = poolManager.GetObject(type.ToString()).GetComponent<BasicProjectile>();
        return projectile;
    }

    public ImpactBase GetImpact(ImpactType type)
    {
        ImpactBase impact = poolManager.GetObject(type.ToString()).GetComponentInChildren<ImpactBase>();
        return impact;
    }

    public void Release(ProjectileType type, BasicProjectile projectile)
    {
        poolManager.ReleaseObject(type.ToString(), projectile.gameObject);
    }

    public void Release(ImpactType type, ImpactBase impact)
    {
        poolManager.ReleaseObject(type.ToString(), impact.gameObject);
    }
}
