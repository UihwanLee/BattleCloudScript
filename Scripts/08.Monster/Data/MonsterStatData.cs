using System;
using UnityEngine;

[Serializable]
public class MonsterStatData
{
    [Header("몬스터 STAT ID")]
    public int ID;                                          // ID
    [Header("몬스터 이동속도")]
    public float MOVE_SPEED;                                // 이동속도
    [Header("몬스터 데미지")]
    public float BASE_DAMAGE;                               // 기본데미지
    [Header("몬스터 공격 주기")]
    public float ATTACK_INTERVAL;                           // 공격주기
    [Header("몬스터 사거리")]
    public float RANGE;                                     // 사거리
    [Header("몬스터 넉백 저항")]
    public float KNOCKBACK_RESIST;                          // 넉백저항
    [Header("몬스터 특수 태그")]
    public string SPECIAL_TAG;                              // 특수 태그
    [Header("몬스터 특수 수치")]
    public float SPECIAL_VALUE;                             // 툭수 수치
    [Header("몬스터 스폰 가중치")]
    public float SPECIAL_WEIGHT;                            // 스폰 가중치
    [Header("몬스터 피격 시 효과")]
    public string ON_HIT_EFFECT;                            // 피격 시 효과
}
