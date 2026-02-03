
using System;
using UnityEngine;

// GameRuleData 컨테이너 클래스
[Serializable]
public class GameRulePhaseData 
{
    [Header("페이즈 이름")]
    public string PHASE;
    [Header("페이즈 지속 시간")]
    public float DURATION;
    [Header("몬스터 스탯 증가하는 비율")]
    public float MONSTER_STAT_MULTIPLIER;
    [Header("동시 몬스터 상한")]
    public float CONCURRENT_MONSTER_LIMIT;
    [Header("기본 리젠 조건")]
    public float SUMMON_IMMEDIATELY;
    [Header("기본 리젠 간격")]
    public float MONSTER_RESPAWN_TIME;
    [Header("시간 경과 가속")]
    public float TIME_LAPSE_ACCELERATION;
    [Header("몬스터가 주는 경험치")]
    public float MONSTER_GAIN_EXP;
    [Header("몬스터 에너지 경험치 가중치")]
    public float MONSTER_GAIN_ENERGY_RATE;
}
