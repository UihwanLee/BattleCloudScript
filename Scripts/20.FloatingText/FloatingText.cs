using System.Collections;
using TMPro;
using UnityEngine;

// 데미지, 경험치 등 UI Text 표기하는 클래스
// 데미지 표기는 Canvas 설정 -> WorldSpace로 설정해야함.
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI floatingText;

    private Coroutine fadeCoroutine;
    private WaitForSeconds waitForSeconds;
    private float fadeDuration;
    private float stopTime = 0.3f;

    private FloatingTextPoolManager floatingTextPoolManager;

    private void Reset()
    {
        floatingText = GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        waitForSeconds = new WaitForSeconds(0.1f);

        Initialize();
    }

    private void Start()
    {
        floatingTextPoolManager = FloatingTextPoolManager.Instance;
    }

    public void Initialize()
    {
        // 색상 초기화
        Color color = floatingText.color;
        color.a = 1.0f;
        floatingText.color = color;

        // Text 초기화
        floatingText.text = string.Empty;
    }

    public void SetText(string text)
    {
        this.floatingText.text = text;
    }

    public void SetColor(Color color)
    {
        this.floatingText.color = color;
    }

    public void SetPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public void SetDuration(float duration)
    {
        fadeDuration = duration;
        waitForSeconds = new WaitForSeconds(fadeDuration);
    }

    public void StartFadeCoroutine(FloatingTextSO data)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCoroutine(data));
    }

    public IEnumerator FadeCoroutine(FloatingTextSO data)
    {
        float fadeTime = fadeDuration;
        float curTime = 0.0f;
        Color tempColor = floatingText.color;
        Vector3 targetPosition = Vector3.zero;
        float alpha = 1f;
        tempColor.a = alpha;
        floatingText.color = tempColor;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * data.FloatingDist;

        yield return new WaitForSeconds(stopTime);

        while (curTime < fadeTime)
        {
            curTime += Time.deltaTime;

            float t = (curTime / fadeTime);

            // alpha 값 수정
            alpha = 1f - t;
            tempColor.a = alpha;
            floatingText.color = tempColor;

            // 위치 값 수정
            transform.position = Vector3.Lerp(startPos, endPos, t);
            //targetPosition = new Vector3(transform.position.x, transform.position.y + data.FloaingDist, transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, targetPosition, t);
            

            yield return null;
        }

        tempColor.a = 0;
        floatingText.color = tempColor;
        transform.position = targetPosition;

        // 반환
        floatingTextPoolManager.Release(data.Type.ToString(), this.gameObject);
    }
}
