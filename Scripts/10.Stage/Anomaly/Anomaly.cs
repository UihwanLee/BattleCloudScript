using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Anomaly : BaseAnomaly
{
    private Dictionary<Transform, Vector3> initialScales = new Dictionary<Transform, Vector3>();
    private List<GameObject> anomalyobj = new List<GameObject>();

    [Header("애니메이션 클립갯수")]
    [SerializeField] private int act1AniCount;
    [SerializeField] private int act2AniCount;
    [SerializeField] private int act3AniCount;

    private bool resetAni=false;

    protected override void Start()
    {
        base.Start();
        InitAnomaly();
    }

    #region 자식 오브젝트 불러오기
    private void InitAnomaly()
    {
        anomalyobj.Clear();
        foreach (Transform child in transform)
        {
            anomalyobj.Add(child.gameObject);
        }
        foreach (var obj in anomalyobj)
        {
            initialScales[obj.transform] = obj.transform.localScale;
        }
    }
    #endregion

    #region 실행
    public void StartAnomaly(float duration, float speed)
    {
        if (anomalyobj.Count == 0)
            return;
        var target = anomalyobj[Random.Range(0, anomalyobj.Count)];
        SetTarget(target);


        int act = GameManager.Instance.DayManager.CurrentAct;
        int maxValue = 3; // 기본 Act1

        if (act == 2) maxValue = 4; // Act2 → 0~3
        else if (act == 3) maxValue = 6; // Act3 → 0~5

        int value = Random.Range(0, maxValue);
        int count = GetAniCountByAct(act);

        while ((act == 3 && value == 2) || (value == 0 && count == 0))
        {
            value = Random.Range(0, maxValue);
        }


        switch (value)
        {
            case 0:
                Debug.Log(target.name + "이미지변경");
                ChangeBlendTree(act, count);
                resetAni = true;
                break;
            case 1:
                Debug.Log(target.name + "스케일 커짐");
                ObjScaleUp(act, duration, speed);
                break;
            case 2:
                Debug.Log(target.name + " Act1 → 투명도 낮추기");
                if (targetSR != null)
                {
                    Color c = targetSR.color;
                    c.a = 0.3f;
                    targetSR.color = c;
                }
                break;
            case 3:
                Debug.Log(target.name + " Act2/3 → 애니메이션 빠르게 실행");
                if (targetAnim != null)
                {
                    targetAnim.speed = 1.5f;
                }
                break;

            case 4:
                Debug.Log(target.name + " Act3 → 오브젝트 비활성화");
                target.SetActive(false);
                break;
            case 5:
                Debug.Log(target.name + " Act3 → 애니메이션 느리게 실행");
                if (targetAnim != null)
                {
                    targetAnim.speed = 0.5f;
                }
                break;
        }
    }
    #endregion

    #region 이상현상

    private int GetAniCountByAct(int act)
    {
        switch (act)
        {
            case 1: return act1AniCount;
            case 2: return act2AniCount;
            case 3: return act3AniCount;
            default: return 0;
        }
    }

    private void ObjScaleUp(int act, float duration, float speed)
    {
        switch (act)
        {
            case 1:
                scaleRoutine = StartCoroutine(ScaleUp(duration * 1.5f, speed * 1.5f)); //액트에 맞는 스케일값 넣기
                break;
            case 2:
                scaleRoutine = StartCoroutine(ScaleUp(duration, speed));
                break;
            case 3:
                scaleRoutine = StartCoroutine(ScaleUp(duration * 0.5f, speed * 0.5f));
                break;
        }
    }


    #endregion

    #region 초기화
    public void ResetAnomaly()
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
            scaleRoutine = null;
        }

        foreach (var obj in anomalyobj)
        {
            Transform t = obj.transform;

            if (initialScales.ContainsKey(t))
                t.localScale = initialScales[t];
            
            if (resetAni && targetAnim != null)
            {
                targetAnim.SetTrigger("IsDone");
                targetAnim.SetFloat("AniIndex", 0f);
            }

            Animator anim = obj.GetComponent<Animator>();
            if (anim != null)
            {
                anim.speed = 1f;
                anim.Play("BaseAni", 0, 0f); // 원하는 기본 상태 이름
            }
                

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }

            if (!obj.activeSelf)
                obj.SetActive(true);

            
        }
        
        resetAni = false;
        targetSR = null;
        targetAnim = null;
        targetTransform = null;
    }
    #endregion
}