
using System;

// 레벨업 보상 롤 컨테이너 클래스
[Serializable]
public class LevelUpRewardRuleData 
{
    public int ID;
    public float DAY_MIN;
    public float DAY_MAX;
    public float COMMON;
    public float UNCOMMON;
    public float RARE;
}
