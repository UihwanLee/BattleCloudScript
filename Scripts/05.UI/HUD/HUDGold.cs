using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class HUDGold : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Attribute")]
    [SerializeField] private int goldAmount;

    private void Reset()
    {
        goldText = transform.FindChild<TextMeshProUGUI>("GoldAmountText");
    }

    private void OnEnable()
    {
        // 이벤트 등록
        EventBus.Subscribe(AttributeType.Gold, UpdateGold);
    }

    private void Start()
    {
        if(GameManager.Instance == null) 
        { Debug.Log("GameManager가 없음"); }
        if (GameManager.Instance.Player == null)
        { Debug.Log("Player가 없음"); }
        goldAmount = (int)GameManager.Instance.Player.Condition.Gold.Value;
        UpdateUI();
    }

    private void OnDisable()
    {
        // 이벤트 해제
        EventBus.UnSubscribe(AttributeType.Gold, UpdateGold);
    }

    private void UpdateGold(float gold)
    {
        goldAmount = (int)gold;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (goldText == null)
        {
            Debug.Log($"{goldText.gameObject.name} is null");
            return;
        }
        
        goldText.text = goldAmount.ToString();
    }
}
