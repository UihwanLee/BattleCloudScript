using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneController : MonoBehaviour
{
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    private int currentScene;
    private bool isLoading;

    private static SceneController instance;

    public static SceneController Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindFirstObjectByType<SceneController>();
            }
            return instance;
        }
    }

    private void Reset()
    {
        loadingUI = GameObject.Find("LoadingUIWindow");
        loadingBar = GameObject.Find("LoadingBar").GetComponent<Slider>();
        loadingText = transform.FindChild<TextMeshProUGUI>("LoadingPercent");
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            // 씬 로드 이벤트를 구독하여 모든 씬 로드 후 처리를 여기서 담당
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        currentScene = 0;
    }

    private SceneController() { }

    private SaveData saveData;

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(int index)
    {
        saveData = null;
        if(currentScene != 0)
        {
            if (isLoading) return;
            loadingUI.SetActive(true);
            AsyncOperation operation = SceneManager.LoadSceneAsync(index);
            StartCoroutine(LoadAsynchronously(operation));
        }
        else
        {
            SceneManager.LoadScene(index);
        }

        currentScene = index;
    }

    public void LoadSceneWithSaveData(int index, SaveData _saveData)
    {
        saveData = _saveData;
        if (loadingUI != null) loadingUI.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        StartCoroutine(LoadAsynchronously(operation));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드가 끝날 시 실행
        if(saveData != null)
        {
            StartCoroutine(WaitAndInit(scene));
        }
    }

    private IEnumerator WaitAndInit(Scene scene)
    {
        // BattleScene이 아니면 아무것도 하지 않음
        if (scene.name != "BattleScene")
            yield break;

        yield return null;

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
            yield break;

        Player player = playerObj.GetComponent<Player>();
        if (player == null)
            yield break;

        Player p = player.GetComponent<Player>();
        if (GameManager.Instance.Player == null)
            GameManager.Instance.SetPlayer(p);

        yield return null;

        ApplySaveData();

        if (loadingUI != null)
            loadingUI.SetActive(false);
    }

    private void ApplySaveData()
    {
        if (saveData == null) return;
        if (GameManager.Instance.Player == null) return;

        PlayerSlotUI playerSlotUI = GameManager.Instance.Player.PlayerSlotUI;
        if (playerSlotUI == null) return;

        playerSlotUI.Load(saveData.playerSlotData, saveData.weaponSlots);
    }

    private IEnumerator LoadAsynchronously(AsyncOperation operation)
    {
        isLoading = true;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            float percentage = progress * 100f;

            if (loadingBar != null)
                loadingBar.value = progress;

            if (loadingText != null)
                loadingText.text = percentage.ToString("0") + "%";

            yield return null;
        }

        loadingUI.SetActive(false);
        isLoading = false;
    }

    #region 프로퍼티
    
    public int CurrentScene {  get { return currentScene; } }
    public OptionUI OptionUI => optionUI;

    #endregion
}
