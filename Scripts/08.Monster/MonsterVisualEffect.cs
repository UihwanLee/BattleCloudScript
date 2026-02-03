using System;
using System.Collections;
using UnityEngine;

public class MonsterVisualEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("페이즈별 마테리얼")]
    [SerializeField] private Material monsterPhase1;
    [SerializeField] private Material monsterPhase2;
    [SerializeField] private Material monsterPhase3;

    private Material currentMaterial;

    [Header("축소 시간")]
    [SerializeField] private float shrinkDuration = 0.25f;
    [Header("회전 속도")]
    [SerializeField] private float rotateSpeed = 720f;

    private Vector3 originalScale;
    private Quaternion originalRotation;

    private MaterialPropertyBlock materialPropertyBlock;

    private Coroutine hitRoutine;
    private Coroutine attackEffectRoutine;
    private Coroutine fadeRoutine;
    private Coroutine deathRoutine;

    private const string HIT_BLEND = "_HitEffectBlend";
    private const string FADE_AMOUNT = "_FadeAmount";

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        materialPropertyBlock = new MaterialPropertyBlock();

        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    private void OnDisable()
    {
        ResetVisual();
    }

    #region OutLine

    public void ApplyWaveOutLine(int phase)
    {
        Color color = Color.white;

        switch (phase)
        {
            case 1:
                currentMaterial = monsterPhase1;
                break;
            case 2:
                currentMaterial = monsterPhase2;
                break;
            case 3:
                currentMaterial = monsterPhase3;
                break;
        }

        spriteRenderer.material = currentMaterial;
    }

    #endregion

    #region Hit Effect

    public void PlayHitEffect(float duration = 0.08f, float strength = 1f)
    {
        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(HitEffectRoutine(duration, strength));
    }

    private IEnumerator HitEffectRoutine(float duration, float strength)
    {
        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(HIT_BLEND, strength);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);

        yield return new WaitForSeconds(duration);

        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(HIT_BLEND, 0f);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);

        hitRoutine = null;
    }

    #endregion

    #region Attack Effect
    public void PlayAttackEffect(float duration)
    {
        if (attackEffectRoutine != null)
            StopCoroutine(attackEffectRoutine);

        attackEffectRoutine = StartCoroutine(AttackEffectRoutine(duration));
    }

    private IEnumerator AttackEffectRoutine(float duration)
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(duration);

        spriteRenderer.color = Color.white;
    }
    #endregion

    #region Fade Out & Despawn

    public void PlayFadeOut(Action onComplete, float duration = 1f)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutRoutine(onComplete, duration));
    }

    private IEnumerator FadeOutRoutine(Action onComplete, float duration)
    {
        float time = 0f;

        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat("_FadeAmount", 0f);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);

        while (time < duration)
        {
            time += Time.deltaTime;
            float fade = Mathf.Lerp(0f, 1f, time / duration);

            spriteRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetFloat(FADE_AMOUNT, fade);
            spriteRenderer.SetPropertyBlock(materialPropertyBlock);

            yield return null;
        }

        onComplete?.Invoke();
    }
    #endregion

    #region Death Effect
    public void PlayDeathEffect(Action onComplete)
    {
        if (deathRoutine != null)
            StopCoroutine(deathRoutine);

        deathRoutine = StartCoroutine(DeathEffectRoutine(onComplete));
    }

    private IEnumerator DeathEffectRoutine(Action onComplete)
    {
        float time = 0f;

        Vector3 startScale = transform.localScale;

        while (time < shrinkDuration)
        {
            time += Time.deltaTime;

            float ratio = time / shrinkDuration;

            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, ratio);

            yield return null;
        }

        transform.localScale = Vector3.zero;

        onComplete?.Invoke();
    }
    #endregion

    public void ResetVisual()
    {
        transform.localScale = originalScale;
        transform.rotation = originalRotation;

        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(HIT_BLEND, 0f);
        materialPropertyBlock.SetFloat(FADE_AMOUNT, 0f);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
        spriteRenderer.color = Color.white;
    }
}
