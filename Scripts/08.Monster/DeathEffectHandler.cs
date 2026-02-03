using UnityEngine;
using System.Collections;

public class DeathEffectHandler : MonoBehaviour
{
    [Header("축소")]
    [SerializeField] private float shrinkDuration = 0.25f;

    [Header("회전")]
    [SerializeField] private float rotateSpeed = 720f; // 초당 회전 각도

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isPlaying;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        // 풀 재사용 대비 초기화
        transform.localScale = originalScale;
        transform.rotation = originalRotation;
        isPlaying = false;
    }

    public IEnumerator PlayDeathEffect()
    {
        if (isPlaying)
            yield break;

        isPlaying = true;

        float t = 0f;
        Vector3 startScale = transform.localScale;

        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float ratio = t / shrinkDuration;

            // 회전
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);

            // 축소
            transform.localScale = Vector3.Lerp(
                startScale,
                Vector3.zero,
                ratio
            );

            yield return null;
        }

        transform.localScale = Vector3.zero;
    }
}
