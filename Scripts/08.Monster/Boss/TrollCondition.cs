using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class TrollCondition : BaseCondition, IKillable
{
    [Header("최대 체력 설정")]
    [SerializeField] private float _maxHp;

    public bool IsDead { get; private set; }
    private TrollController controller;
    private MonsterVisualEffect monsterVisualEffect;

    private void Reset()
    {
        _maxHp = 20f;
    }

    private void Awake()
    {
        controller = GetComponent<TrollController>();
        monsterVisualEffect = GetComponentInChildren<MonsterVisualEffect>();

        InitCondition(_maxHp);
    }

    private void OnEnable()
    {
        IsDead = false;
    }

    public override void TakeDamage(float damage, WeaponBase weaponBase = null, Color? color = null)
    {
        if (IsDead)
            return;

        base.TakeDamage(damage, weaponBase, color);

        // 효과음
        AudioManager.Instance.PlayClip(SFXType.MonsterHit);

        GameManager.Instance.ParticleManager.PlayHitEffect(transform);

        monsterVisualEffect.PlayHitEffect();

        if (hp.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EventBus.OnMonsterKilled?.Invoke(1f);
        IsDead = true;
        controller.Death();
    }
}
