using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using WhereAreYouLookinAt.Enum;

// 게임 내 오브젝트들의 Data에 접근할 수 있는 전역 클래스
// Data는 Excel->Json으로 변환된 데이터 구조를 이용하여 저장하고 있다.
// Dictionary 자료구조를 통해 Data를 가지고 있으며 고유 ID로 Key를 분리한다.
// 따라서 데이터를 가져올 때 해당 ID를 통해 가져올 수 있우며 이 ID는 Define에 정의한다.
// Data를 적용하기 위하여 오브젝트는 Data를 담을 수 있는 컨테이너 클래스를 구현해야함(Ex. MonsterData)
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadAllData();
    }

    private DataManager() { }

    #region 플레이어, 무기, 아이템, 몬스터 데이터
    public static Dictionary<int, PlayerTraitData> PlayerTraitDict { get; private set; }
    public static Dictionary<int, PlayerStatData> PlayerStatDict { get; private set; }
    public static Dictionary<int, WeaponTraitData> WeaponTraitDict { get; private set; }
    public static Dictionary<int, WeaponStatData> WeaponStatDict { get; private set; }
    public static Dictionary<int, MonsterTraitData> MonsterTraitDict {  get; private set; }
    public static Dictionary<int, MonsterStatData> MonsterStatDict { get; private set; }

    public static Dictionary<Tier, List<WeaponTraitData>> WeaponTierDataDict { get; private set; }
    public static List<WeaponTraitData> WeaponCommanDataList { get; private set; }
    public static List<WeaponTraitData> WeaponRareDataList { get; private set; }
    public static List<WeaponTraitData> WeaponEpicDataList { get; private set; }
    public static List<WeaponTraitData> WeaponLegendDataList { get; private set; }

    public static Dictionary<int, ItemData> ItemDataDict { get; private set; }
    public static Dictionary<int, ItemData> ExclusiveItemDataDict { get; private set; }
    public static Dictionary<int, ItemData> ItemBaseItemDateDict { get; private set; }

    public static Dictionary<Tier, List<ItemData>> ItemTierDataDict { get; private set; }
    public static List<ItemData> ItemCommanDataList { get; private set; }
    public static List<ItemData> ItemRareDataList { get; private set; }
    public static List<ItemData> ItemEpicDataList { get; private set; }
    public static List<ItemData> ItemLegendDataList { get; private set; }
    public static List<ItemData> ExclusiveItemDataList { get; private set; }

    #endregion

    #region 레벨업 보상 데이터 변수
    // RewardData
    public static Dictionary<EnhancementTier, List<LevelUpRewardEnhancementData>> LevelUpRewardEnhancementDict { get; private set; }
    public static List<LevelUpRewardEnhancementData> LevelUpRewardEnhancementTire1List { get; private set; }
    public static List<LevelUpRewardEnhancementData> LevelUpRewardEnhancementTire2List { get; private set; }
    public static List<LevelUpRewardEnhancementData> LevelUpRewardEnhancementTire3List { get; private set; }
    public static List<LevelUpRewardEnhancementData> LevelUpRewardEnhancementTire4List { get; private set; }
    #endregion

    #region 게임 룰 데이터
    // GameRuleData
    public static Dictionary<PhaseType, GameRulePhaseData> GameRulePhaseDict { get; private set; }
    public static GameRulePhaseData GameRulePhaseData { get; private set; }
    public static List<GameRuleDayData> GameRuleDayDataList { get; private set; }
    public static Dictionary<WeaponType, GameRuleExpWeaponData> GameRuleExpWeaponDataDict { get; private set; }
    public static GameRuleExpWeaponData GameRuleExpWeaponData { get; private set; }

    public static Dictionary<DropType, GameRuleDropRate> GameRuleDropDict { get; private set; }
    public static GameRuleDropRate GameRuleDropData { get; private set; }

    public static List<GameRuleItemLvData> GameRuleItemLvList { get; private set; }
    public static List<GameRuleWeaponLvData> GameRuleWeaponLvList { get; private set; }
    public static List<GameRuleWaveData> GameRuleWaveDataList { get; private set; }

    #endregion

    // 일반 변수
    public static Sprite BaseDropItemSprite { get; set; }
    public static Sprite BaseDropItem2Sprite { get; set; }
    public static Sprite BaseDropItem3Sprite { get; set; }

    /// <summary>
    /// 모든 Data 로드
    /// </summary>
    private static void LoadAllData()
    {
        PlayerTraitDict = LoadJsonToDict<PlayerTraitData>("Data/PlayerTrait");
        PlayerStatDict = LoadJsonToDict<PlayerStatData>("Data/PlayerStat");
        WeaponTraitDict = LoadJsonToDict<WeaponTraitData>("Data/WeaponTrait");
        WeaponStatDict = LoadJsonToDict<WeaponStatData>("Data/WeaponStat");
        MonsterTraitDict = LoadJsonToDict<MonsterTraitData>("Data/MonsterTrait");
        MonsterStatDict = LoadJsonToDict<MonsterStatData>("Data/MonsterStat");
        ItemDataDict = LoadJsonToDict<ItemData>("Data/ItemData");
        ItemBaseItemDateDict = LoadJsonToDict<ItemData>("Data/ItemData");
        ExclusiveItemDataDict = LoadJsonToDict<ItemData>("Data/ExclusiveItemData");

        // ItemDataDict에 덮어씀
        foreach (var kv in ExclusiveItemDataDict)
        {
            ItemDataDict[kv.Key] = kv.Value;
        }

        // 티어별 무기 리스트 나누기
        WeaponCommanDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "COMMAN");
        WeaponRareDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "RARE");
        WeaponEpicDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "EPIC");
        WeaponLegendDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "LEGEND");

        WeaponTierDataDict = new Dictionary<Tier, List<WeaponTraitData>>();
        WeaponTierDataDict[Tier.COMMAN] = WeaponCommanDataList;
        WeaponTierDataDict[Tier.RARE] = WeaponRareDataList;
        WeaponTierDataDict[Tier.EPIC] = WeaponEpicDataList;
        WeaponTierDataDict[Tier.LEGEND] = WeaponLegendDataList;

        // 티어별 아이템 리스트 나누기
        ItemCommanDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "COMMAN");
        ItemRareDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "RARE");
        ItemEpicDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "EPIC");
        ItemLegendDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "LEGEND");
        ExclusiveItemDataList = LoadJsonToList<ItemData>("Data/ExclusiveItemData", "TIER", "LEGEND");

        ItemTierDataDict = new Dictionary<Tier, List<ItemData>>();
        ItemTierDataDict[Tier.COMMAN] = ItemCommanDataList;
        ItemTierDataDict[Tier.RARE] = ItemRareDataList;
        ItemTierDataDict[Tier.EPIC] = ItemEpicDataList;
        ItemTierDataDict[Tier.LEGEND] = ItemLegendDataList;
        ItemTierDataDict[Tier.LEGEND].AddRange(ExclusiveItemDataList);

        InitLevelUpEnhancementData();

        InitGameRuleData();

        BaseDropItemSprite = GetImage("DropItem/G_Idle");
        BaseDropItem2Sprite = GetImage("DropItem/G_Idle_02");
        BaseDropItem3Sprite = GetImage("DropItem/G_Idle_03");

        Debug.Log($"[DataLoader] Dictionary Loaded Count : {PlayerTraitDict.Count + PlayerStatDict.Count + WeaponTraitDict.Count + WeaponStatDict.Count + MonsterTraitDict.Count + MonsterStatDict.Count + ItemDataDict.Count}");
    }

    #region 게임 룰 데이터 초기화
    private static void InitGameRuleData()
    {
        // 페이즈 관련 룰 적용
        GameRulePhaseDict = new Dictionary<PhaseType, GameRulePhaseData>();
        GameRulePhaseData = LoadJsonToData<GameRulePhaseData>("Data/GameRulePhase", "PHASE", "Phase1");
        GameRulePhaseDict[PhaseType.Phase_01] = GameRulePhaseData;
        GameRulePhaseData = LoadJsonToData<GameRulePhaseData>("Data/GameRulePhase", "PHASE", "Phase2");
        GameRulePhaseDict[PhaseType.Phase_02] = GameRulePhaseData;
        GameRulePhaseData = LoadJsonToData<GameRulePhaseData>("Data/GameRulePhase", "PHASE", "Phase3");
        GameRulePhaseDict[PhaseType.Phase_03] = GameRulePhaseData;

        // 드롭 확룔 관련 룰 적용
        GameRuleDropDict = new Dictionary<DropType, GameRuleDropRate>();
        GameRuleDropData = LoadJsonToData<GameRuleDropRate>("Data/GameRuleDropRate", "DROP", "Energy");
        GameRuleDropDict[DropType.Energy] = GameRuleDropData;
        GameRuleDropData = LoadJsonToData<GameRuleDropRate>("Data/GameRuleDropRate", "DROP", "Heal");
        GameRuleDropDict[DropType.Heal] = GameRuleDropData;
        GameRuleDropData = LoadJsonToData<GameRuleDropRate>("Data/GameRuleDropRate", "DROP", "Item");
        GameRuleDropDict[DropType.Item] = GameRuleDropData;

        // 게임 룰 데이 적용
        GameRuleDayDataList = LoadJsonToList<GameRuleDayData>("Data/GameRuleDay", 2000);

        // 아이템 분해/강화 관련 룰 적용
        GameRuleItemLvList = LoadJsonToList<GameRuleItemLvData>("Data/GameRuleItemLv", 2500);

        // 무기 분해 관련 룰 적용
        GameRuleWeaponLvList = LoadJsonToList<GameRuleWeaponLvData>("Data/GameRuleWeaponLv", 2600);

        // Wave Data 룰 적용
        GameRuleWaveDataList = LoadJsonToList<GameRuleWaveData>("Data/GameRuleWave", 2700);

        // 무기 경험치 관련 룰 적용
        GameRuleExpWeaponDataDict = new Dictionary<WeaponType, GameRuleExpWeaponData>();
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "Base");
        GameRuleExpWeaponDataDict[WeaponType.Base] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "FastAttack");
        GameRuleExpWeaponDataDict[WeaponType.FastAttack] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "HighDamage");
        GameRuleExpWeaponDataDict[WeaponType.HighDamage] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "Piercing");
        GameRuleExpWeaponDataDict[WeaponType.Piercing] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "Explosive");
        GameRuleExpWeaponDataDict[WeaponType.Explosive] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "RapidFire");
        GameRuleExpWeaponDataDict[WeaponType.RapidFire] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "Shotgun");
        GameRuleExpWeaponDataDict[WeaponType.Shotgun] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "AreaDebuffer");
        GameRuleExpWeaponDataDict[WeaponType.AreaDebuffer] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "AreaBuffer");
        GameRuleExpWeaponDataDict[WeaponType.AreaBuffer] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "TargetDebuffer");
        GameRuleExpWeaponDataDict[WeaponType.TargetDebuffer] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "TargetHealer");
        GameRuleExpWeaponDataDict[WeaponType.TargetHealer] = GameRuleExpWeaponData;
        GameRuleExpWeaponData = LoadJsonToData<GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", "Summoner");
        GameRuleExpWeaponDataDict[WeaponType.Summoner] = GameRuleExpWeaponData;
    }
    #endregion

    #region 레벨업 보상 데이터 초기화
    private static void InitLevelUpEnhancementData()
    {
        LevelUpRewardEnhancementTire1List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER1");
        LevelUpRewardEnhancementTire2List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER2");
        LevelUpRewardEnhancementTire3List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER3");
        LevelUpRewardEnhancementTire4List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER4");

        LevelUpRewardEnhancementDict = new Dictionary<EnhancementTier, List<LevelUpRewardEnhancementData>>();
        LevelUpRewardEnhancementDict[EnhancementTier.TIER1] = LevelUpRewardEnhancementTire1List;
        LevelUpRewardEnhancementDict[EnhancementTier.TIER2] = LevelUpRewardEnhancementTire2List;
        LevelUpRewardEnhancementDict[EnhancementTier.TIER3] = LevelUpRewardEnhancementTire3List;
        LevelUpRewardEnhancementDict[EnhancementTier.TIER4] = LevelUpRewardEnhancementTire4List;
    }
    #endregion

    /// <summary>
    /// 제네릭 T 타입에 따른 Dicitonary 추가
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    private static Dictionary<int, T> LoadJsonToDict<T>(string path)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogWarning($"JSON Not Found : {path}");
            return new Dictionary<int, T>();
        }

        var list = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);
        var dict = new Dictionary<int, T>();

        foreach (var item in list)
        {
            int id = GetID(item);
            dict[id] = item;
        }
        return dict;
    }

    private static List<T> LoadJsonToList<T>(string path, string field, string TIER)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogWarning($"JSON Not Found : {path}");
            return new List<T>();
        }

        var list = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);
        var newlist = new List<T>();

        foreach(var item in list)
        {
            if(GetField(item, field) == TIER)
            {
                newlist.Add(item);
            }
        }

        return newlist;
    }

    private static List<T> LoadJsonToList<T, TEnum>(string path, string field, TEnum tierEnum)
    where TEnum : struct, Enum
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogWarning($"JSON Not Found : {path}");
            return new List<T>();
        }

        var list = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);
        var newlist = new List<T>();

        // 비교할 기준 문자열 (Enum을 문자열로 변환)
        string tierString = tierEnum.ToString();

        foreach (var item in list)
        {
            // GetField로 가져온 데이터가 해당 Enum 문자열과 일치하는지 확인
            object fieldValue = GetField(item, field);

            if (fieldValue != null && fieldValue.ToString().Equals(tierString, StringComparison.OrdinalIgnoreCase))
            {
                newlist.Add(item);
            }
        }

        return newlist;
    }

    private static T LoadJsonToData<T>(string path, string field, string TIER)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogWarning($"JSON Not Found : {path}");
            return default(T);
        }

        var list = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);
        var newlist = new List<T>();

        foreach (var item in list)
        {
            if (GetField(item, field) == TIER)
            {
                return (T)item;
            }
        }

        return default(T);
    }

    private static List<T> LoadJsonToList<T>(string path, int id)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogWarning($"JSON Not Found : {path}");
            return new List<T>();
        }

        var list = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);
        var newlist = new List<T>();

        foreach (var item in list)
        {
            if (GetID(item) == id)
            {
                newlist.Add(item);
            }
        }

        return newlist;
    }

    /// <summary>
    /// ID 필드 추출
    /// </summary>
    private static int GetID<T>(T obj)
    {
        var idField = typeof(T).GetField("ID");
        return (int)idField.GetValue(obj);
    }

    private static string GetField<T>(T obj, string field)
    {
        var tierField = typeof(T).GetField(field);
        return (string)tierField.GetValue(obj);
    }

    /// <summary>
    /// ID 필드를 이용하여 데이터 갖기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public static T Get<T>(int id)
    {
        if (typeof(T) == typeof(PlayerTraitData))
            return (T)(object)PlayerTraitDict[id];

        if (typeof(T) == typeof(PlayerStatData))
            return (T)(object)PlayerStatDict[id];

        if (typeof(T) == typeof(WeaponTraitData))
            return (T)(object)WeaponTraitDict[id];

        if (typeof(T) == typeof(WeaponStatData))
            return (T)(object)WeaponStatDict[id];

        if (typeof(T) == typeof(MonsterTraitData))
            return (T)(object)MonsterTraitDict[id];

        if (typeof(T) == typeof(MonsterStatData))
            return (T)(object)MonsterStatDict[id];

        if (typeof(T) == typeof(ItemData))
            return (T)(object)ItemDataDict[id];

        Debug.LogError($"[DataLoader] Unknown Type : {typeof(T)}");
        return default;
    }

    public static int GetDictCount<T>()
    {
        if(typeof(T) == typeof(PlayerTraitData))
            return (int)PlayerTraitDict.Count;

        if(typeof(T) == typeof(WeaponTraitData))
            return (int)WeaponTraitDict.Count;

        if(typeof (T) == typeof(MonsterTraitData))
            return (int)MonsterTraitDict.Count;

        if (typeof(T) == typeof(ItemData))
            return ItemDataDict.Count;

        Debug.LogError($"[DataLoader] Unknown Type : {typeof(T)}");
        return default;
    }

    public static Dictionary<EnhancementTier, List<T>> GetDict<T>()
    {
        if (typeof(T) == typeof(LevelUpRewardEnhancementData))
            return LevelUpRewardEnhancementDict as Dictionary<EnhancementTier, List<T>>;

        return default;
    }

    public static Dictionary<PhaseType, GameRulePhaseData> GetGameRulePhaseDict()
    {
        return GameRulePhaseDict;
    }

    public static Dictionary<DropType, GameRuleDropRate> GetGameRuleDropDict()
    {
        return GameRuleDropDict;
    }

    public static Dictionary<WeaponType, GameRuleExpWeaponData> GetGameRuleExpWeaponDict()
    {
        return GameRuleExpWeaponDataDict;
    }


    public static List<T> GetList<T>()
    { 
        if (typeof(T) == typeof(GameRuleDayData))
            return GameRuleDayDataList as List<T>;

        if (typeof(T) == typeof(GameRuleItemLvData))
            return GameRuleItemLvList as List<T>;

        if (typeof(T) == typeof(GameRuleWeaponLvData))
            return GameRuleWeaponLvList as List<T>;

        return default;
    }

    /// <summary>
    /// 이미지 이름으로 Resouces 폴더에 Sprite 반환하는 함수
    /// </summary>
    /// <param name="imgName">이미지 이름</param>
    /// <returns></returns>
    public static Sprite GetImage(string imgName)
    {
        Sprite sprite = Resources.Load<Sprite>($"Sprites/{imgName}");

        if (sprite == null) Debug.Log($"해당 {imgName} 이미지를 로드할 수 없습니다.");
        return sprite;
    }

    /// <summary>
    /// 프리팹 이름으로 Resouces 폴더에 GameObject 반환하는 함수
    /// </summary>
    /// <param name="prefabName">프리팹 이름</param>
    /// <returns></returns>
    public static GameObject GetPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefab/{prefabName}");

        if (prefab == null) Debug.Log($"해당 {prefabName} 프리팹을 로드할 수 없습니다.");
        return prefab;
    }

    public static Tier GetTier(string value)
    {
        if (Enum.TryParse<Tier>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 Tier 지정되지 않았습니다");
        return Tier.UNDEF;
    }

    #region 데이터 타입 파싱

    public static T GetEnumType<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }

        Debug.LogError($"{value}에 대한 {typeof(T).Name}이 지정되지 않았습니다.");

        return default;
    }

    public static WeaponType GetWeaponType(string value)
    {
        if (Enum.TryParse<WeaponType>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 WeaponType이 지정되지 않았습니다");
        return WeaponType.UNDEF;
    }

    public static ItemType GetItemType(string value)
    {
        if (Enum.TryParse<ItemType>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 ItemType이 지정되지 않았습니다");
        return ItemType.UNDEF;
    }

    public static ItemApplyTarget GetItemApplyTarget(string value)
    {
        if (Enum.TryParse<ItemApplyTarget>(value, true, out var result))
            return result;

        Debug.LogError($"{value}에 대한 ItemApplyTarget이 지정되지 않았습니다");
        return ItemApplyTarget.UNDEF;
    }

    #endregion
}
