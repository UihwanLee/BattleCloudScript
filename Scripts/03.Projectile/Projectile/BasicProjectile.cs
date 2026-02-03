using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

/// <summary>
/// 기본 투사체
/// 상속 시 moveSpeed, type에 대한 초기화 필요
/// </summary>
public class BasicProjectile : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float existenceDistance;
    [SerializeField] protected float damage;
    [SerializeField] protected float knockbackPower;
    [SerializeField] protected int pierceCount;

    [Header("Projectile Type")]
    [SerializeField] protected ProjectileType projectileType;
    [SerializeField] protected LayerMask targetLayer;

    [Header("Impact")]
    [SerializeField] protected ImpactType impactType;

    protected Vector3 startPosition;

    protected ProjectilePoolManager poolManager;
    protected WeaponBase owner;

    protected HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();
    protected int lastHitFrame = -1;

    protected virtual void Reset()
    {
        moveSpeed = 10f;
        impactType = ImpactType.None;
    }

    protected virtual void Start()
    {
        poolManager = ProjectilePoolManager.Instance;
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        Move();
        CheckLifeTime();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 최초의 충돌에만 충돌 효과 구현
        if (lastHitFrame == Time.frameCount)
            return;

        if ((targetLayer & (1 << collision.gameObject.layer)) == 0)
            return;

        if (collision.TryGetComponent<IDamageable>(out var target))
        {
            if (hitTargets.Contains(target))
                return;

            hitTargets.Add(target);

            lastHitFrame = Time.frameCount;

            OnHit(target);

            // 넉백 적용
            if (collision.TryGetComponent<IKnockbackable>(out var knockbackable))
            {
                knockbackable.ApplyKnockback(transform, knockbackPower);
            }
        }
    }

    /// <summary>
    /// 투사체 정보 초기화
    /// </summary>
    /// <param name="targetLayer">공격하려는 타겟의 Layer</param>
    /// <param name="type">투사체 타입</param>
    /// <param name="existenceDistance">무기의 사거리(투사체가 사거리 이상 움직였다면 삭제)</param>
    /// <param name="damage">무기의 공격력</param>
    /// <param name="knockbackPower">무기가 넉백시키는 힘</param>
    public virtual void Initialize(WeaponBase owner, LayerMask targetLayer, ProjectileType type, float existenceDistance, float damage, float knockbackPower, int pierceCount)
    {
        this.owner = owner;
        this.targetLayer = targetLayer;
        this.projectileType = type;
        this.existenceDistance = existenceDistance + 1f;
        this.damage = damage;
        this.knockbackPower = knockbackPower;
        this.pierceCount = pierceCount;

        startPosition = transform.position;
        //hasHit = false;

        hitTargets.Clear();
    }

    protected virtual void Move()
    {
        // 투사체 이동.
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    protected virtual void CheckLifeTime()
    {
        // 사거리 이상 이동하면 삭제
        float dist = Vector3.Distance(startPosition, transform.position);

        if (dist > existenceDistance)
        {
            poolManager.Release(projectileType, this);
            return;
        }
    }

    /// <summary>
    /// 충돌 효과 메서드 <br/>
    /// TakeDamage 또는 이펙트 생성
    /// </summary>
    /// <param name="monster">투사체와 충돌이 일어난 타겟 몬스터</param>
    protected virtual void OnHit(IDamageable target)
    {
        pierceCount--;

        target.TakeDamage(damage, owner);

        NotifyHitResult(target);

        if (pierceCount < 0)
        {
            // Projectile Release
            poolManager.Release(projectileType, this);
        }
    }

    /// <summary>
    /// Hit의 결과를 알려주는 메서드 <br/>
    /// 이벤트 전파 역할
    /// </summary>
    /// <param name="target"></param>
    protected virtual void NotifyHitResult(IDamageable target)
    {
        HitInfo hit = new HitInfo
        {
            Owner = owner,
            Target = target,
            Position = transform.position,
            Damage = damage,
            IsCritical = false,
            ProjectileType = projectileType,
        };

        owner.Controller.RaiseHit(hit);

        if (target is IKillable killable && killable.IsDead)
        {
            owner.Controller.RaiseKill(hit);
        }
    }

    protected virtual ImpactBase CreateImpact(ImpactType type)
    {
        // Create Impact
        ImpactBase impact = poolManager.GetImpact(impactType);

        impact.gameObject.transform.position = transform.position;
        impact.gameObject.transform.rotation = transform.rotation;

        impact.Initialize(type);

        impact.gameObject.SetActive(true);

        return impact;
    }
}
