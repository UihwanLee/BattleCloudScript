using System;
using UnityEngine;

// Player View Data 컨테이너 클래스
[Serializable]
public class PlayerTraitData
{
    [Header("플레이어 ID")]
    public int ID;                                          // ID
    [Header("플레이어 이름")]
    public string NAME;                                     // 이름
    [Header("플레이어 설명")]
    public string DESC;                                     // 설명
    [Header("플레이어 이미지")]
    public string Image;                                    // 이미지
    [Header("플레이어 프리팹")]
    public string PREFAB;                                   // 연결할 WEAPON ID
    [Header("플레이어 레벨")]
    public float LV;                                        // 레벨
    [Header("플레이어 체력")]
    public float MAX_HP;                                     // 최대 체력
    [Header("플레이어 체력 계수")]
    public float HP_COEFFICIENT;                            // 최대 체력 계수
    [Header("플레이어 최대 경험치")]
    public float MAX_EXP;                                   // 최대 경험치
    [Header("플레이어 경험치 계수")]
    public float EXP_COEFFICIENT;                           // 레벨업계수
    [Header("플레이어 레벨업 당 늘어나는 STAT 계수")]
    public float LEVEL_STAT_COEFFICIENT;                    // 레벨업 당 STAT 올라가는 계수
    [Header("플레이어 연결할 STAT ID")]
    public int ID_STAT;                                     // 연결할 STAT ID
    [Header("플레이어 연결할 WEAPON ID")]
    public int ID_WEAPON;                                   // 연결할 WEAPON ID
}
