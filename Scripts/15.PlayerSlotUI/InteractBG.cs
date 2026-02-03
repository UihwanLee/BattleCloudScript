using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractBG : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("컴포넌트")]
    [SerializeField] private Image bg;
    [SerializeField] private Image bg_highlight;
    [SerializeField] private Color interactColor;

    private Color originColor = Color.white;

    private Vector3 originPosition;
    private Vector3 interactPosition;
    private int index;

    private void Reset()
    {
        bg = transform.FindChild<Image>("Slot_BG");
    }

    public void SetOffset(Vector3 _originPosition, Vector3 _initeractPosition)
    {
        originPosition = _originPosition;
        interactPosition = _initeractPosition;
    }

    public void InitIndex(int _index)
    {
        index = _index;
    }

    public void HighlightInteractBG(bool actvive)
    {
        Vector3 targetPosition = (actvive) ? interactPosition : originPosition;
        transform.localPosition = targetPosition;
        bg_highlight.gameObject.SetActive(actvive);
        bg_highlight.transform.localPosition = targetPosition;

        Color color = (actvive) ? interactColor : originColor;
        bg_highlight.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.Player.PlayerSlotUI.HighlightInteractBG(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.Player.PlayerSlotUI.HighlightOffInteractBG(index);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.DetailViewUI != null)
        {
            // DetailViewUI 슬롯 열기
            GameManager.Instance.DetailViewUI.SetUI(index);

            // Interact 설정
            GameManager.Instance.Player.PlayerSlotUI.SetHighlightInteract(index);
        }
    }
}
