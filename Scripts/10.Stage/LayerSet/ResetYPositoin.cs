using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetYPositoin : MonoBehaviour
{
    void LateUpdate()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers)
        {
            sr.sortingOrder = -(Mathf.RoundToInt(sr.transform.position.y * 100));
        }
    }
}
