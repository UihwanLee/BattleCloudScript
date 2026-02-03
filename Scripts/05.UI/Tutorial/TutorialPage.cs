using UnityEngine;

public enum TutorialLayoutType
{
    Normal,     // 기존 페이지
    Extended    // 이미지 2 + 설명 2 페이지
}

[System.Serializable]
public class TutorialPage
{
    public TutorialLayoutType layoutType = TutorialLayoutType.Normal;

    // 기존
    public Sprite icon;
    public string title;
    [TextArea]
    public string description;

    // 첫페이지 전용 
    public Sprite subIcon;
    [TextArea]
    public string subDescription;
}
