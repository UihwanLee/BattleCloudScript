using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClearUI : MonoBehaviour
{
    [SerializeField] private GameObject clearUI;
    [SerializeField] private TextMeshProUGUI waveNumTxt;

    private void OnEnable()
    {
        EventBus.OnClearGame += OnGameClear;
    }

    private void OnDisable()
    {
        EventBus.OnClearGame -= OnGameClear;
    }

    public void OnGameClear()
    {
        Time.timeScale = 0f;
        clearUI.SetActive(true);

        waveNumTxt.text = GameManager.Instance.DayManager.CurrentDay.ToString("D2");

        // 슬롯 연동
        PlayerSlotDetailUI detailUI =
            GetComponent<PlayerSlotDetailUI>();

        if (detailUI != null)
            detailUI.RefreshAll();
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadScene(2);
    }

    public void GoLobby()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadScene(1);
    }
    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadScene(0);
    }
}
