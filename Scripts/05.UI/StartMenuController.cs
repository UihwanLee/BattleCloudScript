using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    [Header("판넬")]
    [SerializeField] private GameObject settingPanel;

    public void OnClickGameStart()
    {
        // StartScene → LobbyScene
        //SceneController.Instance.LoadScene(1);

        SceneController.Instance.LoadScene(1);
    }

    public void OnClickSetting()
    {
        SceneController.Instance.transform.GetComponentInChildren<OptionUI>().OpenFromLobby();
    }

    public void OnClickCloseSetting()
    {
        if (settingPanel != null)
            settingPanel.SetActive(false);
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}