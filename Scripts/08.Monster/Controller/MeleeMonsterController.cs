using UnityEngine;

public class MeleeMonsterController : MonsterController
{
    private PlayerCondition contactPlayer;
    private float damageTimer;

    protected override void UpdateAI()
    {
        moveDirection = GetMoveDirection();

        if (contactPlayer != null)
        {
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0f)
                DealDamage();
        }
    }

    private void DealDamage()
    {
        if (contactPlayer == null)
            return;

        damageTimer = monster.Stat.AttackInterval.Value;
        contactPlayer.TakeDamage(stat.Damage.Value);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerCondition>(out var playerCondition))
            return;

        contactPlayer = playerCondition;
        playerCondition.TakeDamage(stat.Damage.Value);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerCondition>(out var playerCondition))
            return;

        contactPlayer = null;
    }
}
