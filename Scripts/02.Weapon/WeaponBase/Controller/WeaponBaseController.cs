using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using WhereAreYouLookinAt.Enum;

public struct HitInfo
{
    public WeaponBase Owner;
    public IDamageable Target;
    public Vector3 Position;
    public float Damage;
    public bool IsCritical;
    public ProjectileType ProjectileType;
}

/// <summary>
/// 기본 무기의 Controller <br/>
/// tree의 tick, pivot으로 이동 기능만 수행
/// </summary>
public abstract class WeaponBaseController : MonoBehaviour
{
    [Header("Target Layer")]
    [SerializeField] protected LayerMask target;
    [SerializeField] private LayerMask monsterLayer;

    protected BehaviorTree tree;
    protected Transform pivot;

    protected WeaponBase weapon;
    protected WeaponStat stat;

    [Header("추적 지연 시간")]
    [SerializeField] private float followDelay = 0.2f;
    private Vector3 smoothVelocity;
    private Vector3 delayedPivot;

    private bool isInitialized = false;

    [Header("가속 구간")]
    [SerializeField] private float accelDistance = 0.5f;
    [Header("감속 구간")]
    [SerializeField] private float decelDistance = 1.5f;
    [Header("현재 속도")]
    [SerializeField] private float currentSpeed = 0f;

    public event Action<HitInfo> OnHit;
    public event Action<HitInfo> OnKill;

    private Coroutine extraFireRoutine;
    private Coroutine periodicRoutine;
    private Coroutine explosionRoutine;
    private Coroutine freezeRoutine;
    private Coroutine periodicBombRoutine;

    private bool isFreeze = false;

    protected Transform currentTarget;
    private readonly List<Collider2D> validTargets = new List<Collider2D>();

    #region 프로퍼티
    public Transform Pivot => pivot;
    public Transform CurrentTarget => currentTarget;
    #endregion

    #region Life Cycle
    protected virtual void Reset()
    {
        target = LayerMask.GetMask("Monster");
        monsterLayer = LayerMask.GetMask("Monster");
    }

    protected virtual void Awake()
    {
        weapon = GetComponent<WeaponBase>();
        stat = weapon.Stat;
    }

    protected virtual void Start()
    {
        tree = CreateBehaviorTree();
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        tree?.Tick();

        if (!isInitialized)
        {
            InitializePosition();
            return;
        }

        UpdateDelayedPivot();
        MoveToDelayedPivot();
    }

    protected virtual void OnDisable()
    {
        OnHit = null;
        OnKill = null;
    }
    #endregion

    #region Movement
    private void InitializePosition()
    {
        if (pivot == null) return;

        delayedPivot = pivot.position;
        transform.position = pivot.position;
        smoothVelocity = Vector3.zero;

        isInitialized = true;
    }

    /// <summary>
    /// followDelay만큼 지연된 Pivot 계산
    /// </summary>
    private void UpdateDelayedPivot()
    {
        if (pivot == null) return;

        delayedPivot = Vector3.SmoothDamp(
            delayedPivot,
            pivot.position,
            ref smoothVelocity,
            followDelay
            );
    }

    public void FreezeFor(float duration)
    {
        if (freezeRoutine != null)
        {
            StopCoroutine(freezeRoutine);
        }

        freezeRoutine = StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        isFreeze = true;

        yield return new WaitForSeconds(duration);

        isFreeze = false;

        freezeRoutine = null;
    }

    /// <summary>
    /// delayedPivot을 목표로 이동
    /// </summary>
    private void MoveToDelayedPivot()
    {
        if (isFreeze) return;

        if (delayedPivot == null || stat.MoveSpeed == null) return;

        float maxSpeed = stat.MoveSpeed.Value;
        float distance = Vector3.Distance(transform.position, delayedPivot);

        // 거리 기반 속도 비율 계산 (0 ~ 1)
        float speedFactor = 1f;

        if (distance < decelDistance)
        {
            // 도착 직전 감속
            speedFactor = Mathf.InverseLerp(0f, decelDistance, distance);
        }
        else if (distance < accelDistance)
        {
            // 출발 직후 가속
            speedFactor = Mathf.InverseLerp(0f, accelDistance, distance);
        }
        else
        {
            speedFactor = 1f;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * speedFactor, Time.deltaTime * 5f);
        transform.position = Vector3.MoveTowards(transform.position, delayedPivot, currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 무기의 기본 위치(목적지) 세팅
    /// </summary>
    /// <param name="pivot">무기의 기본 위치</param>
    public void SetPivot(Transform pivot)
    {
        this.pivot = pivot;
    }

    public void TeleportationToPivot()
    {
        delayedPivot = pivot.position;
        transform.position = pivot.position;
    }
    #endregion

    #region Event
    public void RaiseHit(HitInfo hitInfo)
    {
        OnHit?.Invoke(hitInfo);
    }

    public void RaiseKill(HitInfo hitInfo)
    {
        OnKill?.Invoke(hitInfo);
    }
    #endregion

    public Transform FindRandomTarget(Collider2D exclude = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stat.Range.Value, monsterLayer);

        if (hits == null || hits.Length == 0)
            return null;

        validTargets.Clear();

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (hit == null)
                continue;

            if (hit == exclude)
                continue;

            if (hit.TryGetComponent<IKillable>(out var killable))
            {
                if (killable.IsDead)
                    continue;
            }

            validTargets.Add(hit);
        }

        if (validTargets.Count == 0)
            return null;

        Transform randomTarget = null;
        int rand = UnityEngine.Random.Range(0, validTargets.Count);
        randomTarget = validTargets[rand].transform;

        return randomTarget;
    }

    /// <summary>
    /// 사거리 내의 가장 가까운 적 탐색
    /// </summary>
    /// <returns>가장 가까운 적</returns>
    public Transform FindNearestTarget(Collider2D exclude = null)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stat.Range.Value, target);

        if (hits.Length == 0)
        {
            currentTarget = null;
            return null;
        }

        float minDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (Collider2D hit in hits)
        {
            if (hit == exclude)
                continue;

            if (hit.TryGetComponent<IKillable>(out var killable))
            {
                if (killable.IsDead)
                    continue;
            }

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestTarget = hit.transform;
            }
        }

        currentTarget = nearestTarget;
        return nearestTarget;
    }

    #region Item Effect
    public void StartExtraFire(int shotCount, float damage, Transform target, float interval)
    {
        if (extraFireRoutine != null)
            StopCoroutine(extraFireRoutine);

        extraFireRoutine = StartCoroutine(ExtraFireRoutine(shotCount, damage, target, new WaitForSeconds(interval)));
    }

    private IEnumerator ExtraFireRoutine(int shotCount, float damage, Transform target, WaitForSeconds waitForSeconds)
    {
        for (int i = 0; i < shotCount; i++)
        {
            FireExtraShot(damage, target);
            yield return waitForSeconds;
        }

        extraFireRoutine = null;
    }

    private void FireExtraShot(float damage, Transform target)
    {
        //Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stat.Range.Value, monsterLayer);

        //if (hits == null || hits.Length == 0)
        //    return;

        //validTargets.Clear();

        //for (int i = 0; i < hits.Length; i++)
        //{
        //    Collider2D hit = hits[i];

        //    if (hit == null)
        //        continue;

        //    if (hit.TryGetComponent<IKillable>(out var killable))
        //    {
        //        if (killable.IsDead)
        //            continue;
        //    }

        //    validTargets.Add(hit);
        //}

        //if (validTargets.Count == 0)
        //    return;

        //Transform randomTarget = null;
        //int rand = UnityEngine.Random.Range(0, validTargets.Count);
        //randomTarget = validTargets[rand].transform;

        //Transform randomTarget = FindRandomTarget();
        if (target == null)
            return;

        BasicProjectile projectile = ProjectilePoolManager.Instance.GetProjectile(ProjectileType.PlayerExtra);

        Vector3 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        projectile.Initialize(
            owner: weapon,
            targetLayer: monsterLayer,
            type: ProjectileType.PlayerExtra,
            existenceDistance: stat.Range.Value,
            damage: damage,
            knockbackPower: stat.KnockBack.Value,
            pierceCount: 0
            );

        projectile.gameObject.SetActive(true);
    }

    public void StartExplosionWithDelay(ImpactType type, Vector3 position, float damage, float range, float delay, LayerMask layerMask)
    {
        explosionRoutine = StartCoroutine(ExplosionRoutine(type, position, damage, range, new WaitForSeconds(delay), layerMask));
    }

    private IEnumerator ExplosionRoutine(ImpactType type, Vector3 position, float damage, float range, WaitForSeconds waitForSeconds, LayerMask layerMask)
    {
        yield return waitForSeconds;

        SpawnExplosion(type, position, damage, range, layerMask);
    }

    private void SpawnExplosion(ImpactType type, Vector3 position, float damage, float range, LayerMask layerMask)
    {
        var explosion = ProjectilePoolManager.Instance.GetImpact(type);

        if (explosion.TryGetComponent<ExplosionImpact>(out var explosionImpact))
        {
            explosionImpact.Initialize(type, weapon, damage, range, layerMask);
            explosionImpact.gameObject.transform.position = position;
            explosionImpact.gameObject.SetActive(true);
        }
    }

    public void StartPeriodicAttack(float period, int shotCount, float damageMultiplier, Transform target, float interval)
    {
        StopPeriodicAttack();
        periodicRoutine = StartCoroutine(PeriodicAttackRoutine(new WaitForSeconds(period), target, shotCount, damageMultiplier, new WaitForSeconds(interval)));
    }

    public void StopPeriodicAttack()
    {
        if (periodicRoutine != null)
        {
            StopCoroutine(periodicRoutine);
            periodicRoutine = null;
        }
    }

    private IEnumerator PeriodicAttackRoutine(WaitForSeconds waitForSecondsPeriod, Transform target, int shotCount, float damageMultiplier, WaitForSeconds waitForSecondsInterval)
    {
        while (true)
        {
            yield return waitForSecondsPeriod;
            for (int i = 0; i < shotCount; i++)
            {
                FireExtraShot(weapon.Stat.Damage * damageMultiplier, target);
                yield return waitForSecondsInterval;
            }
        }
    }

    public void StartPeriodicBomb(float period, ImpactType type, float damageMultiplier, float range)
    {
        StopPeriodicAttack();
        periodicBombRoutine = StartCoroutine(PeriodicBombRoutine(new WaitForSeconds(period), type, damageMultiplier, range));
    }

    public void StopPeriodicBomb()
    {
        StopCoroutine(periodicBombRoutine);
    }

    public IEnumerator PeriodicBombRoutine(WaitForSeconds waitForSeconds, ImpactType type, float damageMultiplier, float range)
    {
        while (true)
        {
            Transform target = FindNearestTarget();

            if (target == null)
            {
                yield return waitForSeconds;
                continue;
            }

            Vector3 targetPos = target.position;

            var bomb = ProjectilePoolManager.Instance.GetImpact(type);

            if (bomb.TryGetComponent<ExplosionImpact>(out var explosion))
            {
                explosion.Initialize(type, weapon, damageMultiplier * weapon.Stat.Damage, range, this.target);
                explosion.transform.position = targetPos;
            }

            yield return waitForSeconds;
        }
    }

    public void SpawnThunder(ImpactType type, float damage)
    {
        Transform targetTr = FindRandomTarget();

        if (targetTr == null)
            return;

        IDamageable target = targetTr.GetComponent<IDamageable>();

        var thunder = ProjectilePoolManager.Instance.GetImpact(type);

        if (thunder.TryGetComponent<ThunderImapct>(out var thunderImpact))
        {
            thunderImpact.Initialize(type, weapon, damage, target);
            thunderImpact.transform.parent.position = target.Transform.position;
            thunderImpact.transform.parent.gameObject.SetActive(true);
        }
    }
    #endregion

    public float CalculateFinalDamage()
    {
        float value1 = weapon.Trait.Scale;
        float value2 = GameManager.Instance.Player.Stat.Command.Value;
        float value3 = value1 * value2;
        float finalDamage = value3 + stat.Attack.Value;

        return weapon.Stat.Damage;
    }

    /// <summary>
    /// Behavior Tree 생성
    /// </summary>
    /// <returns></returns>
    protected abstract BehaviorTree CreateBehaviorTree();
}
