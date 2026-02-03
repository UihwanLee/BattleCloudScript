// 게임 프로젝트에서 사용할 상수값에 대해 접근할 수 있는 전역 클래스
// 상수를 정의 할 때 region/endregion을 활용하여 사용한다.
// const -> 상수이므로 그냥 사용해도됨
// static readonly -> Inspector 창에서 바꿀 수 없는 상수값 / 직렬화 불가
// LayerMask도 여기서 정의
using UnityEngine;

public static class Define 
{
    //public static readonly float TEST1;
    //public const float TEST2;

    #region 파일 Define

    public const string FILE_PAHT_LANAUAGE_JSON_DATA = "Data/Localization/";

    #endregion

    #region PlayerID

    public const int PLATER_BASE_ID = 0;

    #endregion

    #region PlayerData

    public const float PLAYER_INTERACT_COOLTIME = 0.5f;

    #endregion

    #region InteractPriority

    // 상호작용 오브젝트 우선순위
    public const int INTERACT_PRIORITY_01 = 1;
    public const int INTERACT_PRIORITY_02 = 2;

    #endregion

    #region LanaguageID

    // ItemInfoUI
    public const string LANAUAGE_INTERACT_DROPITEM_SUCCESS = "UI_Interact_F";
    public const string LANAUAGE_INTERACT_DROPITEM_FAIL = "UI_Interact_DropItem_Fail";

    // Table Name
    public const string TABLE_WEAPON_NAME = "WeaponTable";
    public const string TABLE_ITEM_NAME = "ItemTable";
    public const string TABLE_ITEM_EXCLUSIVE_NAME = "ExclusiveItemTable";

    // Max ItemInfoUIValue
    public const int LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT = 5;

    #endregion

    #region Sounds

    public const int AVAILABLE_SOUNDSORUCE_COUNT = 10;

    #endregion

    #region Magic Number

    public const int NO_FIND_NUMBER = -1;

    #endregion

    #region WeaponID

    public const int WEAPON_BASE_ID = 200;

    #endregion

    #region ItemID

    public const int ITME_BASE_ID = 600;
    public const int ITEM_EXCLUSIVE_ID = 700;

    #endregion

    #region MonsterID

    public const int MONSTER_BASE_ID = 400;

    #endregion

    #region 무기(조언자) 최대 레벨
    public const int MAX_WEAPONLEVEL = 20;
    #endregion

    #region 무기(조언자) 최대 개수
    public const int MAX_WEAPONCOUNT = 6;
    #endregion

    #region Lv 설정

    public const int MAX_WEAPON_LV = 5;
    public const int MAX_ITEM_LV = 5;
    public const string MAX_LV_LABEL = "Max";

    #endregion

    #region 티어 색상

    public static readonly Color TIER_COLOR_COMMON = new Color(0.53f, 0.53f, 0.53f, 1f); // #878787
    public static readonly Color TIER_COLOR_RARE = new Color(0.26f, 0.45f, 0.24f, 1f); // #43723E
    public static readonly Color TIER_COLOR_EPIC = new Color(0.44f, 0.24f, 0.35f, 1f); // #713D59
    public static readonly Color TIER_COLOR_LEGEND = new Color(0.70f, 0.54f, 0.16f, 1f); // #B38928
    public static readonly Color TIER_COLOR_EXCLUSIVE = new Color(0.46f, 0.20f, 0.20f, 1f); // #743232

    #endregion

    #region 타격음 동시 소리 최대 개수

    public const int MAX_SFX_COUNT = 10;

    #endregion
}
