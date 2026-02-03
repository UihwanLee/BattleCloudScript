// 게임 프로젝트에서 사용할 Enum 타입에 접근할 수 있는 Enum Namespace
// Enum Type을 정의 할 때 region/endregion을 활용하여 사용한다.
namespace WhereAreYouLookinAt.Enum
{
    /// <summary>
    /// GameState
    /// </summary>
    #region GameState

    public enum GameState
    {
        TUTORIAL,
        RUNNING,
        PAUSE,
        UNDEF,
    }

    #endregion

    /// <summary>
    /// Attribute에 대한 Enum 타입 - Data 구조에 따라 달라질 수 있음.
    /// </summary>
    #region AttributeEnum
    public enum AttributeType
    {
        // Player Condition
        MaxHp,
        Hp,
        MaxExp,
        Exp,
        Gold,

        Lv,

        // Player Stat
        Command,
        MoveSpeed,
        Evasion,
        Defense,
        Luck,

        // Weapon Stat
        Damage,
        Range,
        Extent,
        KnockBack,
        AttackInterval,
        OnHitChancePercent,
        CriticalChancePercent,
        CriticalMultiplier,


        // Monster Stat
        KnockBackResist,
        SpecialValue,
        SpecialWeight,
    }
    #endregion

    /// <summary>
    /// Weapon에 대한 Enum 타입
    /// </summary>
    #region WeaponEnum

    public enum WeaponType
    {
        Base,               // 기본형
        FastAttack,         // 고속 공격형
        HighDamage,         // 고대미지형
        Piercing,           // 관통형
        Explosive,          // 폭발형

        RapidFire,          // 따발총형 
        Shotgun,            // 샷건형

        AreaDebuffer,       // 범위형 디버프
        AreaBuffer,         // 범위형 버퍼
        TargetDebuffer,     // 타겟형 디버프
        TargetHealer,       // 타겟형 힐러

        Summoner,           // 소환형
        UNDEF,
    }

    #endregion

    /// <summary>
    /// Item에 대한 Enum 타입
    /// </summary>
    #region ItemEunm

    public enum Tier
    {
        COMMAN,
        RARE,
        EPIC,
        LEGEND,
        EXCLUSIVE,
        Thrownitem,
        UNDEF,
    }

    public enum ItemType
    {
        AdvisorEnhancement,
        Attack,
        AttackSpeedCooldown,
        MovementPositioning,
        DefenseSurvival,
        ResourceEconomy,
        UniqueEffect,
        SummonCommand,
        UNDEF,
    }

    public enum ItemTraitType
    {
        Base,
        Exclusive,
        UNDEF,
    }

    #region DropItemEnum

    public enum DropItemType
    {
        Weapon,
        Item,
        ExclusiveItem,
    }

    #endregion

    #endregion

    /// <summary>
    /// Item Effect에 대한 Enum 타입
    /// </summary>
    #region EffectEnum

    public enum ItemEffectType
    {
        // 스탯 계열
        DAMAGE_FLAT,
        DAMAGE_PERCENT,
        TOTAL_DAMAGE_FLAT,
        TOTAL_DAMAGE_PERCENT,
        MOVE_SPEED_FLAT,
        MOVE_SPEED_PERCENT,
        ATTACK_SPEED_FLAT,
        ATTACK_SPEED_PERCENT,
        RANGE_FLAT,
        RANGE_PERCENT,
        ATTACK_INTERVAL_FLAT,
        ATTACK_INTERVAL_PERCENT,
        KNOCKBACK_FLAT,
        KNOCKBACK_PERCENT,
        CRITICAL_RATE_PERCENT,
        CRITICAL_DAMAGE_FLAT,

        // 온힛
        ON_HIT_HEAL,
        ON_HIT_STACK_AOE_PERCENT,
        ON_HIT_STACK_AOE_FLAT,
        ON_HIT_SELF_RISK_EXPLODE_PERCENT,
        ON_HIT_SELF_RISK_EXPLODE_FLAT,
        ON_HIT_PERIODIC_ATTACK,
        ON_HIT_EXTRA_PROJECTILE,
        ON_HIT_HEAL_RISK_DAMAGE_PERCENT,
        ON_HIT_HEAL_RISK_DAMAGE_FLAT,

        ON_HIT_DRAIN,

        // 온킬
        ON_KILL_GROW_DAMAGE,
        ON_KILL_GROW_DAMAGE_RISK,
        ON_KILL_GROW_DAMAGE_HP_RISK,
        ON_KILL_GROW_DAMAGE_MOVE_SPEED_RISK,
        ON_KILL_EXPLOSION,

        // 기타
        SALVAGE_BONUS,
        SALVAGE_STACK_BONUS,
        FATAL_IMMORTAL,
        MISSING_ADVICE_SCALING,
        DAILY_REWARD,
        STAT_RATIO_CRIT_TO_DAMAGE,
        STAT_RATIO_HP_TO_DAMAGE,

        EXTRA_PROJECTILE,

        // 전용
        Base,
        AreaDebuffer,
        Piercing,
        Explosive,
        Shotgun,
        RapidFire,
        TargetHealer,
        Summoner,

        // 패시브
        ON_PASSIVE_MULTI_SHOT,              // 다각 발사
        ON_PASSIVE_PIERCE_COUNT,            // 관통 증가
        ON_PASSIVE_MEGA_EXPLOSION,          // 핵폭발

        // 온힛
        ON_HIT_EXTRA_SHOT_CHANCE,           // 추가 발사
        ON_HIT_EXTRA_SHOT_CHANCE_FLAT,      // 추가 발사 고정 데미지
        ON_HIT_CHAIN_LIGHTNING,             // 번개 발사
        ON_HIT_CHAIN_LIGHTNING_FLAT,        // 번개 발사 고정 데미지
        ON_HIT_HOMING_ENABLE,               // 호밍 추가


        UNDEF,
    }

    public enum ItemTriggerType
    {
        ON_EQUIPPED,
        ON_HIT,
        ON_DAY_CHANGE,
        ON_FATAL_DAMAGE,
        ON_SALVAGE,
        ON_KILL,
        ON_PASSIVE,
        UNDEF,
    }

    public enum ItemApplyTarget
    {
        Player,
        GENERAL,        // 범용
        RAPID_FIRE,     // 연사형
        SHOOTING,       // 발사형
        AREA_FIELD,    // 장판형
        HEALING,        // 힐형
        UNDEF,
    }

    public enum ItemApplyType
    {
        FLAT,
        PERCENT,
        UNDEF,
    }

    #endregion

    /// <summary>
    /// Monster에 대한 Enum 타입
    /// </summary>
    #region MonsterEnum
    public enum MonsterType
    {
        BASE_01,
        BASE_02,
        BASE_03,
        BASE_04,
        BASE_05,
        UNDEF,
    }

    #endregion

    /// <summary>
    /// FloatingText에 대한 Enum 타입 - 굳이 타입이 필요없을 시 지워야함
    /// </summary>
    #region FloatingTextEnum

    public enum FloatingTextType
    {
        NormalDamage,
        CriticalDamage,
        Heal,
        Miss,
        Gold,
        Exp,
        UNDEF,
    }

    #endregion

    /// <summary>
    /// PopUpUI 대한 Enum 타입
    /// </summary>
    #region FloatingTextEnum

    public enum PopUp
    {
        SaveData,               // 세이브 데이터 저장
        LoadData,               // 세이브 데이터 로드
        DeleteSaveData,         // 세이브 데이터 삭제
        CoverSaveData,          // 세이브 데이터 덮어쓰기
        UNDEF,
    }

    #endregion

    /// <summary>
    /// Phase에 대한 Enum 타입
    /// </summary>
    #region PhaseEnum

    public enum PhaseType
    {
        Phase_01,
        Phase_02,
        Phase_03,
    }

    #endregion

    /// <summary>
    /// Drop 대한 Enum 타입
    /// </summary>
    #region PhaseEnum

    public enum DropType
    {
        Energy,
        Heal,
        Item
    }

    #endregion

    /// <summary>
    /// AudioEnum 타입
    /// </summary>
    #region AudioEnum

    public enum BGMType
    {
        Main
    }

    public enum SFXType
    {
        MonsterHit,
        PlayerHit,
        PlayerLevelUp,
        PlayerLootEat,
        PlayerLootGold,
        PlayerWalk,
        PlayerShot,
        PlayerLowHp,
        UIItemUpgrade,
        UIItemSell,
        UiButtonClick,
        UiButtonHover,
        UiPopUp,
        UiOption,
        impossible
    }

    public enum SFXPlayType
    {
        Single,
        Multi,
        UNDEF
    }
    #endregion

    /// <summary>
    /// Behavior Tree의 Node에 대한 상태
    /// </summary>
    #region NodeStateEnum
    public enum NodeState
    {
        SUCCESS,
        FAILURE,
        RUNNING,
    }
    #endregion

    /// <summary>
    /// InfoUI에 대한 Enum 타입
    /// </summary>
    #region InfoUIEnum
    public enum InfoUIType
    {
        Item,
        Interact,
        Circle,
    }
    #endregion

    /// <summary>
    /// Lanaguage에 대한 Enum 타입
    /// </summary>
    #region LanguageEnum
    public enum LanguageType
    {
        EN,
        KR,
        JP,
        SP,
    }
    #endregion

    /// <summary>
    /// 레벨업 시 Enhancement대한 Enum 타입
    /// </summary>
    #region EnhancementEnum
    public enum EnhancementTier
    {
        TIER1,
        TIER2,
        TIER3,
        TIER4,
        UNDEF,
    }

    public enum EnhancementEffect
    {
        ADD_ITEM,
        ADD_COMMAND,
        ADD_MAX_HP,
        ADD_MOVE_SPEED,
        ADD_DEFENSE,
        ADD_EVASION,
        ADD_LUCK,
        ADD_LEVEL_UP_ALL_ADVISOR,
        ADD_EXP_ALL_ADVISOR,
        ADD_REINFORCEMENT,
        UNDEF
    }

    #endregion

    /// <summary>
    /// Anomaly Result Enum 타입
    /// </summary>
    #region AnomalyResult

    public enum AnomalyResultType
    {
        Benefit,
        Penalty,
        UNDEF,
    }

    public enum AnomalyBenfitType
    {
        BENEFIT_NONE,
        BENEFIT_GAIN_REINFORCEMENT,
        BENEFIT_ADD_PHASE_01,
        BENEFIT_GAIN_ITEM,
        BENEFIT_GAIN_ITEM_EXCLUSIVE,
        BENEFIT_ADD_ADVISOR_DAMAGE,
        BENEFIT_SUB_ADVISOR_ATTACKINTERVAL,
        BENEFIT_ADD_ADVISOR_MOVESPEED,
        UNDEF,
    }

    public enum AnomalyPenaltyType
    {
        PENALTY_NONE,
        PENALTY_SUB_PHASE_01,
        PENALTY_SUB_EXP_RANGE,
        PENALTY_START_HP_1,
        PENALTY_MONSTER_DENSITY,
        PENALTY_MONSTER_DAMAGE,
        PENALTY_SUB_ADVISOR_DAMAGE,
        BENEFIT_ADD_ADVISOR_ATTACKINTERVAL,
        PENALTY_SUB_ADVISOR_MOVESPEED,
        PENALTY_HUD_HEALTH,
        UNDEF
    }

    #endregion

    /// <summary>
    /// Dialog 타입
    /// </summary>
    #region  DialogPositionType
    public enum DialogPositionType
    {
        LEFT,
        RIGHT,
        UNDEF,
    }
    #endregion

    /// <summary>
    /// QA 타입
    /// </summary>
    #region SlotToggleQAType
    public enum SlotToggleQAType
    {
        Toggle방식,
        지속방식,
    }
    #endregion

    /// <summary>
    /// QA 타입
    /// </summary>
    #region PlayerSlotQAType
    public enum PlayerSlotQAType
    {
        전체UI,
        인게임UI,
    }
    #endregion

    #region ProjectileTypeEnum
    public enum ProjectileType
    {
        None,
        // For Player
        PlayerBasic,
        PlayerPierce,
        PlayerExplosion,
        PlayerHeal,
        PlayerExtra,
        PlayerHighDamage,
        PlayerFastShot,
        PlayerShotgun,
        PlayerRapidShot,
        PlayerHoming,
        // For Monster
        MonsterHarpoon,
        MonsterBone,
    }

    public enum ImpactType
    {
        None,
        BasicImpact,
        Explosion,
        Dust,
        Fire,
        Thunder,
    }
    #endregion

    #region ManagementViewType
    public enum ManagementViewType
    {
        Detail,
        Enhancement
    }
    #endregion

    #region MouseCursorTypeEnum
    public enum CursorType
    {
        Default,
        Interact,
        Impossible
    }
    #endregion
    #region DifficultyEnum
    public enum DifficultyType
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
    }
    #endregion


}
