using UnityEngine;

public class MonsterPhaseVisual : MonoBehaviour
{
    [Header("페이즈별 마테리얼")]
    [SerializeField] private Material monsterPhase1;
    [SerializeField] private Material monsterPhase2;
    [SerializeField] private Material monsterPhase3;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ApplyPhaseMaterial(int phase)
    {
        OldMonsterController mc = GetComponent<OldMonsterController>();
        if (mc != null && mc.IsDespawning)
            return;

        if (TryGetComponent<HitEffectHandler>(out var hit))
        {
            hit.ForceClear();
        }

        switch (phase)
        {
            case 1:
                spriteRenderer.material = monsterPhase1;
                break;

            case 2:
                spriteRenderer.material = monsterPhase2;
                break;

            case 3:
                spriteRenderer.material = monsterPhase3;
                break;
        }

        var mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_FadeAmount", 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
