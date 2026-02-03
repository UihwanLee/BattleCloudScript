using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveNumTxt;

    private void OnEnable()
    {
        waveNumTxt.text = GameManager.Instance.DayManager.CurrentDay.ToString("D2");
    }

    // 로비로
    public void OnClickGoLobby()
    {
        SceneController.Instance.LoadScene(1);
    }

    // 다시하기
    public void OnClickRetry()
    {
        SceneController.Instance.LoadScene(2);
    }

    // 메인
    public void OnClickGoMain()
    {
        SceneController.Instance.LoadScene(0);
    }
}
