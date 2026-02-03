using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class NextWave : MonoBehaviour
{
    [Header("플레이어 이동 위치")]
    [SerializeField] private Transform nextWaveSpawnPoint;

    [Header("페이드 설정")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        
    }

    private IEnumerator Start()
    {
        yield return null;

        GameManager gameManager = GameManager.Instance;
        Player player = gameManager.Player;

        player.Controller.Teleportation(nextWaveSpawnPoint.position);
        fadePanel.gameObject.SetActive(false);

        // 몬스터 스폰 제어
        gameManager.State = WhereAreYouLookinAt.Enum.GameState.PAUSE;

        // 플레이어 움직임 제어
        player.Controller.SetMovementEnabled(false);

        gameManager.DayManager.NextDay();

        yield return new WaitForSeconds(1f);

        // 플레이어 등장 연출
        EventBus.SpawnPlayer?.Invoke();

        // 플레이어 모습 들어내기
        player.Prefab.SetActive(true);

        player.Trait.SetSpriteRenderer();

        yield return new WaitForSeconds(0.5f);

        // 조언자 장착
        player.Trait.InitPlayerWeapon();

        yield return new WaitForSeconds(1f);

        // 몬스터 스폰 제어
        gameManager.State = WhereAreYouLookinAt.Enum.GameState.RUNNING;

        gameManager.PhaseManager.StartPhase();
    }


    public void GoToNextWave()
    {
        StartCoroutine(GoNextWaveRoutine());
    }


    #region 코루틴
    private IEnumerator GoNextWaveRoutine()
    {
        yield return StartCoroutine(FadeIn()); // 페이드 인

        UIManager.Instance.OnWaveStart();       // Home Canvas 비활성화

        GameManager.Instance.Player.Controller.Teleportation(nextWaveSpawnPoint.position); // 플레이어 이동(위치초기화)

        CameraManager.Instance.SetCenter(0.5f, 0.5f);

        GameManager.Instance.DayManager.NextDay(); // 다음날 변경

        GameManager.Instance.spawnManager.ResetCurrentSpawnCount(); // 몬스터 스폰 카운트 리셋

        yield return StartCoroutine(FadeOut()); // 페이드 아웃

        GameManager.Instance.DPSManager.UpdateUI();

        GameManager.Instance.PhaseManager.StartPhase(); // 전투 시작
    }

    private IEnumerator FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        float t = 0f;
        Color c = fadePanel.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 1f;
        fadePanel.color = c;
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = fadePanel.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
    }
    #endregion
}
