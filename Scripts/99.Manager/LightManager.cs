using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;


public class LightingManager : MonoBehaviour
{
    [Header("글로벌 라이트")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float duration;

    [Header("Act1")]
    [SerializeField] private Color act1Color;
    [SerializeField] private float act1Intensity = 1f;
    [Header("Act2")]
    [SerializeField] private Color act2Color;
    [SerializeField] private float act2Intensity = 1f;
    [Header("Act3")]
    [SerializeField] private Color act3Color;
    [SerializeField] private float act3Intensity = 1f;

    [Header("Phase1")]
    [SerializeField] private Color phase1Color;
    [SerializeField] private float phase1Intensity = 1f;
    [Header("Phase2")]
    [SerializeField] private Color phase2Color;
    [SerializeField] private float phase2Intensity = 1f;
    [Header("Phase3")]
    [SerializeField] private Color phase3Color;
    [SerializeField] private float phase3Intensity = 1f;




    private void Start()
    {
        GameManager.Instance.LightingManager = this;
    }
    public void ApplyActLighting(int act)
    {
        switch (act)
        {
            case 1:
                globalLight.color = act1Color;
                globalLight.intensity = act1Intensity;
                break;
            case 2:
                globalLight.color = act2Color;
                globalLight.intensity = act2Intensity;
                break;
            case 3:
                globalLight.color = act3Color;
                globalLight.intensity = act3Intensity;
                break;
        }
    }

    public void ApplyPhaseLighting(int phase)
    {
        switch (phase)
        {
            case 1:
                StartCoroutine(LerpLight(phase1Color, phase1Intensity, duration));
                break;
            case 2:
                StartCoroutine(LerpLight(phase2Color, phase2Intensity, duration));
                break;
            case 3:
                StartCoroutine(LerpLight(phase3Color, phase3Intensity, duration));
                break;
        }
    }


    private IEnumerator LerpLight(Color targetColor, float targetIntensity, float duration)
    {
        Color startColor = globalLight.color;
        float startIntensity = globalLight.intensity;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            globalLight.color = Color.Lerp(startColor, targetColor, t);
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            yield return null;
        }

        globalLight.color = targetColor;
        globalLight.intensity = targetIntensity;
    }
}