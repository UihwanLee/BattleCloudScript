using UnityEngine;

public class MeleeAttackCollider : MonoBehaviour
{
    private float damage;

    private bool isAttackActive;
    private float offsetX;

    private void Awake()
    {
        offsetX = 0.75f;
    }

    public void Init(float damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttackActive)
            return;

        if (collision.TryGetComponent<Player>(out var player))
        {
            player.Condition.TakeDamage(damage);
            isAttackActive = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isAttackActive)
            return;

        if (collision.TryGetComponent<Player>(out var player))
        {
            player.Condition.TakeDamage(damage);
            isAttackActive = false;
        }
    }

    public void StartAttack()
    {
        isAttackActive = true;
    }

    public void EndAttack()
    {
        isAttackActive = false;
    }

    public void FlipX(bool enabled)
    {
        Vector3 pos = transform.localPosition;

        if (enabled)
        {
            pos.x = -offsetX;
        }
        else
        {
            pos.x = offsetX;
        }
        transform.localPosition = pos;
    }
}
