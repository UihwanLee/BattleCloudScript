using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class PlayerHpBar : MonoBehaviour
{
    [Header("Hp Fill")]
    [SerializeField] private Image hpFill;
    [SerializeField] private Image hpLock;

    [Header("Attribute")]
    [SerializeField] private float hp;
    [SerializeField] private float maxHp;

    [Header("Color")]
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private Color normalColor = Color.red;

    [Header("Time")]
    [SerializeField] private float colorChangeDelay = 0.5f;
    [SerializeField] private float hideDelay = 2f;

    private Coroutine hpBarRoutine;

    private PlayerCondition condition;

    private void Reset()
    {
        hpFill = transform.FindChild<Image>("Hp Fill");
        hpLock = transform.FindChild<Image>("Hp Lock");
    }

    private void OnEnable()
    {
        EventBus.Subscribe(AttributeType.Hp, UpdateHpBar);
        EventBus.Subscribe(AttributeType.MaxHp, UpdateHpBar);
    }

    private void OnDisable()
    {
        EventBus.UnSubscribe(AttributeType.Hp, UpdateHpBar);
        EventBus.UnSubscribe(AttributeType.MaxHp, UpdateHpBar);
    }

    private void UpdateHpBar(float value)
    {
        if (condition == null)
        {
            condition = GameManager.Instance.Player.Condition;
        }

        hpFill.fillAmount = condition.Hp.Value / condition.MaxHp.Value;
    }

    public void OnHit(bool isLock)
    {
        if (hpFill == null)
        {
            Debug.Log($"{hpFill.gameObject.name} is null");
            return;
        }

        if (maxHp <= 0f)
            maxHp = 1f;

        UpdateHpBar(hp);

        // 이상현상 패널티 효과 적용
        hpLock.gameObject.SetActive(isLock);

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (hpBarRoutine != null)
            StopCoroutine(hpBarRoutine);

        hpBarRoutine = StartCoroutine(HpBarSequence());
    }

    private IEnumerator HpBarSequence()
    {
        hpFill.color = hitColor;

        yield return new WaitForSeconds(colorChangeDelay);
        hpFill.color = normalColor;

        yield return new WaitForSeconds(hideDelay);
        gameObject.SetActive(false);
    }
}
