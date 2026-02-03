using NPOI.SS.Formula.Functions;
using System.Collections;
using UnityEngine;


public abstract class BaseAnomaly : MonoBehaviour
{
    protected SpriteRenderer targetSR;
    protected Animator targetAnim;
    protected Transform targetTransform;
    protected Coroutine scaleRoutine;
    protected Transform player;
    protected float triggerDistance = 10f;

    #region 프로퍼티
    public Coroutine ScaleRoutine => scaleRoutine;
    public Animator TargetAnim => targetAnim;
    public Transform TargetTransform => targetTransform;
    #endregion
    protected virtual void Start()
    {
        player = GameManager.Instance.Player.transform;
    }

    #region 타겟지정
    public virtual void SetTarget(GameObject target)
    {
        if (target == null)
            return;

        targetSR = target.GetComponent<SpriteRenderer>();
        targetAnim = target.GetComponent<Animator>();
        targetTransform = target.transform;
    }
    #endregion

    #region 애니메이터 관리
    public virtual void ChangeBlendTree(int act, int count)
    {
        if (targetAnim == null)
            return;

        int index = Random.Range(1, count);
        targetAnim.SetFloat("AniIndex", (float)index);
        int chosenAct = act;

        switch (act)
        {
            case 1:
                chosenAct = 1;
                break;
            case 2:
                chosenAct = Random.Range(1, 3);
                break;
            case 3:
                chosenAct = Random.Range(2, 4);
                break;
        }

        switch (chosenAct)
        {
            case 1:
                targetAnim.SetTrigger("Act1Trigger");
                Debug.Log($"Act {act} → Act1BlendTree 클립 {index} 실행");
                break;
            case 2:
                targetAnim.SetTrigger("Act2Trigger");
                Debug.Log($"Act {act} → Act2BlendTree 클립 {index} 실행");
                break;
            case 3:
                targetAnim.SetTrigger("Act3Trigger");
                Debug.Log($"Act {act} → Act3BlendTree 클립 {index} 실행");
                break;
        }
    }
    #endregion

    #region 스케일값 관리
    public virtual IEnumerator ScaleUp(float duration, float scaleSpeed)
    {
        if (targetTransform == null)
            yield break;

        while (Vector2.Distance(targetTransform.position, player.position) > triggerDistance)
        {
            if (GameManager.Instance.IsTutorial && GameManager.Instance.PhaseManager.CurrentPhase == 4)
                break;
            yield return null;
        }
       

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            targetTransform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
            yield return null;
        }
        scaleRoutine = null;
    }
    #endregion

}
