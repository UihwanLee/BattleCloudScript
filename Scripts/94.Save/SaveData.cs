using System;
using System.Collections.Generic;
using UnityEngine;

// 게임 상태를 저장하는 구조체
public class GameStateData
{
    public int Day {  get; set; }
    public bool IsTutorial { get; set; }
}

// PlayerSlot 상태를 저장하는 구조체
public class PlayerSlotData
{
    public int MaxWeaponSlotCount { get; set; }            // 최대 WeaponSlot 개수
    public int MaxItemSlotCount { get; set; }               // 최대 ItemSlot 개수
    public int ActiveWeaponSlotCount { get; set; }          // 해금된 weaponSlot 개수
    public int ActiveItemSlotCount { get; set; }            // 해금된 itemSlot 개수
    public int CurrentActiveWeaponSlotCount { get; set; }   // 현재 활성화 된 weaponSlot 개수
    public int CurrentActiveItemSlotCount { get; set; }     // 현재 활성화 된 itemSlot 개수
}

// WeaponSlot 상태를 저장하는 구조체
public class WeaponSlotData
{
    public int Index { get; set; }
    public int ItemID { get; set; }
    public ISlotItem Item { get; set; }
    public bool IsActvie { get; set; }
    public List<ItemSlotData> ItemSlotsData { get; set; }
}

// ItemSlot의 상태를 저장하는 구조체
public class ItemSlotData
{
    public ISlotItem Item { get; set; }
    public int ItemID { get; set; }
    public bool IsActvie { get; set; }
}


// 게임 내 저장할 SaveData 컨테이너 클래스
[Serializable]
public class SaveData
{
    [Header("환경 정보")]
    public string saveTime; 

    [Header("게임 상태 정보")]
    public GameStateData gameStateData;

    [Header("PlayerSlot 상태 정보")]
    public PlayerSlotData playerSlotData;

    [Header("슬롯 정보")]
    public List<WeaponSlotData> weaponSlots = new List<WeaponSlotData>();
}
