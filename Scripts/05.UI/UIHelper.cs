using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public static class UIHelper
{
    // 자주 사용하는 16진수 색상 코드 정의 
    public const string ColorRed = "#FF4B4B";
    public const string ColorGreen = "#4BFF4B";
    public const string ColorBlue = "#4BBAFF";
    public const string ColorYellow = "#FFFF4B";
    public const string ColorOrange = "#FF9800"; 

    /// <summary>
    /// 수치 데이터에 Rich Text 색상 태그를 입혀줍니다.
    /// </summary>
    /// <param name="value">표시할 수치</param>
    /// <param name="colorHex">16진수 색상 코드 (예: "#FF0000" 또는 "red")</param>
    /// <returns>색상 태그가 포함된 문자열</returns>
    public static string ColorText(float value, string colorHex)
    {
        return $"<color={colorHex}>{Mathf.RoundToInt(value)}</color>";
    }

    public static string ColorTextWithType(float value, ItemApplyType type, string colorHex)
    {
        int roundedValue = Mathf.RoundToInt(value);
        // ADD 타입이면 뒤에 %를 붙임
        string result = type == ItemApplyType.PERCENT ? $"{roundedValue}%" : $"{roundedValue}";

        return $"<color={colorHex}>{result}</color>";
    }

    public static string ColorTextWithTypeAndNextLine(float value, ItemApplyType type, string colorHex)
    {
        int roundedValue = Mathf.RoundToInt(value);
        // ADD 타입이면 뒤에 %를 붙임
        string result = type == ItemApplyType.PERCENT ? $"{roundedValue}%" : $"{roundedValue}";

        return $"<color={colorHex}>{result}</color>\n";
    }

    /// <summary>
    /// TMP 텍스트 타이핑 효과 실행 (확장 메서드 방식)
    /// 사용 예: this.StartTyping(targetTxt, "Hello World", 0.1f);
    /// </summary>
    public static Coroutine StartTyping(this MonoBehaviour mono, TextMeshProUGUI textUI, string fullText, float speed = 0.05f)
    {
        return mono.StartCoroutine(TypingRoutine(textUI, fullText, speed));
    }

    private static IEnumerator TypingRoutine(TextMeshProUGUI textUI, string fullText, float speed)
    {
        textUI.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            textUI.text += letter;
            yield return new WaitForSeconds(speed);
        }
    }
}
