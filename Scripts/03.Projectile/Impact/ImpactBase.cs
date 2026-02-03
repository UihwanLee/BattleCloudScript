using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ImpactBase : MonoBehaviour
{
    [Header("Impact Type")]
    [SerializeField] protected ImpactType impactType;

    protected ProjectilePoolManager poolManager;

    protected virtual void Start()
    {
        poolManager = ProjectilePoolManager.Instance;
    }

    public virtual void Initialize(ImpactType type)
    {
        impactType = type;
    }

    public virtual void OnExplosionFinished()
    {
        poolManager.Release(impactType, this);
    }
}
