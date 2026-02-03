using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using WhereAreYouLookinAt.Enum;

[System.Serializable]
public class DialogData
{
    public LocalizedString npcNameKey;          // NPC 이름
    public Sprite icon;                         // NPC 아이콘
    public string messageKey;                   // 대화 키
    public string message_KR;                   // NPC 대화 메세지_한국어
    public string message_EN;                   // NPC 대화 메세지_한국어
    public DialogPositionType positionType;     // NPC UI 위치
}

// Dialog 스크립트 - 한 시나리오의 대화 리스트를 담고 있다.
[CreateAssetMenu(fileName = "newDialog", menuName = "Dialog/newDialog")]
public class Dialog : ScriptableObject
{
    // 대화 시 사용하는 Dialog 데이터 : 대화에 대한 내용이 들어있음
    public List<DialogData> dialogList;             // NPC 대화 리스트
}