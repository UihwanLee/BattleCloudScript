using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DayTransitionUI : MonoBehaviour
{
    [SerializeField] private Image blackPanel;
    [SerializeField] private TextMeshProUGUI dayText;

    [SerializeField] private float panelFadeInDuration = 1.0f;
    [SerializeField] private float textFadeInDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 1.0f;

    float t = 0f;

    private void Start()
    {
        blackPanel.gameObject.SetActive(false);
        dayText.gameObject.SetActive(false);
    }


    public IEnumerator FadeInPanel()
    {
        blackPanel.gameObject.SetActive(true);
        dayText.gameObject.SetActive(true);

        blackPanel.color = new Color(0f, 0f, 0f, 0f);
        dayText.color = new Color(1f, 1f, 1f, 0f);
        dayText.text = "Day " + (GameManager.Instance.DayManager.CurrentDay+1);

        float t = 0f;
        while (t < panelFadeInDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / panelFadeInDuration);
            blackPanel.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }
    public IEnumerator FadeInText(bool Correct)
    {
        t = 0f;
        bool isCorrect = Correct;
        Vector3 originalPos = dayText.rectTransform.localPosition;
        while (t < textFadeInDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / textFadeInDuration);
            dayText.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (isCorrect)//정답
        {
            t = 0f;
            while (t < textFadeInDuration)
            {
                t += Time.deltaTime;
                dayText.color = Color.Lerp(Color.white, Color.green, t / textFadeInDuration);
                yield return null;
            }
        }
        else//오답
        {
            float magnitude = 5f;
            t = 0f;
            while (t < textFadeInDuration)
            {
                t += Time.deltaTime;
                dayText.color = Color.Lerp(Color.white, Color.red, t / textFadeInDuration);
                float offsetX = Random.Range(-magnitude, magnitude);
                float offsetY = Random.Range(-magnitude, magnitude);
                dayText.rectTransform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

                yield return null;
            }

            dayText.rectTransform.localPosition = originalPos;
        }
    }
    public IEnumerator TextAndFadeOut()
    {
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            blackPanel.color = new Color(0f, 0f, 0f, a);
            dayText.color = new Color(1f, 1f, 1f, a);
            yield return null;
        }

        blackPanel.color = new Color(0f, 0f, 0f, 0f);
        dayText.color = new Color(1f, 1f, 1f, 0f);
        
        blackPanel.gameObject.SetActive(false);
        dayText.gameObject.SetActive(false);
    }
}
