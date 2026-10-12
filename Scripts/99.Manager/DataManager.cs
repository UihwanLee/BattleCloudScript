using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// 게임 내 오브젝트들의 Data에 접근할 수 있는 전역 클래스
// Data는 Excel->Json으로 변환된 데이터 구조를 이용하여 저장하고 있다.
// Dictionary 자료구조를 통해 Data를 가지고 있으며 고유 ID로 Key를 분리한다.
// 따라서 데이터를 가져올 때 해당 ID를 통해 가져올 수 있으며 이 ID는 Define에 정의한다.
// Data를 적용하기 위하여 오브젝트는 Data를 담을 수 있는 컨테이너 클래스를 구현해야함(Ex. MonsterData)
// JSON 파일은 한 번만 파싱하고, 티어별 리스트 등은 파싱된 리스트를 나눠서 만든다.
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
            return;
        }

        LoadAllData();
    }

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

    // 타입별 조회 테이블 (Get<T>, GetDictCount<T>, GetList<T>에서 사용)
    private static readonly Dictionary<Type, object> dictByType = new Dictionary<Type, object>();
    private static readonly Dictionary<Type, object> listByType = new Dictionary<Type, object>();

    // 로드 중에만 사용하는 파싱 캐시: 같은 JSON을 여러 번 파싱하지 않도록 경로별로 보관
    private static readonly Dictionary<string, object> parsedJsonCache = new Dictionary<string, object>();

    // 리플렉션 FieldInfo 캐시
    private static readonly Dictionary<(Type, string), FieldInfo> fieldCache = new Dictionary<(Type, string), FieldInfo>();

    /// <summary>
    /// 모든 Data 로드
    /// </summary>
    private static void LoadAllData()
    {
        PlayerTraitDict = RegisterDict(LoadJsonToDict<PlayerTraitData>("Data/PlayerTrait"));
        PlayerStatDict = RegisterDict(LoadJsonToDict<PlayerStatData>("Data/PlayerStat"));
        WeaponTraitDict = RegisterDict(LoadJsonToDict<WeaponTraitData>("Data/WeaponTrait"));
        WeaponStatDict = RegisterDict(LoadJsonToDict<WeaponStatData>("Data/WeaponStat"));
        MonsterTraitDict = RegisterDict(LoadJsonToDict<MonsterTraitData>("Data/MonsterTrait"));
        MonsterStatDict = RegisterDict(LoadJsonToDict<MonsterStatData>("Data/MonsterStat"));
        ItemBaseItemDateDict = LoadJsonToDict<ItemData>("Data/ItemData");
        ExclusiveItemDataDict = LoadJsonToDict<ItemData>("Data/ExclusiveItemData");

        // 기본 아이템 + 전용 아이템 (같은 ID면 전용 아이템으로 덮어씀)
        ItemDataDict = new Dictionary<int, ItemData>(ItemBaseItemDateDict);
        foreach (var kv in ExclusiveItemDataDict)
        {
            ItemDataDict[kv.Key] = kv.Value;
        }
        RegisterDict(ItemDataDict);

        // 티어별 무기 리스트 나누기
        WeaponCommanDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "COMMAN");
        WeaponRareDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "RARE");
        WeaponEpicDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "EPIC");
        WeaponLegendDataList = LoadJsonToList<WeaponTraitData>("Data/WeaponTrait", "TIER", "LEGEND");

        WeaponTierDataDict = new Dictionary<Tier, List<WeaponTraitData>>
        {
            [Tier.COMMAN] = WeaponCommanDataList,
            [Tier.RARE] = WeaponRareDataList,
            [Tier.EPIC] = WeaponEpicDataList,
            [Tier.LEGEND] = WeaponLegendDataList,
        };

        // 티어별 아이템 리스트 나누기
        ItemCommanDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "COMMAN");
        ItemRareDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "RARE");
        ItemEpicDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "EPIC");
        ItemLegendDataList = LoadJsonToList<ItemData>("Data/ItemData", "TIER", "LEGEND");
        ExclusiveItemDataList = LoadJsonToList<ItemData>("Data/ExclusiveItemData", "TIER", "LEGEND");

        // 레전드 티어 = 레전드 아이템 + 전용 아이템
        // ItemLegendDataList에 직접 AddRange 하면 두 리스트가 같은 참조라 원본 리스트까지 바뀌므로 새 리스트로 합친다
        List<ItemData> legendWithExclusive = new List<ItemData>(ItemLegendDataList);
        legendWithExclusive.AddRange(ExclusiveItemDataList);

        ItemTierDataDict = new Dictionary<Tier, List<ItemData>>
        {
            [Tier.COMMAN] = ItemCommanDataList,
            [Tier.RARE] = ItemRareDataList,
            [Tier.EPIC] = ItemEpicDataList,
            [Tier.LEGEND] = legendWithExclusive,
        };

        InitLevelUpEnhancementData();

        InitGameRuleData();

        BaseDropItemSprite = GetImage("DropItem/G_Idle");
        BaseDropItem2Sprite = GetImage("DropItem/G_Idle_02");
        BaseDropItem3Sprite = GetImage("DropItem/G_Idle_03");

        Debug.Log($"[DataLoader] JSON {parsedJsonCache.Count}개 파싱, Dictionary Loaded Count : {PlayerTraitDict.Count + PlayerStatDict.Count + WeaponTraitDict.Count + WeaponStatDict.Count + MonsterTraitDict.Count + MonsterStatDict.Count + ItemDataDict.Count}");

        // 로드가 끝나면 파싱 캐시는 더 이상 필요 없다
        parsedJsonCache.Clear();
    }

    #region 게임 룰 데이터 초기화
    private static void InitGameRuleData()
    {
        // 페이즈 관련 룰 적용 (데이터의 PHASE 값과 enum 이름이 달라 직접 매핑)
        GameRulePhaseDict = new Dictionary<PhaseType, GameRulePhaseData>
        {
            [PhaseType.Phase_01] = LoadJsonToData<GameRulePhaseData>("Data/GameRulePhase", "PHASE", "Phase1"),
            [PhaseType.Phase_02] = LoadJsonToData<GameRulePhaseData>("Data/GameRulePhase", "PHASE", "Phase2"),
            [PhaseType.Phase_03] = LoadJsonToData<GameRulePhaseData>("Data/GameRulePhase", "PHASE", "Phase3"),
        };
        GameRulePhaseData = GameRulePhaseDict[PhaseType.Phase_03];

        // 드롭 확률 관련 룰 적용
        GameRuleDropDict = LoadJsonToEnumDict<DropType, GameRuleDropRate>("Data/GameRuleDropRate", "DROP");
        GameRuleDropData = GameRuleDropDict[DropType.Item];

        // 게임 룰 데이 적용
        GameRuleDayDataList = RegisterList(LoadJsonToList<GameRuleDayData>("Data/GameRuleDay", 2000));

        // 아이템 분해/강화 관련 룰 적용
        GameRuleItemLvList = RegisterList(LoadJsonToList<GameRuleItemLvData>("Data/GameRuleItemLv", 2500));

        // 무기 분해 관련 룰 적용
        GameRuleWeaponLvList = RegisterList(LoadJsonToList<GameRuleWeaponLvData>("Data/GameRuleWeaponLv", 2600));

        // Wave Data 룰 적용
        GameRuleWaveDataList = RegisterList(LoadJsonToList<GameRuleWaveData>("Data/GameRuleWave", 2700));

        // 무기 경험치 관련 룰 적용
        GameRuleExpWeaponDataDict = LoadJsonToEnumDict<WeaponType, GameRuleExpWeaponData>("Data/GameRuleExpWeapon", "KEY", WeaponType.UNDEF);
        GameRuleExpWeaponData = GameRuleExpWeaponDataDict[WeaponType.Summoner];
    }
    #endregion

    #region 레벨업 보상 데이터 초기화
    private static void InitLevelUpEnhancementData()
    {
        LevelUpRewardEnhancementTire1List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER1");
        LevelUpRewardEnhancementTire2List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER2");
        LevelUpRewardEnhancementTire3List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER3");
        LevelUpRewardEnhancementTire4List = LoadJsonToList<LevelUpRewardEnhancementData>("Data/EnhancementPlayerCardData", "TIER", "TIER4");

        LevelUpRewardEnhancementDict = new Dictionary<EnhancementTier, List<LevelUpRewardEnhancementData>>
        {
            [EnhancementTier.TIER1] = LevelUpRewardEnhancementTire1List,
            [EnhancementTier.TIER2] = LevelUpRewardEnhancementTire2List,
            [EnhancementTier.TIER3] = LevelUpRewardEnhancementTire3List,
            [EnhancementTier.TIER4] = LevelUpRewardEnhancementTire4List,
        };
    }
    #endregion

    #region JSON 로드

    /// <summary>
    /// JSON 파일을 List로 파싱. 같은 경로는 한 번만 파싱한다.
    /// </summary>
    private static List<T> ParseJson<T>(string path)
    {
        if (parsedJsonCache.TryGetValue(path, out object cached))
        {
            if (cached is List<T> cachedList) return cachedList;

            Debug.LogError($"[DataLoader] {path}를 서로 다른 타입({cached.GetType()}, {typeof(List<T>)})으로 읽으려 합니다.");
            return new List<T>();
        }

        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        List<T> list = null;

        if (jsonFile == null)
        {
            Debug.LogWarning($"JSON Not Found : {path}");
        }
        else
        {
            try
            {
                list = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);
            }
            catch (JsonException e)
            {
                Debug.LogError($"[DataLoader] {path} 파싱 실패: {e.Message}");
            }
        }

        list ??= new List<T>();
        parsedJsonCache[path] = list;
        return list;
    }

    /// <summary>
    /// ID를 Key로 하는 Dictionary 생성. ID가 중복되면 첫 번째 데이터를 유지하고 경고한다.
    /// </summary>
    private static Dictionary<int, T> LoadJsonToDict<T>(string path)
    {
        var dict = new Dictionary<int, T>();

        foreach (var item in ParseJson<T>(path))
        {
            int id = GetID(item);
            if (!dict.TryAdd(id, item))
                Debug.LogWarning($"[DataLoader] {path} ID {id}가 중복되어 첫 번째 데이터만 사용합니다.");
        }
        return dict;
    }

    /// <summary>
    /// field 값이 value와 같은 데이터만 모은 List
    /// </summary>
    private static List<T> LoadJsonToList<T>(string path, string field, string value)
    {
        var newlist = new List<T>();

        foreach (var item in ParseJson<T>(path))
        {
            if (GetField(item, field) == value)
                newlist.Add(item);
        }

        if (newlist.Count == 0)
            Debug.LogWarning($"[DataLoader] {path}에 {field} = {value}인 데이터가 없습니다.");

        return newlist;
    }

    /// <summary>
    /// field 값이 value인 첫 번째 데이터. 여러 개면 경고한다.
    /// </summary>
    private static T LoadJsonToData<T>(string path, string field, string value)
    {
        T found = default;
        int matchCount = 0;

        foreach (var item in ParseJson<T>(path))
        {
            if (GetField(item, field) != value) continue;

            if (matchCount == 0) found = item;
            matchCount++;
        }

        if (matchCount == 0)
            Debug.LogWarning($"[DataLoader] {path}에 {field} = {value}인 데이터가 없습니다.");
        else if (matchCount > 1)
            Debug.LogWarning($"[DataLoader] {path}에 {field} = {value}인 데이터가 {matchCount}개 있어 첫 번째만 사용합니다.");

        return found;
    }

    /// <summary>
    /// enum 값마다 field 값이 enum 이름과 같은 데이터를 매핑 (exclude 값은 제외)
    /// </summary>
    private static Dictionary<TEnum, TData> LoadJsonToEnumDict<TEnum, TData>(string path, string field, TEnum? exclude = null)
        where TEnum : struct, Enum
    {
        var dict = new Dictionary<TEnum, TData>();

        foreach (TEnum key in Enum.GetValues(typeof(TEnum)))
        {
            if (exclude.HasValue && EqualityComparer<TEnum>.Default.Equals(key, exclude.Value)) continue;

            dict[key] = LoadJsonToData<TData>(path, field, key.ToString());
        }
        return dict;
    }

    /// <summary>
    /// ID 필드 값으로 데이터를 모은 List (GameRule 시트는 ID를 그룹 키로 사용)
    /// </summary>
    private static List<T> LoadJsonToList<T>(string path, int id)
    {
        var newlist = new List<T>();

        foreach (var item in ParseJson<T>(path))
        {
            if (GetID(item) == id)
                newlist.Add(item);
        }

        return newlist;
    }

    private static Dictionary<int, T> RegisterDict<T>(Dictionary<int, T> dict)
    {
        dictByType[typeof(T)] = dict;
        return dict;
    }

    private static List<T> RegisterList<T>(List<T> list)
    {
        listByType[typeof(T)] = list;
        return list;
    }

    /// <summary>
    /// ID 필드 추출
    /// </summary>
    private static int GetID<T>(T obj)
    {
        FieldInfo idField = GetFieldInfo(typeof(T), "ID");
        return idField != null ? (int)idField.GetValue(obj) : 0;
    }

    private static string GetField<T>(T obj, string field)
    {
        FieldInfo fieldInfo = GetFieldInfo(typeof(T), field);
        return fieldInfo?.GetValue(obj) as string;
    }

    private static FieldInfo GetFieldInfo(Type type, string field)
    {
        if (fieldCache.TryGetValue((type, field), out FieldInfo cached))
            return cached;

        FieldInfo fieldInfo = type.GetField(field);
        if (fieldInfo == null)
            Debug.LogError($"[DataLoader] {type.Name}에 {field} 필드가 없습니다.");

        fieldCache[(type, field)] = fieldInfo;
        return fieldInfo;
    }

    #endregion

    #region 데이터 조회

    /// <summary>
    /// ID 필드를 이용하여 데이터 갖기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public static T Get<T>(int id)
    {
        if (!dictByType.TryGetValue(typeof(T), out object dict))
        {
            Debug.LogError($"[DataLoader] Unknown Type : {typeof(T)}");
            return default;
        }

        if (!((Dictionary<int, T>)dict).TryGetValue(id, out T data))
        {
            Debug.LogError($"[DataLoader] {typeof(T).Name}에 ID {id} 데이터가 없습니다.");
            return default;
        }

        return data;
    }

    public static int GetDictCount<T>()
    {
        if (dictByType.TryGetValue(typeof(T), out object dict))
            return ((Dictionary<int, T>)dict).Count;

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
        if (listByType.TryGetValue(typeof(T), out object list))
            return (List<T>)list;

        Debug.LogWarning($"[DataLoader] 등록되지 않은 List 타입 : {typeof(T)}");
        return default;
    }

    #endregion

    /// <summary>
    /// 이미지 이름으로 Resouces 폴더에 Sprite 반환하는 함수
    /// </summary>
    /// <param name="imgName">이미지 이름</param>
    /// <returns></returns>
    public static Sprite GetImage(string imgName)
    {
        Sprite sprite = Resources.Load<Sprite>($"Sprites/{imgName}");

        if (sprite == null) Debug.LogWarning($"해당 {imgName} 이미지를 로드할 수 없습니다.");
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

        if (prefab == null) Debug.LogWarning($"해당 {prefabName} 프리팹을 로드할 수 없습니다.");
        return prefab;
    }

    #region 데이터 타입 파싱

    public static T GetEnumType<T>(string value) where T : struct, Enum
    {
        return ParseEnum(value, default(T));
    }

    public static Tier GetTier(string value) => ParseEnum(value, Tier.UNDEF);

    public static WeaponType GetWeaponType(string value) => ParseEnum(value, WeaponType.UNDEF);

    public static ItemType GetItemType(string value) => ParseEnum(value, ItemType.UNDEF);

    public static ItemApplyTarget GetItemApplyTarget(string value) => ParseEnum(value, ItemApplyTarget.UNDEF);

    public static WeaponReinforcementType GetWeaponReinforcementType(string value) => ParseEnum(value, WeaponReinforcementType.UNDEF);

    /// <summary>
    /// 문자열을 enum으로 변환 (대소문자 무시). 실패하면 fallback 반환
    /// </summary>
    private static T ParseEnum<T>(string value, T fallback) where T : struct, Enum
    {
        if (Enum.TryParse(value, true, out T result))
            return result;

        Debug.LogError($"{value}에 대한 {typeof(T).Name}이 지정되지 않았습니다.");
        return fallback;
    }

    #endregion
}
