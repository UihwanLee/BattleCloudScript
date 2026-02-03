using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 버튼 다양한 상호작용 이벤트
public class ButtonEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("상호작용 버튼 이미지")]
    [SerializeField] private Image btn_image;
    [Header("상호작용 버튼 이벤트")]
    [SerializeField] private Button btn_button;

    [Header("상호작용 설정")]
    [Header("스케일링")]
    [SerializeField] private float scale = 1.2f;
    [Header("클릭 이벤트 설정 할 것인가?")]
    [SerializeField] private bool isClickEvent = true;

    [Header("클릭 설정 값")]
    [SerializeField] private float pressScale = 0.85f;
    [SerializeField] private float pressDuration = 0.06f;
    [SerializeField] private float releaseDuration = 0.12f;
    [SerializeField] private float overshootScale = 1.05f;

    [Header("애니메이션 설정 할 것인가?")]
    [SerializeField] private bool isAnimationEvent = true;

    [Header("버튼 클릭 애니메이션 후에 버튼 작동?")]
    [SerializeField] private bool isOnButtonAtferClickAnimation = true;

    private Vector3 originPosition;

    private Coroutine clickCoroutine;

    private void Reset()
    {
        btn_image = GetComponent<Image>();
        btn_button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 들어오면 이미지 키우기
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiButtonHover, WhereAreYouLookinAt.Enum.SFXPlayType.Single);
        btn_image.transform.localScale = new Vector3(scale, scale, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 나가면 원복하기
        btn_image.transform.localScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isClickEvent == false) return;
        // 클릭사운드
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiButtonClick, WhereAreYouLookinAt.Enum.SFXPlayType.Multi);
        if (isAnimationEvent)
        {
            originPosition = transform.localPosition;

            if (isOnButtonAtferClickAnimation == false) btn_button.onClick.Invoke();

            if (!gameObject.activeInHierarchy)
                return;

            if (clickCoroutine != null) StopCoroutine(clickCoroutine);

            clickCoroutine = StartCoroutine(ClickCoroutine());
        }
        else
        {
            btn_button.onClick.Invoke();
        }
    }

    private IEnumerator ClickCoroutine()
    {
        Vector3 origin = Vector3.one;

        float time = 0f;
        while (time < pressDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / pressDuration;


            t = t * t;

            btn_image.transform.localScale =
                Vector3.Lerp(origin, Vector3.one * pressScale, t);

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.03f);

        time = 0f;
        while (time < releaseDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / releaseDuration;

            // Ease Out
            t = 1f - Mathf.Pow(1f - t, 2f);

            float scale = Mathf.Lerp(pressScale, overshootScale, t);
            btn_image.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        // 최종 원복
        btn_image.transform.localScale = Vector3.one;

        if(isOnButtonAtferClickAnimation) btn_button.onClick.Invoke();
    }
}
