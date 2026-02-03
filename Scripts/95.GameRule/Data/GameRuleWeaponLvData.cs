using System;
using UnityEngine;

// GameRuleWeaponLv 컨테이너 클래스
[Serializable]
public class GameRuleWeaponLvData
{
    [Header("ID")]
    public int ID;
    [Header("LV")]
    public float LV;
    [Header("분해 비용")]
    public float DISASSEMBLY_COST;
    [Header("강화 비용")]
    public float REINFORCEMENT_COST;
}
