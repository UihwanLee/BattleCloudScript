using UnityEngine;
using UnityEngine.Localization.SmartFormat.Utilities;
using WhereAreYouLookinAt.Enum;

public class HealWeaponController : BasicWeaponController
{
    private PlayerCondition player;

    protected override void Reset()
    {
        base.Reset();

        projectileType = ProjectileType.PlayerHeal;
        target = LayerMask.GetMask("Player");
    }

    protected override void Start()
    {
        base.Start();

        player = GameManager.Instance.Player.Condition;
    }

    public override void FireOneShot()
    {
        if (!GameManager.Instance.Player.Controller.GetMovementEnabled())
            return;

        FloatingTextPoolManager.Instance.SpawnText(FloatingTextType.Heal.ToString(), stat.Damage.ToString("0"), player.transform);

        player.Add(AttributeType.Hp, stat.Damage);
        player.PlayHealEffect();
    }
}
