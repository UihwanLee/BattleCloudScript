using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleInfoUI : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private Image circle;

    [Header("연출 시간")]
    [SerializeField] private float duration;

    private void Reset()
    {
        circle = transform.FindChild<Image>("Model");
        duration = 1.0f;
    }

    #region 프로퍼티

    public Image Circle { get { return circle; } }
    public float Duration { get { return duration; } }

    #endregion
}
