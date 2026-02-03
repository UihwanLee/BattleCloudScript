using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class HUDHealth : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image hpFill;
    [SerializeField] private Image hpFillLock;
    [SerializeField] private TextMeshProUGUI currentValue;
    [SerializeField] private TextMeshProUGUI maxValue;

    [Header("Attribute")]
    [SerializeField] private float hp;
    [SerializeField] private float maxHp;

    private PlayerCondition condition;

    private void Reset()
    {
        hpFill = transform.FindChild<Image>("HpFill");
        hpFillLock = transform.FindChild<Image>("HpFillLock");
    }

    private void OnEnable()
    {
        // 이벤트 등록
        EventBus.Subscribe(AttributeType.Hp, UpdateUI);
        EventBus.Subscribe(AttributeType.MaxHp, UpdateUI);
        EventBus.SetHUDHealthLock += SetHpLock;
    }

    private void Start()
    {
        maxHp = GameManager.Instance.Player.Condition.MaxHp.Value;
        hp = maxHp;

        hpFillLock.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // 이벤트 해제
        EventBus.UnSubscribe(AttributeType.Hp, UpdateUI);
        EventBus.UnSubscribe(AttributeType.MaxHp, UpdateUI);
        EventBus.SetHUDHealthLock -= SetHpLock;
    }

    //private void UpdateHp(float hp)
    //{
    //    this.hp = hp;
    //    UpdateUI();
    //}

    //private void UpdateMaxHp(float maxHp)
    //{
    //    this.maxHp = maxHp;
    //    UpdateUI();
    //}

    private void UpdateUI(float value)
    {
        if (hpFill == null)
        {
            Debug.Log($"{hpFill.gameObject.name} is null");
            return;
        }

        if (maxHp <= 0f)
            maxHp = 1f;

        if (condition == null)
        {
            condition = GameManager.Instance.Player.Condition;
        }

        hpFill.fillAmount = condition.Hp.Value / condition.MaxHp.Value;
        currentValue.text = ((int)condition.Hp.Value).ToString();
        maxValue.text = ((int)condition.MaxHp.Value).ToString();
    }

    public void SetHpLock(bool isActive)
    {
        hpFillLock.gameObject.SetActive(isActive);
    }
}
