using UnityEngine;

// 플레이어 InteractHandler
// 상호작용 가능한 오브젝트들을 검출 후 가장 가까이 있는 상호작용 오브젝트 전달
public class PlayerInteractHandler : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private float detectRadius = 1f;       // 감지 범위
    [SerializeField] private LayerMask interactLayer;       // 상호작용 오브젝트의 레이어

    [Header("자석 컴포넌트")]
    [SerializeField] private float attractRadius = 3f;      // 자석 범위

    private Collider2D interactObj;

    private Player player;
    private IInteractable nearestObject = null;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        CheckForInteractable();
        SendInteract();
        CheckForAttractItem();
    }

    void CheckForInteractable()
    {
        // 주변 모든 상호작용 물체 검출
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z),
                                                        detectRadius, interactLayer);

        nearestObject = null;
        float minDistance = Mathf.Infinity;

        // 가장 가까운 상호작용 오브젝트 찾기
        foreach (var col in colliders)
        {
            IInteractable interactable = col.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < minDistance)
                {
                    interactObj = col;
                    minDistance = distance;
                    nearestObject = interactable;
                }
            }
        }
    }

    private void SendInteract()
    {
        // 검출된 오브젝트가 없다면 
        if (nearestObject == null)
        {
            // 기존에 잡고 있던 오브젝트가 있었다면 UI를 닫고 비워줌
            if (player.Interact != null)
            {
                player.Interact.CloseInteractUI(player);
                player.Interact = null;
            }
            return; 
        }

        // 갱신된 것이 아니라면 return
        if (player.Interact == nearestObject) return;

        // 이전 상호작용 오브젝트 UI 창 닫기
        if (player.Interact != null) player.Interact.CloseInteractUI(player);

        // 플레이어 인터렉트 오브젝트 설정
        player.Interact = nearestObject;

        // 상호작용 UI 띄우기
        player.Interact.OpenInteractUI(player);
    }

    private void CheckForAttractItem()
    {
        // 주변 모든 상호작용 물체 검출
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z),
                                                        attractRadius, interactLayer);

        // 리소스 아이템이라면 자석 기능 키기
        foreach (var col in colliders)
        {
            ResourceDrop resource = col.GetComponent<ResourceDrop>();
            if (resource != null)
            {
                // 조언자&조언의 경우 한번 체크할 수 있도록 설정
                DropItem dropItem = col.GetComponent<DropItem>();
                if (dropItem != null)
                {
                    if (dropItem.IsMonsterDrop)
                        resource.OnAttract();
                    else resource.OffAttract();
                    return;
                }
                resource.OnAttract();
            }
        }
    }

    // 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), detectRadius);
    }

    #region 프로퍼티

    public Collider2D InteractCollider { get { return interactObj; } }
    public float AttractRadius { get { return attractRadius; } set { attractRadius = value; if (attractRadius < 1f) attractRadius = 1f; } }

    #endregion
}
