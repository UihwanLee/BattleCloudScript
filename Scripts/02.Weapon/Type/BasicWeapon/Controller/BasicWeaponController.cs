using System.Collections;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class BasicWeaponController : WeaponBaseController
{
    [Header("Projectile")]
    [SerializeField] protected ProjectileType projectileType;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float firePointRadius;

    [Header("Shooting")]
    [SerializeField] protected float cooldownTimer = 0f;
    //[SerializeField] protected Transform currentTarget;
    [SerializeField] protected int shotCount;
    [SerializeField] protected float shotInterval;

    [Header("확산 각도")]
    [SerializeField] protected float spreadAngle;
    [Header("공격 당 발사되는 투사체 수")]
    [SerializeField] protected float projectilesPerShot;
    [Header("관통 횟수")]
    [SerializeField] protected int pierceCount;
    //[Header("아이템 효과로 늘어나는 각도")]
    //[SerializeField] protected float spreadAngleBonus;

    protected ProjectilePoolManager poolManager;

    protected Coroutine fireRoutine;

    private bool hasHighDamageExclusiveItem = false;
    private float freezeDuration;

    #region 프로퍼티
    public float AttackRange => stat.Range.Value;
    #endregion

    #region Life Cycle
    protected override void Reset()
    {
        base.Reset();

        firePoint = transform.Find("FirePoint");
        firePointRadius = 0.3f;
        projectileType = ProjectileType.PlayerBasic;
        shotCount = 1;
        shotInterval = 0.1f;
    }

    protected override void Start()
    {
        base.Start();
        poolManager = ProjectilePoolManager.Instance;
    }

    protected override void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        cooldownTimer += Time.deltaTime;
        base.Update();
    }
    #endregion

    protected override BehaviorTree CreateBehaviorTree()
    {
        return new BehaviorTree(
            new Selector(
                new Sequence(
                    new IsCooldownReadyNode(this),
                    new FindNearestTargetNode(this),
                    new IsTargetInRangeNode(this),
                    new ShootProjectileNode(this)
                    )
                )
            );
    }

    /// <summary>
    /// 공격 가능 여부 확인
    /// </summary>
    /// <returns>공격 가능 여부</returns>
    public bool IsCooldownReady()
    {
        return cooldownTimer >= stat.AttackInterval.Value;
    }

    /// <summary>
    /// 공격 쿨타임 초기화
    /// </summary>
    public void ResetCooldown()
    {
        cooldownTimer = 0f;
    }

    public virtual void StartFire()
    {
        if (fireRoutine != null)
            StopCoroutine(fireRoutine);

        fireRoutine = StartCoroutine(FireRoutine(new WaitForSeconds(shotInterval)));
    }

    private IEnumerator FireRoutine(WaitForSeconds waitForSeconds)
    {
        for (int i = 0; i < shotCount; i++)
        {
            FireOneShot();
            yield return waitForSeconds;
        }

        fireRoutine = null;
    }

    /// <summary>
    /// 투사체 발사
    /// </summary>
    public virtual void FireOneShot()
    {
        RotateFirePoint();

        float startAngleZ = firePoint.rotation.eulerAngles.z - (spreadAngle / projectilesPerShot);
        float angleStep = spreadAngle / projectilesPerShot;

        for (int i = 0; i < projectilesPerShot; i++)
        {
            BasicProjectile projectile = poolManager.GetProjectile(projectileType);

            projectile.transform.position = firePoint.position;

            Vector3 newRotation = Vector3.zero;
            newRotation.z = startAngleZ + (angleStep * i);

            projectile.transform.rotation = Quaternion.Euler(newRotation);

            float finalDamage = CalculateFinalDamage();

            projectile.Initialize(
                owner: weapon,
                targetLayer: target,
                type: projectileType,
                existenceDistance: stat.Range.Value,
                damage: finalDamage,
                knockbackPower: stat.KnockBack.Value,
                pierceCount: pierceCount
                );

            projectile.gameObject.SetActive(true);
        }

        AudioManager.Instance.PlayClip(SFXType.PlayerShot);

        if (hasHighDamageExclusiveItem)
            FreezeFor(freezeDuration);
    }

    protected virtual void RotateFirePoint()
    {
        if (CurrentTarget == null) return;
        
        Vector3 dir = (CurrentTarget.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        firePoint.position = transform.position + dir * firePointRadius;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetEnabledHighDamageExclusiveItem(bool enabled, float duration)
    {
        hasHighDamageExclusiveItem = enabled;
        freezeDuration = duration;
    }

    public void AddShotCount(int count)
    {
        shotCount += count;
    }

    public void SubShotCount(int count)
    {
        shotCount -= count;
    }

    public void AddSpreadAngle(float amount)
    {
        spreadAngle += amount;
    }

    public void SubSpreadAngle(float amount)
    {
        spreadAngle -= amount;
    }

    public void AddProjectilePerShot(int amount)
    {
        projectilesPerShot += amount;
    }

    public void SubProjectilePerShot(int amount)
    {
        projectilesPerShot -= amount;
    }

    public void AddPierceCount(int amount)
    {
        pierceCount += amount;
    }

    public void SubPierceCount(int amount)
    {
        pierceCount -= amount;
    }   
}
