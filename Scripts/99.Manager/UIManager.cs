using UnityEngine;
using UnityEngine.UI;
using WhereAreYouLookinAt.Enum;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private HUDTimer timerUI;

    [Header("Management UI")]
    [SerializeField] private ManagementUI managementCanvas;

    [Header("GameOver UI")]
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Shop UI")]
    [SerializeField] private Button shopNPC;
    [SerializeField] private GameObject shopWindow;

    [Header("HomeCanvas")]
    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject playerSlotUI;
    [SerializeField] private GameObject ShopUI;
    [SerializeField] private GameObject detailViewUI;

    [Header("Vignette UI")]
    [SerializeField] private GameObject vignetteUI;

    #region 프로퍼티
    public ManagementUI ManagementUI => managementCanvas;
    public GameOverUI GameOverUI => gameOverUI;
    #endregion


    private void Reset()
    {
        hudCanvas = GameObject.Find("HUD Canvas");
        timerUI = hudCanvas.transform.FindChild<HUDTimer>("Time Group");
        managementCanvas = FindFirstObjectByType<ManagementUI>();
        gameOverUI = FindFirstObjectByType<GameOverUI>();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        EnterGamePlay();
        SetEnableTimer(false);

        OnWaveStart();
    }

    public void OnWaveStart()
    {
        ShopUI.SetActive(false);
        detailViewUI.SetActive(false);
        playerSlotUI.SetActive(false);
        homeCanvas.SetActive(false);
        vignetteUI.SetActive(true);
    }


    public void OnWaveDone()
    {
        ShopUI.SetActive(false);
        detailViewUI.SetActive(false);
        playerSlotUI.SetActive(true);
        homeCanvas.SetActive(true);
        vignetteUI.SetActive(false);
    }

    public void EnterGamePlay()
    {
        hudCanvas.SetActive(true);
        managementCanvas.gameObject.SetActive(false);
        GameManager.Instance.State = GameState.RUNNING;
    }

    public void EnterManagement()
    {
        hudCanvas.SetActive(false);
        managementCanvas.gameObject.SetActive(true);
        GameManager.Instance.State = GameState.PAUSE;
    }

    public void ShowGameOverUI()
    {
        gameOverUI.gameObject.SetActive(true);

        // GameOver 결과 슬롯 갱신
        PlayerSlotDetailUI detailUI =
            gameOverUI.GetComponent<PlayerSlotDetailUI>();

        if (detailUI != null)
            detailUI.RefreshAll();
    }

    public void SetEnableTimer(bool enabled)
    {
        timerUI.gameObject.SetActive(enabled);
    }
}
