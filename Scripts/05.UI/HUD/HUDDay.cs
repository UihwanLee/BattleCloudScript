using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDDay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nowDayText;

    private void Reset()
    {
        nowDayText = transform.FindChild<TextMeshProUGUI>("NowDayText");
        nowDayText.text = 1.ToString("D2");
    }

    private void OnEnable()
    {
        EventBus.OnDayChanged += UpdateDay;
    }

    private void OnDisable()
    {
        EventBus.OnDayChanged -= UpdateDay;
    }

    private void UpdateDay(int day)
    {
        nowDayText.text = day.ToString("D2");
    }
}
