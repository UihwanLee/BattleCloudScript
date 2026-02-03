
using System;

// 레벨업 강화 컨테이너 클래스
[Serializable]
public class LevelUpRewardEnhancementData
{
    public string TIER;             // 티어 (Key)
    public string NAME;             // 이름
    public string DESC;             // 설명
    public string IMAGE;            // 이미지
    public string EFFECT;           // 효과
    public string APPLYTARGET;      // 효과 적용 대상
    public float VALUE;             // 수치 
}
