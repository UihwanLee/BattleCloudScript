using System;
using UnityEngine;

[Serializable]
public class GameRuleDropRate
{
    [Header("드롭 종류")]
    public string DROP;
    [Header("드롭 확률")]
    public float DROP_RATE;
}
