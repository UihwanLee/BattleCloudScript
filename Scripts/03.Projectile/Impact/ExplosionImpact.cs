using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ExplosionImpact : ImpactBase
{
    private float damage;
    private float range;
    private LayerMask targetLayer;
    private WeaponBase owner;

    public void Initialize(ImpactType type, WeaponBase owner, float damage, float range, LayerMask targetLayer)
    {
        base.Initialize(type);

        this.owner = owner;
        this.damage = damage;
        this.range = range;
        this.targetLayer = targetLayer;

        transform.localScale = Vector3.one * range;
    }

    public void OnDamageInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, targetLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var component))
            {
                component.TakeDamage(damage, owner);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, transform.localScale.x);
    }
}
