using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhereAreYouLookinAt.Enum;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    public int SelectedCharacterId { get; private set; } = Define.PLATER_BASE_ID;

    private Player player;
    public bool IsEndingPhase { get; private set; } = false;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SelectedCharacterId = RunConfig.SelectedCharacterId;
        player = FindFirstObjectByType<Player>();
        OptionUI = FindAnyObjectByType<OptionUI>();
        SetSelectedCharacter(SelectedCharacterId, player);

        // 배경음악 변경
        //AudioManager.Instance.ChangeBackGroundMusic(WhereAreYouLookinAt.Enum.BGMType.Main);

        // 타임 스케일 걸려있을 경우에 풀기
        Time.timeScale = 1f;

        // MainScene 시작시 RUNNING 상태로 초기화
        State = GameState.RUNNING;

        // SaveData 로드
        SaveData = SaveManager.Instance.Load();

        if(SaveData != null)
        {
            // 정보 갱신
            IsTutorial = SaveData.gameStateData.IsTutorial;
        }
    }
    public void SetSelectedCharacter(int characterId, Player player)
    {
        PlayerTraitData data = DataManager.Get<PlayerTraitData>(characterId);
        SelectedCharacterId = characterId;
        GameObject prefab = DataManager.GetPrefab(data.PREFAB);
        GameObject playerPrefab = Instantiate(prefab, transform.GetChild(0).transform);
        player.Prefab = playerPrefab;
        player.Prefab.SetActive(false);
    }
    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    private GameManager() { }

    #region 프로퍼티

    public GameState State { get; set; }
    public Player Player { get { return player; } }
    public SaveData SaveData { get; set; }
    public bool IsStart { get; private set; } = false;
    public bool IsTutorial { get; set; } = false;
    public bool TutorialInfo { get; set; } = true;
    public bool TutorialAnomaly { get; set; } = true;
    public bool TutorialAltar { get; set; } = true;
    public bool TutorialHouse { get; set; } = true;
    public AnomalyManager AnomalyManager { get; set; }
    public DaysManager DayManager { get; set; }
    public PhaseManager PhaseManager { get; set; }
    public LightingManager LightingManager { get; set; }
    public LevelUpRewardUI LevelUpRewardUI { get; set; }
    public GameRule GameRule { get; set; }
    public ItemDataTable ItemDataTable { get; set; }
    public WeaponDataTable WeaponDataTable { get; set; }
    public SpawnManager spawnManager { get; set; }
    public ReinforcementNPC ReinforcementNPC { get; set; }
    public ParticleManager ParticleManager { get; set; }
    public TemporaryInventory TemporaryInventory { get; set; }
    public DetailViewUI DetailViewUI { get; set; }
    public ShopManager ShopManager { get; set; }
    public LogInfoUI LogInfoUI { get; set; }
    public DPSManager DPSManager { get; set; }
    public TutorialInfoButton TutorialInfoButton {  get; set; }
    public OptionUI OptionUI { get; set; }

    #endregion

    #region 게임루프
    #region 삭제예정
    public void TutorialIsDone()
    {
        IsTutorial = false;
    }
    public void StartPhase()
    {
        IsEndingPhase = false;
        IsStart = true;

        PhaseManager.StartPhase();
        Debug.Log("전투 페이즈 시작!");

    }

    public void EndPhase()
    {
        IsStart = false;
        IsEndingPhase = true;


        if (spawnManager != null)
        {
            spawnManager.StopSpawn();
            spawnManager.ClearAllWarnings();
            spawnManager.ClearAllMonsters();
        }

        OldMonsterProjectile.ReleaseAll();
    }
    #endregion

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndGame()
    {
        Time.timeScale = 0f;
        UIManager.Instance.ShowGameOverUI();
    }
    #endregion

    #region 게임 클리어

    public void CheckClearGame()
    {
        if(DayManager.CurrentDay >= DayManager.MaxDay)
        {
            EventBus.OnClearGame?.Invoke();
        }
    }

    #endregion
}