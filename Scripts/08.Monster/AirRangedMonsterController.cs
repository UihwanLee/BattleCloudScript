using UnityEngine;

public class AirRangedMonsterController : AirMonsterController
{
    [Header("투사체 설정")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float projectileKnockback;

    protected override void Attack()
    {
        if (player == null)
            return;

        if (PoolManager.Instance == null)
            return;

        GameObject obj = PoolManager.Instance.GetObject("MonsterProjectile");
        if (obj == null)
            return;

        OldMonsterProjectile projectile = obj.GetComponent<OldMonsterProjectile>();
        if (projectile == null)
            return;

        // 방향 계산 (Z 고정)
        Vector3 dir = player.position - transform.position;
        dir.z = 0f;
        dir = dir.normalized;

        // 콜라이더 밖에서 생성
        Vector3 spawnPos = transform.position + dir * 1.2f;
        spawnPos.z = 0f;

        Transform t = projectile.transform;
        t.SetParent(null, false);
        t.position = spawnPos;

        projectile.Initialize(
            dir,
            monster.Stat.Damage.Value,
            projectileKnockback,
            monster.Stat.Range.Value,
            targetLayer,
            transform   // 발사자
        );
    }
}
