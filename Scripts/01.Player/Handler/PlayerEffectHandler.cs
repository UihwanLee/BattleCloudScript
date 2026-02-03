using UnityEngine;
using WhereAreYouLookinAt.Enum;

// Item 효과를 적용하는 스크립트
public class PlayerEffectHandler : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    // Item 적용 - 적용할 타켓에 따라 메서드를 나눔
    public void ApplyItem(ItemApplyTarget target, string effect, float value, int index, bool add)
    {
    }

    public void ApplyItemByPlayer(EnhancementEffect type, float value, bool add)
    {
        float apply_value = (add) ? value : value * -1f;

        switch (type)
        {
            case EnhancementEffect.ADD_ITEM:
                {

                }
                break;
            case EnhancementEffect.ADD_COMMAND:
                {
                    player.Stat.Add(AttributeType.Command, apply_value);
                }
                break;
            case EnhancementEffect.ADD_MAX_HP:
                {
                    player.Condition.Add(AttributeType.MaxHp, apply_value);
                    player.Condition.Add(AttributeType.Hp, apply_value);
                }
                break;
            case EnhancementEffect.ADD_MOVE_SPEED:
                {
                    player.Stat.Add(AttributeType.MoveSpeed, apply_value);
                }
                break;
            case EnhancementEffect.ADD_DEFENSE:
                {
                    player.Stat.Add(AttributeType.Defense, apply_value);
                }
                break;
            case EnhancementEffect.ADD_EVASION:
                {
                    player.Stat.Add(AttributeType.Evasion, apply_value);
                }
                break;
            case EnhancementEffect.ADD_LUCK:
                {
                    player.Stat.Add(AttributeType.Luck, apply_value);
                }
                break;
            case EnhancementEffect.ADD_EXP_ALL_ADVISOR:
                {
                    player.PlayerSlotUI.GainExpAllWeapon(apply_value);
                }
                break;
            case EnhancementEffect.ADD_LEVEL_UP_ALL_ADVISOR:
                {
                    player.PlayerSlotUI.LevelUpAllWeapon();
                }
                break;
            case EnhancementEffect.ADD_REINFORCEMENT:
                {
                    player.Condition.Add(AttributeType.Gold, apply_value);
                }
                break;
            default:
                break;

        }
    }

    //public void ApplyItemByWeapon(Effect effect, bool add, int index)
    //{
    //    // 현재 index 슬롯에 있는 Weapon에 Item 효과 적용
    //    WeaponBase weapon = player.WeaponManager.GetWeapon(index);

    //    if (weapon == null) return;

    //    float apply_value = (add) ? effect.Value : effect.Value * -1f;

    //    AttributeType attributeType;

    //    switch (effect.EffectType)
    //    {
    //        case ItemEffectType.DAMAGE_FLAT:
    //            {
    //                attributeType = AttributeType.Damage;
    //                ApplyEffect(weapon, attributeType, apply_value, true);
    //            }
    //            break;
    //        case ItemEffectType.DAMAGE_PERCENT:
    //            {
    //                attributeType = AttributeType.Damage;
    //                ApplyEffect(weapon, attributeType, apply_value, false);
    //            }
    //            break;
    //        case ItemEffectType.TOTAL_DAMAGE_FLAT:
    //            {
    //                attributeType = AttributeType.Damage;
    //                ApplyEffect(weapon, attributeType, apply_value, true);
    //            }
    //            break;
    //        case ItemEffectType.TOTAL_DAMAGE_PERCENT:
    //            {
    //                attributeType = AttributeType.Damage;
    //                ApplyEffect(weapon, attributeType, apply_value, false);
    //            }
    //            break;
    //        case ItemEffectType.MOVE_SPEED_FLAT:
    //            {
    //                attributeType = AttributeType.MoveSpeed;
    //                ApplyEffect(weapon, attributeType, apply_value, true);
    //            }
    //            break;
    //        case ItemEffectType.MOVE_SPEED_PERCENT:
    //            {
    //                attributeType = AttributeType.MoveSpeed;
    //                ApplyEffect(weapon, attributeType, apply_value, false);
    //            }
    //            break;
    //        case ItemEffectType.ATTACK_INTERVAL_FLAT:
    //            {
    //                attributeType = AttributeType.AttackInterval;
    //                ApplyEffect(weapon, attributeType, apply_value, true);
    //            }
    //            break;
    //        case ItemEffectType.ATTACK_INTERVAL_PERCENT:
    //            {
    //                attributeType = AttributeType.AttackInterval;
    //                ApplyEffect(weapon, attributeType, apply_value, false);
    //            }
    //            break;
    //        case ItemEffectType.RANGE_FLAT:
    //            {
    //                attributeType = AttributeType.Range;
    //                ApplyEffect(weapon, attributeType, apply_value, true);
    //            }
    //            break;
    //        case ItemEffectType.RANGE_PERCENT:
    //            {
    //                attributeType = AttributeType.Range;
    //                ApplyEffect(weapon, attributeType, apply_value, false);
    //            }
    //            break;
    //        case ItemEffectType.KNOCKBACK_FLAT:
    //            {
    //                attributeType = AttributeType.KnockBack;
    //                ApplyEffect(weapon, attributeType, apply_value, true);
    //            }
    //            break;
    //        case ItemEffectType.KNOCKBACK_PERCENT:
    //            {
    //                attributeType = AttributeType.KnockBack;
    //                ApplyEffect(weapon, attributeType, apply_value, false);
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //private void ApplyEffect(WeaponBase weapon, AttributeType attributeType, float value, bool flat)
    //{
    //    if(flat)
    //    {
    //        weapon.Stat.Add(attributeType, value);
    //    }
    //    else
    //    {
    //        float currentStat = weapon.Stat.Get(attributeType);
    //        float originStat = weapon.Stat.GetOrigin(attributeType);
    //        weapon.Stat.Set(attributeType, currentStat + (originStat * (value/100)));
    //    }
    //}
}
