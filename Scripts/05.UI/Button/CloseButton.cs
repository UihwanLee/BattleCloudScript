using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    private void Reset()
    {
        closeButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        closeButton.onClick.AddListener(OnClickClose);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(OnClickClose);
    }

    public void OnClickClose()
    {
        UIManager.Instance.EnterGamePlay();
        AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.UiPopUp, WhereAreYouLookinAt.Enum.SFXPlayType.Single);
    }
}
