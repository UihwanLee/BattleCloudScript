using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class MonsterProjectile : BasicProjectile
{
    protected override void Reset()
    {
        impactType = ImpactType.None;
        projectileType = ProjectileType.MonsterBone;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            player.Condition.TakeDamage(damage, owner);
            poolManager.Release(projectileType, this);
        }
    }

    public void Initialize(float existenceDistance, float damage, float speed)
    {
        this.existenceDistance = existenceDistance;
        this.damage = damage;
        moveSpeed = speed;

        startPosition = transform.position;

    }

    public void Release()
    {
        ProjectilePoolManager.Instance.Release(projectileType, this);
    }
}
