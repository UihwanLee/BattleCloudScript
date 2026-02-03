using System;
using UnityEngine;

// Player View Data 컨테이너 클래스
[Serializable]
public class WeaponTraitData
{
    [Header("조언자 ID")]
    public int ID;                                          // ID
    [Header("조언자 이름")]
    public string NAME;                                     // 이름
    [Header("조언자 설명")]
    public string DESC;                                     // 설명
    [Header("조언자 프리팹")]
    public string PREFAB;                                   // 프리팹
    [Header("조언자 타입")]
    public string TYPE;                                     // 타입
    [Header("조언자 레벨")]
    public float LV;                                        // 레벨
    [Header("조언자 최대 경험치")]
    public float MAX_EXP;                                   // 최대 경험치
    [Header("조언자 경험치 계수")]
    public float EXP_COEFFICIENT;                           // 레벨업계수
    [Header("조언자 연결할 STAT ID")]
    public int ID_STAT;                                     // 연결할 STAT ID
    [Header("조언자 연결할 지휘력")]
    public float SCALE;
    [Header("조언자 상점 가격")]
    public float PRICE;                                     // 조언자 상점 가격
    [Header("조언자 티어")]
    public string TIER;                                      // 조언자 티어
    [Header("조언자 ApplyTarget 타입")]
    public string ItemApplyTarget;                         // 조언자 ApplayTarget 타입
}

