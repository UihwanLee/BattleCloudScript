using UnityEngine;
using System.Collections;

public class HitEffectHandler : MonoBehaviour
{
    [SerializeField] private float hitDuration = 0.08f;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine hitRoutine;

    // ✅ Hit Effect Blend 파라미터
    private static readonly int HitBlendID
        = Shader.PropertyToID("_HitEffectBlend");

    private void Awake()
    {
        // 안전하게 자식까지 탐색
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }
    private void OnEnable()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
            return;

        // 풀 재사용 시 히트 이펙트 강제 초기화
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(HitBlendID, 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }
    public void PlayHitEffect()
    {
        Debug.Log($"[HitEffect] PlayHitEffect 호출됨 : {name}");

        if (spriteRenderer == null)
        {
            Debug.LogError("[HitEffect] SpriteRenderer 없음");
            return;
        }

        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        spriteRenderer.GetPropertyBlock(mpb);

        // 히트 ON
        mpb.SetFloat(HitBlendID, 1f);
        spriteRenderer.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(hitDuration);

        // 히트 OFF
        mpb.SetFloat(HitBlendID, 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }
    public void ForceClear()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(HitBlendID, 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
