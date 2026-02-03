using System;
using UnityEngine;

[Serializable]
public class GameRuleDayData
{
    [Header("ID")]
    public int ID;
    [Header("시작 Day")]
    public float DAY_MIN;
    [Header("끝 Day")]
    public float DAY_MAX;
    [Header("몬스터 체력 배율")]
    public float MONSTER_HP_MULTIPLIER;
    [Header("요구 게이지 에너지")]
    public float REQUIRED_GATE_ENERGY;
    [Header("티어1 확률")]
    public float RATE_TIER1;
    [Header("티어2 확률")]
    public float RATE_TIER2;
    [Header("티어3 확률")]
    public float RATE_TIER3;
    [Header("티어4 확률")]
    public float RATE_TIER4;
    [Header("무기 드롭 확률")]
    public float RATE_WEAPON;
    [Header("아이템 드롭 확률")]
    public float RATE_ITEM;
    [Header("아이템 평균 레벨")]
    public float ITEM_AVERAGE_LEVEL;
    [Header("조언자 평균 레벨")]
    public float WEAPON_AVERAGE_LEVEL;
}
