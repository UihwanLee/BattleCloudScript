using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class HomingProjectile : BasicProjectile
{
    private Transform target;
    private Collider2D exclude;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == exclude)
            return;

        base.OnTriggerEnter2D(collision);
    }

    protected override void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            target = owner.Controller.FindNearestTarget(exclude);

            if (target == null)
            {
                poolManager.Release(projectileType, this);
                return;
            }
        }

        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        Move();
    }

    protected override void OnHit(IDamageable target)
    {
        target.TakeDamage(damage, owner);

        poolManager.Release(projectileType, this);
    }

    public void SetExclude(Collider2D exclude)
    {
        this.exclude = exclude;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
