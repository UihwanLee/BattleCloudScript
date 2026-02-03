using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
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

    private SaveManager() { }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Newtonsoft.Json 설정: 상속 구조 및 객체 타입을 보존하도록 설정
    private JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto, // 상속 관계 저장의 핵심
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // 순환 참조 방지
        Formatting = Formatting.Indented // 보기 좋게 들여쓰기
    };

    // 저장될 경로 설정
    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, $"BattleCloud_SaveData.json");
    }

    public void Save()
    {
        // 현재 정보를 저장
        SaveData saveData = new SaveData();

        // 현재 시간을 원하는 포맷으로 변환 (yyyy: 년, MM: 월, dd: 일, HH: 시, mm: 분)
        saveData.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");

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

        // Newtonsoft.Json을 사용하여 직렬화
        string json = JsonConvert.SerializeObject(saveData, _settings);

        // 파일로 저장
        File.WriteAllText(GetSavePath(), json);

        Debug.Log($"SaveData 저장 완료!");
    }

    public SaveData Load()
    {
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            Debug.LogWarning($"저장된 파일이 없습니다.");
            return null;
        }

        // 파일 읽어오기
        string json = File.ReadAllText(path);

        // 역직렬화 시에도 동일한 설정을 전달하여 상속 구조 복구
        SaveData loadedData = JsonConvert.DeserializeObject<SaveData>(json, _settings);

        return loadedData;
    }

    public void Delete()
    {
        
    }
}
