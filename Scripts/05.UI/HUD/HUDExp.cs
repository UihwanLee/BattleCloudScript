using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class HUDExp : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image expFill;
    [SerializeField] private TextMeshProUGUI currentValue;
    [SerializeField] private TextMeshProUGUI maxValue;

    [Header("Attribute")]
    [SerializeField] private float exp;
    [SerializeField] private float maxExp;

    private PlayerCondition condition;

    private void Reset()
    {
        expFill = transform.FindChild<Image>("ExpFill");
    }

    private void OnEnable()
    {
        EventBus.Subscribe(AttributeType.Exp, UpdateUI);
        EventBus.Subscribe(AttributeType.MaxExp, UpdateUI);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe(AttributeType.Exp, UpdateUI);
        EventBus.UnSubscribe(AttributeType.MaxExp, UpdateUI);
    }

    private void Start()
    {
        maxExp = GameManager.Instance.Player.Condition.MaxExp.Value;
        exp = GameManager.Instance.Player.Condition.Exp.Value;
    }

    //private void UpdateExp(float exp)
    //{
    //    this.exp = exp;
    //    UpdateUI(exp;
    //}

    //private void UpdateMaxExp(float maxExp)
    //{
    //    this.maxExp = maxExp;
    //    UpdateUI(maxExp);
    //}

    private void UpdateUI(float value)
    {
        if (expFill == null)
        {
            Debug.Log($"{expFill.gameObject.name} is null");
            return;
        }

        if (maxExp <= 0)
            maxExp = 1f;

        if (condition == null)
        {
            condition = GameManager.Instance.Player.Condition;
        }

        exp = condition.Exp.Value;
        maxExp = condition.MaxExp.Value;
        expFill.fillAmount = condition.Exp.Value / condition.MaxExp.Value;
        currentValue.text = ((int)condition.Exp.Value).ToString();
        maxValue.text = ((int)condition.MaxExp.Value).ToString();
    }
}
