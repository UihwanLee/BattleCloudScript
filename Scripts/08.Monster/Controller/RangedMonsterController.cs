using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class RangedMonsterController : MonsterController
{
    private float attackTimer;
    private ProjectilePoolManager projectilePoolManager;

    [SerializeField] private Transform firePoint;
    private float firePointRadius = 0.3f;

    [SerializeField] private ProjectileType projectileType;
    private List<MonsterProjectile> projectiles = new();

    [Header("도망 시작 거리")]
    [SerializeField] private float fleeDistance;

    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly int ISMOVE = Animator.StringToHash("IsMove");
    private bool isAttacking;

    protected override void Start()
    {
        base.Start();

        projectilePoolManager = ProjectilePoolManager.Instance;
        attackTimer = stat.AttackInterval.Value;
    }

    protected override void UpdateAI()
    {
        attackTimer -= Time.deltaTime;

        float dist = Vector2.Distance(transform.position, playerTr.position);

        // 너무 가까울 때 도망
        if (dist <= fleeDistance)
        {
            Vector2 fleeDir = (transform.position - playerTr.position).normalized;

            Vector2 separation = CalculateSeparation();

            moveDirection = (fleeDir + separation).normalized;

            monster.AnimationHandler.PlayAnimationWithBool(ISMOVE, true);
            return;
        }

        // 공격 가능 거리 -> 공격
        if (dist <= stat.Range.Value)
        {
            moveDirection = Vector2.zero;
            monster.AnimationHandler.PlayAnimationWithBool(ISMOVE, false);

            if (attackTimer <= 0f)
            {
                isAttacking = true;

                if (DirToPlayer().x < 0)
                    spriteRenderer.flipX = true;
                else if (DirToPlayer().x > 0)
                    spriteRenderer.flipX = false;

                monster.AnimationHandler.PlayAnimationWithTrigger(ATTACK);
                monster.VisualEffect.PlayAttackEffect(0.5f);
                attackTimer = stat.AttackInterval.Value;
            }

            return;
        }

        // 접근
        moveDirection = GetMoveDirection();
        monster.AnimationHandler.PlayAnimationWithBool(ISMOVE, true);
    }

    protected override void Flip()
    {
        if (spriteRenderer == null) return;

        if (isAttacking) return;

        // 이동 중이 아닐 때는 플립 금지
        if (Mathf.Abs(moveDirection.x) < 0.01f)
            return;

        if (moveDirection.x < 0)
            spriteRenderer.flipX = true;
        else if (moveDirection.x > 0)
            spriteRenderer.flipX = false;
    }

    public void Fire()
    {
        RotateFirePoint();

        MonsterProjectile projectile = projectilePoolManager
            .GetProjectile(projectileType)
            .GetComponent<MonsterProjectile>();

        projectiles.Add(projectile);

        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;

        projectile.Initialize(stat.Range.Value, stat.Damage.Value, 10f);
        projectile.gameObject.SetActive(true);

        isAttacking = false;
    }

    private void RotateFirePoint()
    {
        if (playerTr == null)
            return;

        Vector3 dir = (playerTr.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        firePoint.position = transform.position + dir * firePointRadius;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    public override void Despawn()
    {
        foreach (MonsterProjectile projectile in projectiles)
        {
            if (projectile.gameObject.activeInHierarchy)
                projectile.Release();
        }

        projectiles.Clear();

        base.Despawn();
    }
}
