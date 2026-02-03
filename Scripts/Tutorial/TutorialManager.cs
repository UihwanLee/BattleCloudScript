using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class TutorialStep
{
    [Header("타겟 오브젝트 (Static일 때만 사용)")]
    public Transform target;

    [Header("이 스텝이 아노말리인지 여부")]
    public bool useAnomalyTarget = false;

    [Header("이 스텝에서 적용할 GameManager 불값들")]
    public bool tutorialAltar = true;
    public bool tutorialInfo = true;
    public bool tutorialAnomaly = true;
    public bool tutorialHouse = true;
    public LocalizedString message;
    public float holdDuration = 4f;
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;   // 방향표 프리팹
    [SerializeField] private TutorialUI tutorialUI;    // TMP UI 관리 스크립트
    [SerializeField] private List<TutorialStep> steps; // 튜토리얼 단계 리스트

    private GameObject currentArrow;

    private void Start()
    {
        if (GameManager.Instance.IsTutorial)
            StartCoroutine(RunTutorial());
    }


    private IEnumerator RunTutorial()
    {
        foreach (var step in steps)
        {
            Transform stepTarget = step.useAnomalyTarget
                ? GameManager.Instance.AnomalyManager.GetCurrentAnomalyTarget()
                : step.target;

            if (stepTarget == null && step.useAnomalyTarget || step.useAnomalyTarget)
            {
                // target이 생길 때까지 대기
                yield return new WaitUntil(() =>
                    GameManager.Instance.AnomalyManager.GetCurrentAnomalyTarget() != null
                );

                yield return new WaitUntil(() =>
                    GameManager.Instance.PhaseManager != null &&
                    GameManager.Instance.PhaseManager.CurrentPhase == 4 &&
                    GameManager.Instance.AnomalyManager.GetCurrentAnomalyTarget() != null
                );
                // target 갱신
                stepTarget = GameManager.Instance.AnomalyManager.GetCurrentAnomalyTarget();
            }


            currentArrow = Instantiate(arrowPrefab, GameManager.Instance.Player.transform.position, Quaternion.identity);
            var arrowCtrl = currentArrow.GetComponent<ArrowController>();
            arrowCtrl.SetTarget(stepTarget);

            if (step.useAnomalyTarget)
            {
                // 이상현상일 때만 4페이즈 체크
                yield return new WaitUntil(() =>
                Vector3.Distance(GameManager.Instance.Player.transform.position, stepTarget.position) < 10f
                    && (GameManager.Instance.PhaseManager != null && GameManager.Instance.PhaseManager.CurrentPhase == 4)

                );
            }
            else
            {
                // 일반 스텝은 거리 조건만
                yield return new WaitUntil(() =>
                    Vector3.Distance(GameManager.Instance.Player.transform.position, stepTarget.position) < 3f
                );
            }


            Destroy(currentArrow);

            tutorialUI.ShowMessage(step.message.GetLocalizedString());
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            yield return StartCoroutine(tutorialUI.FadeOut());

            GameManager.Instance.TutorialInfo = step.tutorialInfo;
            GameManager.Instance.TutorialAltar = step.tutorialAltar;
            GameManager.Instance.TutorialAnomaly = step.tutorialAnomaly;
            GameManager.Instance.TutorialHouse = step.tutorialHouse;
        }

        OnTutorialComplete();
    }



    private void OnTutorialComplete()
    {
        GameManager.Instance.TutorialIsDone();
        Destroy(this.gameObject);
    }
}
