using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    private Coroutine hitRoutine;
    private const string HIT_BLEND = "_HitEffectBlend";

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        GameManager.Instance.Player.VisualEffect = this;
    }

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
}
