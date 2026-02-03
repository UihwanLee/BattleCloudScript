using System;
using UnityEngine;

// GameRuleWaveData 컨테이너 클래스
[Serializable]
public class GameRuleWaveData
{
    [Header("ID")]
    public int ID;
    [Header("웨이브")]
    public float WAVE;
    [Header("리롤 비용")]
    public float REROLL_VALUE;
    [Header("리롤 증가량")]
    public float REROLL_MULTIPLIER;
}
