using UnityEngine;
using TMPro;
using UnityEngine.UI;

using System.Collections;


public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject uiobj;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private float fadeDuration = 1.5f;

    private Image[] images;
    private TMP_Text[] texts;


    private void Awake()
    {
        // 하위 오브젝트에서 Image, TMP_Text 전부 가져오기
        images = uiobj.GetComponentsInChildren<Image>(true);
        texts = uiobj.GetComponentsInChildren<TMP_Text>(true);

        uiobj.SetActive(false);
    }


    public void ShowMessage(string message)
    {
        tutorialText.text = message;
        StartCoroutine(FadeIn());
    }
    public void HideMessage()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        uiobj.SetActive(true);
        GameManager.Instance.Player.Controller.SetMovementEnabled(false);
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);

            foreach (var img in images)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }

            foreach (var txt in texts)
            {
                Color c = txt.color;
                c.a = alpha;
                txt.color = c;
            }
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        GameManager.Instance.Player.Controller.SetMovementEnabled(true);
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            foreach (var img in images)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }

            foreach (var txt in texts)
            {
                Color c = txt.color;
                c.a = alpha;
                txt.color = c;
            }

            yield return null;
        }
        uiobj.SetActive(false);
    }
}
