using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCursor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector2 basePos;
    [SerializeField] private Vector2 offset;

    [Header("Rect Transform")]
    [SerializeField] private RectTransform rect;

    private void Reset()
    {
        rect = GetComponent<RectTransform>();
    }

    public void UpdatePosition(int index)
    {
        Vector2 pos = rect.anchoredPosition;
        pos.x = basePos.x + (offset.x * index);
        pos.y = basePos.y - (offset.y * index);
        rect.anchoredPosition = pos;
    }
}
