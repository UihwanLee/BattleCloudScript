using System;
using UnityEngine;

// GameRuleExpWeapon 컨테이너 클래스
[Serializable]
public class GameRuleExpWeaponData
{
    [Header("KEY")]
    public string KEY;                                  // WeaponType
    [Header("1차 상승 범위")]
    public float LV_RANGE_1;                           // 1차 상승폭
    [Header("1차 고정 값")]
    public float DAMAGE_FIXED_1;                            // 1차 고정값
    [Header("1차 공격 속도 값")]
    public float ATTACK_INTERVAL_FIXED_1;
    [Header("2차 상승 범위")]
    public float LV_RANGE_2;                           // 2차 상승폭
    [Header("2차 고정 값")]
    public float DAMAGE_FIXED_2;                            // 2차 고정값
    [Header("2차 공격 속도 값")]
    public float ATTACK_INTERVAL_FIXED_2;
    [Header("3차 상승 범위")]
    public float LV_RANGE_3;                           // 3차 상승폭
    [Header("3차 고정 값")]
    public float DAMAGE_FIXED_3;                            // 3차 고정값
    [Header("3차 공격 속도 값")]
    public float ATTACK_INTERVAL_FIXED_3;
    [Header("4차 상승 범위")]
    public float LV_RANGE_4;                           // 4차 상승폭
    [Header("4차 고정 값")]
    public float DAMAGE_FIXED_4;                            // 4차 고정값
    [Header("4차 공격 속도 값")]
    public float ATTACK_INTERVAL_FIXED_4;
}
