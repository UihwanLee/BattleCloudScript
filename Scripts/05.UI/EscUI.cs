using UnityEngine;
using UnityEngine.SceneManagement;

public class EscUI : MonoBehaviour
{
    [SerializeField] private GameObject escPanel;
    private bool isOpen = false;
    private bool isLocked = false;
    public bool IsOpen => isOpen;
    public bool IsLocked => isLocked;
    private void Awake()
    {
        escPanel.SetActive(false);
    }
    private void Start()
    {
        EscInputController input = FindObjectOfType<EscInputController>();
        if (input != null)
        {
            input.SetEscUI(this);
        }
    }
    public void Open()
    {
        isOpen = true;
        Time.timeScale = 0f;
        escPanel.SetActive(true);

        // 슬롯 연동
        PlayerSlotDetailUI detailUI = GetComponent<PlayerSlotDetailUI>();

        if (detailUI != null)
            detailUI.RefreshAll();
        else
            Debug.LogError("PlayerSlotDetailUI가 EscUI에 없음");
    }

    public void Close()
    {
        isOpen = false;
        Time.timeScale = 1f;
        escPanel.SetActive(false);
    }
    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }
    // 다시하기 (현재 스테이지 재시작)
    public void RestartStage()
    {
        Time.timeScale = 1f;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void GoLobby()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadScene(1);
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadScene(0); // MainScene
    }

    public void OnClickOption()
    {
        Lock();
        SceneController.Instance.OptionUI.OpenFromBattle();
    }

    public void OpenWithoutPauseChange()
    {
        isOpen = true;
        escPanel.SetActive(true);
    }
}