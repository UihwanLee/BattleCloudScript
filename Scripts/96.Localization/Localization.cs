using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

// 언어 감독 스크립트를 담당
public class Localization : MonoBehaviour
{

    #region 구 Localiazation 방법
    //private Dictionary<LanguageType, Dictionary<string, string>> allLanageTable = new Dictionary<LanguageType, Dictionary<string, string>>();
    //private Dictionary<string, string> currentLanageTable;

    //private Dictionary<string, string> LanageTable_EN = new Dictionary<string, string>();
    //private Dictionary<string, string> LanageTable_KR = new Dictionary<string, string>();
    //private Dictionary<string, string> LanageTable_JP = new Dictionary<string, string>();
    //private Dictionary<string, string> LanageTable_SP = new Dictionary<string, string>();

    //private static Localization instance;

    //private LanguageType currentLanguageType;

    //public static Localization Instance
    //{
    //    get
    //    {
    //        if (null == instance)
    //        {
    //            instance = FindObjectOfType<Localization>();
    //        }
    //        return instance;
    //    }
    //}

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }

    //    LoadAllLangeByType();
    //}

    //private void LoadAllLangeByType()
    //{
    //    LoadLangeByType("Data/Localization/Localization_Item");
    //    LoadLangeByType("Data/Localization/Localization_Player");
    //    LoadLangeByType("Data/Localization/Localization_Weapon");
    //    LoadLangeByType("Data/Localization/Localization_UI");
    //    LoadLangeByType("Data/Localization/Localization_Reward");

    //    // LanguageType별 테이블 매핑
    //    allLanageTable.Add(LanguageType.EN, LanageTable_EN);
    //    allLanageTable.Add(LanguageType.KR, LanageTable_KR);
    //    allLanageTable.Add(LanguageType.JP, LanageTable_JP);
    //    allLanageTable.Add(LanguageType.SP, LanageTable_SP);

    //    // 기본 EN으로 설정 -> 로드된 데이터가 없을 때
    //    currentLanageTable = LanageTable_EN;

    //    Debug.Log("[Localization] 언어 데이터 로드 성공");
    //}

    //private void LoadLangeByType(string path)
    //{
    //    // Json 파일을 읽어들어 EN, KR, JP, SP 대로 저장하고 모든 Dictionary 초기화
    //    TextAsset jsonFile = Resources.Load<TextAsset>(path);

    //    if (jsonFile == null)
    //    {
    //        Debug.LogWarning($"[Localization] JSON Not Found : {path}");
    //    }

    //    var list = JsonConvert.DeserializeObject<List<LocalizationData>>(jsonFile.text);
    //    if (list == null || list.Count == 0)
    //    {
    //        Debug.LogWarning($"[Localization] Localization JSON Empty : {path}");
    //        return;
    //    }

    //    // 언어별 테이블 매핑
    //    foreach (var data in list)
    //    {
    //        if (string.IsNullOrEmpty(data.KEY))
    //            continue;

    //        LanageTable_EN[data.KEY] = data.EN;
    //        LanageTable_KR[data.KEY] = data.KR;
    //        LanageTable_JP[data.KEY] = data.JP;
    //        LanageTable_SP[data.KEY] = data.SP;
    //    }
    //}

    //public string Get(string key)
    //{
    //    if (CurrentLanageTable == null)
    //        return key;

    //    if (CurrentLanageTable.TryGetValue(key, out var value))
    //        return value;

    //    Debug.LogWarning($"[Localization] 언어 가져오기 실패 : {key}");
    //    return key;
    //}

    //// 언어 변경
    //public void ChangeLanague(LanguageType type)
    //{
    //    currentLanageTable = allLanageTable[type];

    //    // 언어 변경 시 기존의 언어 변경
    //    EventManager.Instance.OnChangeLanuage?.Invoke();

    //    // 폰트 에셋 변경
    //    FontManager.Instance.ApplyFont(type);

    //    currentLanguageType = type;
    //}

    //#region 프로퍼티

    //public Dictionary<LanguageType, Dictionary<string, string>> AllLanageTable {  get { return allLanageTable; } }
    //public Dictionary<string, string> CurrentLanageTable { get { return currentLanageTable; } }
    //public LanguageType CurrentLanaugeType {  get { return currentLanguageType; } }

    //#endregion

    #endregion
}
