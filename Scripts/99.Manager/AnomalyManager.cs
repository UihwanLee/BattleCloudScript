using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    private List<Anomaly> anomalys = new List<Anomaly>();

    [Header("이상현상 발생 확률 (0~1)")]
    [SerializeField] private float anomalyChance;

    [Header("스케일커지는 이상현상 값")]
    [SerializeField] private float scaleDuration;
    [SerializeField] private float scaleSpeed;

    private Anomaly currentAnomaly;

    public bool isAnomaly { get; private set; } = false;

    private void Awake()
    {
        anomalys.Clear();
        anomalys.AddRange(FindObjectsOfType<Anomaly>());
    }
    private void Start()
    {
        GameManager.Instance.AnomalyManager = this;
    }

    #region 실행
    [ContextMenu("Check Anomaly By Day")]
    public void CheckAnomalyByDay()
    {
        if (GameManager.Instance.IsTutorial)
        {
            isAnomaly = true;
            RandomAnomaly();
            return;
        }

        ResetAnomaly();

        if (!isAnomaly)
        {
            isAnomaly = Random.value < anomalyChance;

            if (isAnomaly)
                RandomAnomaly();
        }
    }


    private void RandomAnomaly()
    {
        if (anomalys.Count == 0)
            return;
        Debug.Log("이상현상 실행");
        int index = Random.Range(0, anomalys.Count);
        currentAnomaly = anomalys[index];
        anomalys[index].StartAnomaly(scaleDuration, scaleSpeed);
    }

    public Anomaly GetCurrentAnomaly()
    {
        return currentAnomaly;
    }

    public Transform GetCurrentAnomalyTarget()
    {
        if (currentAnomaly == null) return null;
        return currentAnomaly.TargetTransform != null
            ? currentAnomaly.TargetTransform
            : currentAnomaly.transform;
    }

    #endregion

    #region 초기화
    public void ResetAnomaly()
    {
        foreach (var anomaly in anomalys)
        {
            anomaly.ResetAnomaly();
        }
        currentAnomaly = null;
        isAnomaly = false;
    }
    #endregion
}