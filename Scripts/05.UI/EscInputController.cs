using UnityEngine;

public class EscInputController : MonoBehaviour
{
    private OptionUI OptionUI => SceneController.Instance.OptionUI;
    private EscUI escUI;
    public void SetEscUI(EscUI ui)
    {
        escUI = ui;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        int scene = SceneController.Instance.CurrentScene;

        // BattleScene
        if (scene == 2)
        {
            // 1순위: OptionUI 닫기
            if (OptionUI != null && OptionUI.IsWindowOpen())
            {
                OptionUI.CloseCurrentPanel();
                return;
            }

            // 2순위: EscUI
            HandleEscUI();
        }
        else
        {
            HandleOptionUI();
        }
    }

    private void HandleOptionUI()
    {
        var optionUI = OptionUI;
        if (optionUI == null)
            return;

        if (optionUI.IsWindowOpen())
        optionUI.CloseCurrentPanel();
        else
            optionUI.OpenFromLobby();
    }

    private void HandleEscUI()
    {
        if (escUI == null)
            return;
        if (escUI.IsLocked)
            return;

        if (escUI.IsOpen)
            escUI.Close();
        else
            escUI.Open();
    }
}
