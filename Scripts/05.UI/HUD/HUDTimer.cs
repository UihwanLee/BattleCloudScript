using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private Image phaseIcon;

    [Header("Phase per Sprite")]
    [SerializeField] private Sprite phase1Sprite;
    [SerializeField] private Sprite phase2Sprite;
    [SerializeField] private Sprite phase3Sprite;
    [SerializeField] private Sprite phase4Sprite;

    private void Reset()
    {
        timer = transform.FindChild<TextMeshProUGUI>("TimeText");
        phaseIcon = transform.FindChild<Image>("Phase Icon");
    }

    private void OnEnable()
    {
        EventBus.OnTimerChanged += UpdateUI;
        ResetUI();
    }

    private void OnDisable()
    {
        EventBus.OnTimerChanged -= UpdateUI;
    }

    private void UpdateUI(float time, int phase)
    {
        Sprite sprite = null;

        switch (phase)
        {
            case 0:
                timer.text = "00";
                break;
            case 1:
                sprite = phase1Sprite;
                timer.text = time.ToString("00");
                break;
            case 2:
                sprite = phase2Sprite;
                timer.text = time.ToString("00");
                break;
            case 3:
                sprite = phase3Sprite;
                timer.text = time.ToString("00");
                break;
            case 4:
                sprite = phase4Sprite;
                timer.text = time.ToString("00");
                break;
        }

        if (sprite != null)
            phaseIcon.sprite = sprite;
    }

    private void ResetUI()
    {
        phaseIcon.sprite = phase1Sprite;
        timer.text = "00";
    }
}
