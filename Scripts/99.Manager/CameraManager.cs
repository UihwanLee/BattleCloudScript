using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Virtual Camera")]
    [SerializeField] private CinemachineVirtualCamera currentCamera;

    [Header("카메라 세팅")]
    [SerializeField] private CinemachineVirtualCamera camera1;
    [SerializeField] private CinemachineVirtualCamera camera2;

    public CinemachineVirtualCamera CurrentCamera => currentCamera;

    [Header("Priority")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;

    private CinemachineFramingTransposer framing;

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

        framing = camera1.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        // 카메라 설정
        if (camera1 != null && camera2 != null)
        {
            camera1.Follow = GameManager.Instance.Player.transform;
            camera2.Follow = GameManager.Instance.Player.transform;
        }
    }

    public void SetCenter(float screenX, float screenY)
    {
        framing.m_ScreenX = screenX;
        framing.m_ScreenY = screenY;
    }

    public void SwitchCamera(CinemachineVirtualCamera targetCamera)
    {
        if (currentCamera == targetCamera) return;

        if (currentCamera != null)
            currentCamera.Priority = inactivePriority;

        Vector3 curPos = currentCamera.transform.position;

        currentCamera = targetCamera;
        currentCamera.Priority = activePriority;

        currentCamera.transform.position = curPos;
    }

    public IEnumerator WaitUntilCameraArrived()
    {
        Transform camTr = Camera.main.transform;
        Transform targetTr = currentCamera.Follow;

        while (Vector2.Distance(camTr.position, targetTr.position) > 0.1f)
        {
            yield return null;
        }
    }

    private void ToggleConfiner(bool enable)
    {
        if (currentCamera == null) return;

        var confiner = currentCamera.GetComponent<CinemachineConfiner2D>();

        if (confiner != null)
        {
            Debug.Log("들어옴");
            confiner.enabled = enable;
        }
    }

    #region 이동 + 줌 동시 실행
    public void FocusAndZoom(Transform from, Transform to, float duration, float targetZoom, MonoBehaviour runner)
    {
        runner.StartCoroutine(FocusAndZoomCoroutine(from, to, duration, targetZoom));
    }

    private IEnumerator FocusAndZoomCoroutine(Transform from, Transform to, float duration, float targetZoom)
    {
        ToggleConfiner(false);

        GameObject tempTarget = new GameObject("CameraTempTarget");
        tempTarget.transform.position = from.position;

        currentCamera.Follow = tempTarget.transform;
        currentCamera.LookAt = tempTarget.transform;

        bool isOrtho = currentCamera.m_Lens.Orthographic;
        float startValue = isOrtho ? currentCamera.m_Lens.OrthographicSize : currentCamera.m_Lens.FieldOfView;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            tempTarget.transform.position = Vector3.Lerp(from.position, to.position, t);

            float newValue = Mathf.Lerp(startValue, targetZoom, t);

            if (isOrtho)
                currentCamera.m_Lens.OrthographicSize = newValue;
            else
                currentCamera.m_Lens.FieldOfView = newValue;

            yield return null;
        }

        currentCamera.Follow = to;
        currentCamera.LookAt = to;
        Destroy(tempTarget);
        ToggleConfiner(true);
    }
    #endregion
}
