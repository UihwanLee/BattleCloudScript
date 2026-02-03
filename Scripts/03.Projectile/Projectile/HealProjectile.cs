using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class HealProjectile : BasicProjectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetLayer & (1 << collision.gameObject.layer)) == 0)
            return;

        if (collision.TryGetComponent<PlayerCondition>(out var condition))
        {
            OnHit(condition);
        }
    }

    private void OnHit(PlayerCondition condition)
    {
        condition.Add(AttributeType.Hp, damage);
        condition.PlayHealEffect();

        NotifyHitResult(condition);

        if (!owner.IsInBase)
            owner.Condition.Add(AttributeType.Exp, 10);

        poolManager.Release(projectileType, this);
    }
}
