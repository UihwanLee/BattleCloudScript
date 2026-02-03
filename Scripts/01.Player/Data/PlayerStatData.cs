using System;
using UnityEngine;

// Player Stat Data 컨테이너 클래스
[Serializable]
public class PlayerStatData 
{
    [Header("ID")]
    public int ID;                                      // ID
    [Header("지휘력")]
    public float COMMAND;                               // 지휘력
    [Header("이동 속도")]
    public float MOVE_SPEED;                            // 이동속도
    [Header("회피")]
    public float EVASION;                               // 회피
    [Header("방어")]
    public float DEFENSE;                               // 방어
    [Header("행운")]
    public float LUCK;                                  // 행운
}
