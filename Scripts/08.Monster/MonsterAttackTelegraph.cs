using System.Collections;
using UnityEngine;

public class MonsterAttackTelegraph : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer overlayRenderer;

    [Header("연출")]
    [SerializeField] private float maxAlpha = 0.6f;
    [SerializeField] private float duration = 1.0f;

    private Coroutine routine;

    private void Awake()
    {
        if (overlayRenderer == null)
            overlayRenderer = GetComponent<SpriteRenderer>();

        overlayRenderer.enabled = false;
        SetAlpha(0f);
    }

    private void LateUpdate()
    {
        if (baseRenderer == null)
            return;

        overlayRenderer.sprite = baseRenderer.sprite;
        overlayRenderer.flipX = baseRenderer.flipX;
    }

    public void Bind(SpriteRenderer renderer)
    {
        baseRenderer = renderer;
    }

    public void Play()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PlayRoutine());
    }

    public void Stop()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = null;
        overlayRenderer.enabled = false;
        SetAlpha(0f);
    }

    private IEnumerator PlayRoutine()
    {
        overlayRenderer.enabled = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float n = t / duration;
            SetAlpha(Mathf.Sin(n * Mathf.PI) * maxAlpha);
            yield return null;
        }

        Stop();
    }

    private void SetAlpha(float a)
    {
        Color c = overlayRenderer.color;
        c.a = a;
        overlayRenderer.color = c;
    }

    private void OnDisable()
    {
        Stop();
    }
}
