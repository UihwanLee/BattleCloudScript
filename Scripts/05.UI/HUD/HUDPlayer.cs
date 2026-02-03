using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class HUDPlayer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image playerIcon;
    [SerializeField] private TextMeshProUGUI lvTxt;

    private PlayerCondition condition;

    private void Reset()
    {
        playerIcon = transform.FindChild<Image>("Icon");
        lvTxt = transform.FindChild<TextMeshProUGUI>("Lv");
    }

    private void OnEnable()
    {
        // 이벤트 등록
        EventBus.OnPlayerLevelUp += UpdateLvUI;
        EventBus.OnPlayerStart += UpdatePlayerIcon;
    }

    private void OnDisable()
    {
        // 이벤트 해제
        EventBus.OnPlayerLevelUp -= UpdateLvUI;
        EventBus.OnPlayerStart -= UpdatePlayerIcon;
    }

    public void UpdatePlayerIcon(Sprite icon)
    {
        playerIcon.sprite = icon;
    }

    public void UpdateLvUI(float value)
    {
        lvTxt.text = $"Lv. {value}";
    }
}
