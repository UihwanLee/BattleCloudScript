using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "BattleCloud_SaveData.json";

    private static SaveManager instance;

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SaveManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Newtonsoft.Json 설정: 상속 구조 및 객체 타입을 보존하도록 설정
    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto, // 상속 관계 저장의 핵심
        // 세이브 파일은 유저가 직접 수정할 수 있으므로 $type으로 게임 코드 외의 타입이 생성되지 않도록 제한
        SerializationBinder = new GameTypeSerializationBinder(),
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // 순환 참조 방지
        Formatting = Formatting.Indented // 보기 좋게 들여쓰기
    };

    // 저장될 경로 설정
    private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    private string TempPath => SavePath + ".tmp";
    private string BackupPath => SavePath + ".bak";

    public void Save()
    {
        // 현재 정보를 저장
        SaveData saveData = new SaveData();

        // 현재 시간을 원하는 포맷으로 변환 (yyyy: 년, MM: 월, dd: 일, HH: 시, mm: 분)
        saveData.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        // 게임 상태 저장
        GameStateData gameStateData = new GameStateData();
        gameStateData.Day = GameManager.Instance.DayManager.CurrentDay;
        gameStateData.IsTutorial = GameManager.Instance.IsTutorial;
        saveData.gameStateData = gameStateData;

        // PlayerSlot 상태 저장
        PlayerSlotUI playerSlotUI = GameManager.Instance.Player.PlayerSlotUI;
        PlayerSlotData playerSlotData = new PlayerSlotData();
        playerSlotData.MaxWeaponSlotCount = playerSlotUI.MaxWeaponSlotCount;
        playerSlotData.MaxItemSlotCount = playerSlotUI.MaxItemSlotCount;
        playerSlotData.ActiveWeaponSlotCount = playerSlotUI.ActiveWeaponSlotCount;
        playerSlotData.ActiveItemSlotCount = playerSlotUI.ActiveItemSlotCount;
        playerSlotData.CurrentActiveWeaponSlotCount = playerSlotUI.CurrentActiveWeaponSlotCount;
        playerSlotData.CurrentActiveItemSlotCount = playerSlotUI.CurrentActiveItemSlotCount;
        saveData.playerSlotData = playerSlotData;

        // Weapon/Item 슬롯 저장
        List<WeaponSlot> weaponSlots = playerSlotUI.WeaponSlots;
        foreach (var slot in weaponSlots)
        {
            WeaponSlotData weaponSlotData = new WeaponSlotData();
            weaponSlotData.ItemSlotsData = new List<ItemSlotData>();

            weaponSlotData.Index = slot.Index;
            weaponSlotData.IsActvie = slot.IsActive;
            weaponSlotData.Item = slot.Item;
            if (slot.Item != null) weaponSlotData.ItemID = slot.Item.GetID();

            foreach (var itemSlot in slot.ItemSlots)
            {
                ItemSlotData itemSlotData = new ItemSlotData();
                itemSlotData.Item = itemSlot.Item;
                itemSlotData.IsActvie = itemSlot.IsActive;
                if (itemSlot.Item != null) itemSlotData.ItemID = itemSlot.Item.GetID();
                weaponSlotData.ItemSlotsData.Add(itemSlotData);
            }

            saveData.weaponSlots.Add(weaponSlotData);
        }

        if (WriteToDisk(saveData))
            Debug.Log($"SaveData 저장 완료!");
    }

    public SaveData Load()
    {
        if (!File.Exists(SavePath) && !File.Exists(BackupPath))
        {
            Debug.LogWarning($"저장된 파일이 없습니다.");
            return null;
        }

        // 본 파일이 깨졌으면 직전 저장본(.bak)으로 복구 시도
        if (TryReadFromDisk(SavePath, out SaveData loadedData))
            return loadedData;

        if (TryReadFromDisk(BackupPath, out loadedData))
        {
            Debug.LogWarning("[SaveManager] 세이브 파일을 읽을 수 없어 백업 파일로 불러왔습니다.");
            return loadedData;
        }

        Debug.LogError("[SaveManager] 세이브 파일과 백업 파일을 모두 읽을 수 없습니다.");
        return null;
    }

    public void Delete()
    {
        foreach (string path in new[] { SavePath, TempPath, BackupPath })
        {
            if (File.Exists(path)) File.Delete(path);
        }

        Debug.Log("SaveData 삭제 완료!");
    }

    /// <summary>
    /// 임시 파일에 먼저 쓴 뒤 교체한다.
    /// 쓰는 도중 게임이 종료되어도 기존 세이브 파일은 손상되지 않고, 직전 저장본은 .bak으로 남는다.
    /// </summary>
    private bool WriteToDisk(SaveData saveData)
    {
        try
        {
            // Newtonsoft.Json을 사용하여 직렬화
            string json = JsonConvert.SerializeObject(saveData, _settings);
            File.WriteAllText(TempPath, json);

            if (File.Exists(SavePath))
                File.Replace(TempPath, SavePath, BackupPath);
            else
                File.Move(TempPath, SavePath);

            return true;
        }
        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is JsonException)
        {
            Debug.LogError($"[SaveManager] 저장 실패: {e.Message}");
            return false;
        }
    }

    private bool TryReadFromDisk(string path, out SaveData saveData)
    {
        saveData = null;
        if (!File.Exists(path)) return false;

        try
        {
            // 역직렬화 시에도 동일한 설정을 전달하여 상속 구조 복구
            saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path), _settings);
            return saveData != null;
        }
        catch (Exception e) when (e is IOException || e is JsonException)
        {
            Debug.LogWarning($"[SaveManager] {Path.GetFileName(path)} 읽기 실패: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// $type으로 지정된 타입이 게임 코드(SaveData와 같은 어셈블리)에 있는 경우만 허용
    /// </summary>
    private class GameTypeSerializationBinder : DefaultSerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type type = base.BindToType(assemblyName, typeName);

            if (type.Assembly != typeof(SaveData).Assembly)
                throw new JsonSerializationException($"허용되지 않은 세이브 데이터 타입: {typeName}");

            return type;
        }
    }
}
