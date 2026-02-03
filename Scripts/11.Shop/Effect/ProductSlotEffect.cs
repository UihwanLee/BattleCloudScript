using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductSlotEffect : MonoBehaviour
{
    [Header("리소스")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image overlay;

    [Header("애니메이션 설정")]
    [Header("흰색 플래시 애니메이션 시간")]
    [SerializeField] private float whiteFlashDuration = 0.05f;
    [Header("스케일 업 스케일링")]
    [SerializeField] private float scalingValue = 1.1f;
    [Header("스케일 업 애니메이션 시간")]
    [SerializeField] private float scaleUpDuration = 0.12f;
    [Header("스케일 업 지속 시간 (대기 시간")]
    [SerializeField] private float delayScaleUpDuratuon = 0.1f;
    [Header("떨어지는 애니메이션 시간")]
    [SerializeField] private float fallingDuration = 0.1f;


    public void Play(System.Action onComplete)
    {
        StartCoroutine(PlayRoutine(onComplete));
    }

    private IEnumerator PlayRoutine(System.Action onComplete)
    {
        // 초기 값 저장
        Vector2 originalPos = rectTransform.anchoredPosition;
        Vector3 originalScale = Vector3.one;

        //  플래시 
        overlay.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(whiteFlashDuration); 
        overlay.color = new Color(1, 1, 1, 0);

        // 스케일 업 
        float elapsed = 0f;
        while (elapsed < scaleUpDuration)
        {
            elapsed += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * scalingValue, elapsed / scaleUpDuration);
            yield return null;
        }
        rectTransform.localScale = originalScale * scalingValue;

        // 대기 시간
        yield return new WaitForSeconds(delayScaleUpDuratuon);

        // 아래 방향으로 빠르게 이동 
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 endPos = currentPos + new Vector2(0, -1200f); 
        elapsed = 0f;

        while (elapsed < fallingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallingDuration;
            // t * t를 사용하여 가속도 추가
            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, endPos, t * t);
            yield return null;
        }

        // 끝났음을 전달
        onComplete?.Invoke();

        // 오브젝트 상태 복구
        rectTransform.anchoredPosition = originalPos;
        rectTransform.localScale = originalScale;
    }
}