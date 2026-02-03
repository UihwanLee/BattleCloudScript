using System;
using UnityEngine;

// Player View Data 컨테이너 클래스
[Serializable]
public class MonsterTraitData
{
    [Header("몬스터 ID")]
    public int ID;                                      // ID
    [Header("몬스터 이름")]
    public string NAME;                                 // 이름
    [Header("몬스터 설명")]
    public string DESC;                                 // 설명
    [Header("몬스터 프리팹")]
    public string PREFAB;                               // 프리팹
    [Header("몬스터 체력")]
    public float MAX_HP;                                // HP
    [Header("몬스터 체력 배율")]
    public float HP_INCREASE_RATE;                      // HP 체력 배율
    [Header("몬스터 연결 ID")]
    public int ID_STAT;                                 // 연결할 STAT ID
}

