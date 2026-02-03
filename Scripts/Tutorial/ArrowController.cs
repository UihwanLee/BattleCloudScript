using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Transform target;
    private Transform player;

    [SerializeField] private float offsetDistance = 1.5f; // 플레이어 앞에 띄울 거리
    [SerializeField] private float heightOffset = 0.5f;   // 살짝 위로 띄우기

    public void SetTarget(Transform t)
    {
        target = t;
        player = GameManager.Instance.Player.transform;
    }

    private void Update()
    {
        if (target == null || player == null)
        {
            return;
        }

        Vector3 dirToTarget = (target.position - player.position).normalized;
        transform.position = player.position + dirToTarget * offsetDistance + Vector3.up * heightOffset;

        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

}
