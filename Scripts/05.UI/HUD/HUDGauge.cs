using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDGauge : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dayText;

    [Header("Attribute")]
    [SerializeField] private int currentDay;

    private void Reset()
    {
        dayText = transform.FindChild<TextMeshProUGUI>("DayText");
    }

    private void OnEnable()
    {
        // 이벤트 구독
        EventBus.OnDayChanged += UpdateDay;

    }

    private void OnDisable()
    {
        // 이벤트 해제
        EventBus.OnDayChanged -= UpdateDay;
    }

    private void UpdateDay(int day)
    {
        dayText.text = $"Wave {day}";
    }
}
