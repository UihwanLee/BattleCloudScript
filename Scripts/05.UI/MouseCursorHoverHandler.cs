using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WhereAreYouLookinAt.Enum;

public class MouseCursorHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CursorType hoverCursor = CursorType.Interact;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseCursorController.Instance.SetCursor(hoverCursor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseCursorController.Instance.SetCursor(CursorType.Default);
    }
}