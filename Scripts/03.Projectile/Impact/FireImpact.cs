using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class FireImpact : ImpactBase
{
    private float damage;
    private float extent;
    private LayerMask targetLayer;
    private WeaponBase owner;

    public void Initialize(ImpactType type, WeaponBase owner, float damage, float extent, LayerMask targetLayer)
    {
        base.Initialize(type);

        this.owner = owner;
        this.damage = damage;
        this.extent = extent;
        this.targetLayer = targetLayer;

        transform.localScale = Vector3.one * extent;
    }

    public void OnDamageInRange()
    {
        Vector2 size = new Vector2(extent, extent * 1.5f);
        Collider2D[] hits = Physics2D.OverlapCapsuleAll(transform.position, size, CapsuleDirection2D.Vertical, 0, targetLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var component))
            {
                component.TakeDamage(damage, owner);
                //NotifyHitResult(component);
                owner.Condition.Add(AttributeType.Exp, 10);
            }
        }
    }

    private void NotifyHitResult(IDamageable target)
    {
        HitInfo hit = new HitInfo
        {
            Owner = owner,
            Target = target,
            Position = transform.position,
            Damage = damage,
            IsCritical = false,
            ProjectileType = ProjectileType.PlayerExplosion,
        };

        owner.Controller.RaiseHit(hit);

        if (target is IKillable killable && killable.IsDead)
        {
            owner.Controller.RaiseKill(hit);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            Quaternion.Euler(0, 0, transform.eulerAngles.z),
            Vector3.one
            );

        Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
    }
}
