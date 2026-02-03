using System.Collections;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(Transform other, float damage, Color? color = null);
    public void TakeDamage(float damage, WeaponBase weaponBase, Color? color = null);

    public Transform Transform { get; }
    public Collider2D Collider { get; }
}
