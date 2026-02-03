using UnityEngine;

public class RapidFireWeaponController : BasicWeaponController
{
    [Header("확산 각도 - 연사")]
    [SerializeField] float spreadAngleForRapid;

    protected override void RotateFirePoint()
    {
        if (CurrentTarget == null) return;

        Vector3 baseDir = (CurrentTarget.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        float randomAngle = baseAngle + Random.Range(-spreadAngleForRapid / 2f, spreadAngleForRapid / 2f);

        // 회전된 방향 벡터 재계산
        Vector3 spreadDir = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad),
            0f
        );

        Debug.DrawRay(
            transform.position,
            baseDir * 2f,
            Color.blue,
            0.1f
            );

        Debug.DrawRay(
            transform.position,
            spreadDir * 2f,
            Color.red,
            0.1f
            );

        firePoint.position = transform.position + spreadDir * firePointRadius;
        firePoint.rotation = Quaternion.Euler(0, 0, randomAngle);
    }
}
