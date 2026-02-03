using System;
using UnityEngine;

// Weapon Stat Data 컨테이너 클래스
[Serializable]
public class WeaponStatData 
{
    [Header("ID")]
    public int ID;                                          // ID
    [Header("기본 데미지")]
    public float BASE_DAMAGE;                               // 기본데미지
    [Header("사거리")]
    public float RANGE;                                     // 사거리
    [Header("범위")]
    public float EXTENT;                                    // 범위
    [Header("넉백")]
    public float KNOCKBACK;                                 // 넉백
    [Header("이동속도")]
    public float MOVE_SPEED;                                // 이동속도
    [Header("공격주기")]
    public float ATTACK_INTERVAL;                           // 공격주기
}
