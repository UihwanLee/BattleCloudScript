using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ThunderImapct : ImpactBase
{
    private float damage;
    private IDamageable target;
    private WeaponBase owner;

    public void Initialize(ImpactType type, WeaponBase owner, float damage, IDamageable target)
    {
        base.Initialize(type);

        this.owner = owner;
        this.damage = damage;
        this.target = target;
    }

    public void OnDamage()
    {
        if (target == null) 
            return;

        target.TakeDamage(damage, owner);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.TryGetComponent<IDamageable>(out var damageable))
    //    {
    //        if (damageable != target)
    //            return;

    //        damageable.TakeDamage((int)damage, owner);
    //    }
    //}
}
