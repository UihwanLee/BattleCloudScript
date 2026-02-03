using UnityEngine;
using WhereAreYouLookinAt.Enum;

public static class WeaponEffectFactory
{
    public static IWeaponEffectInstance Create(Effect effect, int id)
    {
        switch (effect.EffectType)
        {
            #region Status Effect
            case ItemEffectType.DAMAGE_FLAT:
                return new StatChangeWithFlatEffect(id, AttributeType.Damage, effect);
            case ItemEffectType.DAMAGE_PERCENT:
                return new StatChangeWithPercentageEffect(id, AttributeType.Damage, effect);
            case ItemEffectType.MOVE_SPEED_FLAT:
                return new StatChangeWithFlatEffect(id, AttributeType.MoveSpeed, effect);
            case ItemEffectType.MOVE_SPEED_PERCENT:
                return new StatChangeWithPercentageEffect(id, AttributeType.MoveSpeed, effect);
            case ItemEffectType.ATTACK_INTERVAL_FLAT:
                return new StatChangeWithFlatEffect(id, AttributeType.AttackInterval, effect);
            case ItemEffectType.ATTACK_INTERVAL_PERCENT:
                return new StatChangeWithPercentageEffect(id, AttributeType.AttackInterval, effect);
            case ItemEffectType.RANGE_FLAT:
                return new StatChangeWithFlatEffect(id, AttributeType.Range, effect);
            case ItemEffectType.RANGE_PERCENT:
                return new StatChangeWithPercentageEffect(id, AttributeType.Range, effect);
            case ItemEffectType.KNOCKBACK_FLAT:
                return new StatChangeWithFlatEffect(id, AttributeType.KnockBack, effect);
            case ItemEffectType.KNOCKBACK_PERCENT:
                return new StatChangeWithPercentageEffect(id, AttributeType.KnockBack, effect);
            #endregion

            #region On Hit Effect
            case ItemEffectType.ON_HIT_HEAL:
                return new HealOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_STACK_AOE_PERCENT:
                return new StackAOEWithPercentageOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_STACK_AOE_FLAT:
                return new StackAOEWithFlatOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_SELF_RISK_EXPLODE_PERCENT:
                return new SelfDamageExplosionOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_HEAL_RISK_DAMAGE_PERCENT:
                return new HealWithDamageReductionOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_EXTRA_SHOT_CHANCE:
                return new ExtraProjectileOnHitWithPercentageDamageEffect(id, effect);
            case ItemEffectType.ON_HIT_EXTRA_SHOT_CHANCE_FLAT:
                return new ExtraProjectileOnHitWithFlatDamageEffect(id, effect);
            case ItemEffectType.ON_HIT_CHAIN_LIGHTNING:
                return new ThunderImpactWithPercentageOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_CHAIN_LIGHTNING_FLAT:
                return new ThunderImpactWithFlatOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_HOMING_ENABLE:
                return new HomingAttackOnHitEffect(id, effect);
            case ItemEffectType.ON_HIT_DRAIN:
                return new DrainOnHitEffect(id, effect);
            #endregion

            #region On Kill Effect
            case ItemEffectType.ON_KILL_GROW_DAMAGE:
                return new GrowDamageOnKillEffect(id, effect);
            case ItemEffectType.ON_KILL_GROW_DAMAGE_RISK:
                return new GrowDamageWithRiskOnKill(id, effect);
            case ItemEffectType.ON_KILL_GROW_DAMAGE_HP_RISK:
                return new GrowDamageWithMaxHPRiskOnKill(id, effect);
            case ItemEffectType.ON_KILL_GROW_DAMAGE_MOVE_SPEED_RISK:
                return new GrowDamageWithSpeedRiskOnKill(id, effect);
            case ItemEffectType.ON_KILL_EXPLOSION:
                return new ExplosionOnKillEffect(id, effect);
            #endregion

            #region ON Passive Effect
            case ItemEffectType.ON_PASSIVE_MULTI_SHOT:
                return new ProjectileSpreadExpansionEffect(id, effect);
            case ItemEffectType.ON_PASSIVE_PIERCE_COUNT:
                return new AdditionalPierceCountEffect(id, effect);
            case ItemEffectType.ON_PASSIVE_MEGA_EXPLOSION:
                return new PeriodicBombEffect(id, effect);
            #endregion

            #region ETC Effect
            case ItemEffectType.ON_HIT_PERIODIC_ATTACK:
                return new PeriodicBombEffect(id, effect);
            #endregion

            #region Exclusive Effect
            case ItemEffectType.Base:
                return new BaseExclusiveEffect(id, effect);
            case ItemEffectType.Piercing:
                return new PiercingExclusiveEffect(id, effect);
            case ItemEffectType.AreaDebuffer:
                return new ZoneDebufferExclusiveEffect(id, effect);
            case ItemEffectType.Shotgun:
                return new ShotGunExclusiveEffect(id, effect);
            case ItemEffectType.RapidFire:
                return new RapidFireExclusiveEffect(id, effect);
                #endregion
        }

        return null;
    }
}
