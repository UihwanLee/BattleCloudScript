using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{
    [SerializeField] private float typingDelay = 0.1f;

    private WaitForSeconds tyingDelay;

    private Coroutine typingCoroutine;

    private void Start()
    {
        tyingDelay = new WaitForSeconds(typingDelay);
    }

    public void Typing(TextMeshProUGUI textUI, string message, Action onCompleted)
    {
        StartCoroutine(TypingCoroutine(textUI, message, onCompleted));
    }

    private IEnumerator TypingCoroutine(TextMeshProUGUI textUI, string message, Action onCompleted)
    {
        // 초반 text UI 비우기
        textUI.text = string.Empty;

        foreach (char c in message)
        {
            textUI.text += c;

            yield return tyingDelay;
        }

        // Typing 끝났다고 Action 델리게이트에 알림
        onCompleted?.Invoke();
        yield return null;
    }
}