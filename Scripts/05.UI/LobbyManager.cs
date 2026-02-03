using UnityEngine;
using UnityEngine.SceneManagement;
using WhereAreYouLookinAt.Enum;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public int SelectedCharacterId { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 캐릭터 선택
    public void SelectCharacter(int characterId)
    {
        SelectedCharacterId = characterId;
        RunConfig.SelectedCharacterId = characterId;

        Debug.Log($"[Lobby] Selected Character ID : {characterId}");
    }

    // 게임 시작
    public void StartGame()
    {
        if (SelectedCharacterId < 0)
        {
            Debug.LogWarning("[Lobby] 캐릭터가 선택되지 않았습니다.");
            return;
        }

        SceneController.Instance.LoadScene(2); // BattleScene
    }

    public void BackStartScene()
    {
        // 시작 씬으로 이동
        SceneController.Instance.LoadScene(0);
    }
}
