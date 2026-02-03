using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStatInfoUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("정보")]
    [SerializeField] private string titleKey;
    [SerializeField] private string descKey;

    [Header("컴포넌트")]
    [SerializeField] private GameObject highlight;

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);
        EventBus.OnMouseOnInfo?.Invoke(titleKey, descKey, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
        EventBus.OnMouseOffSlot?.Invoke();
    }

    private void Start()
    {
        highlight.SetActive(false);
    }


}
