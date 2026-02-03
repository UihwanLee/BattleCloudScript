using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class DropItemChest : MonoBehaviour, IInteractable
{
    [Header("컴포넌트")]
    [SerializeField] private GameObject window;

    [Header("담고 있는 아이템 리스트")]
    [SerializeField] private List<ISlotItem> items = new List<ISlotItem>();

    [Header("드롭까지 시간")]
    [SerializeField] private float dropStartDuration = 2f;

    [Header("드롭 애니메이션 설정")]
    [Header("드롭 원 영역 Min/Max")]
    [SerializeField] private Vector2 randomRadiusDist;
    [Header("드롭 시간")]
    [SerializeField] private float dropDuration;

    private DropItemPool dropItemPool;
    private InfoUIPoolManager infoUIPoolManager;
    private GameObject currentInfoUI;
    private bool isCanInteract;

    private Coroutine dropStartCoroutine;

    private void Start()
    {
        infoUIPoolManager = InfoUIPoolManager.Instance;
        dropItemPool = DropItemPool.Instance;
        isCanInteract = false;
    }

    private void OnEnable()
    {
        //EventBus.OnCheckDropItem += CheckGainDropItem;
    }

    private void OnDisable()
    {
        //EventBus.OnCheckDropItem -= CheckGainDropItem;
    }


    private void Reset()
    {
        window = GameObject.Find("Container");
    }
    public void CheckGainDropItem(List<HUDItemSlot> list)
    {
        items.Clear();

        foreach (HUDItemSlot slot in list)
        {
            if (slot.gameObject.activeSelf)
            {
                items.Add(slot.Item);
                slot.gameObject.SetActive(false);
            }
        }

        if (items.Count <= 0)
        {
            // 없다면 리셋
            ResetChest();
            return;
        }

        // 먹은 아이템이 존재하면 켜기
        window.SetActive(true);

        // 특정 시간 후 드랍 시작
        if(dropStartCoroutine != null) StopCoroutine(dropStartCoroutine);
        dropStartCoroutine = StartCoroutine(StartDropCoroutine());
    }

    private void ResetChest()
    {
        isCanInteract = false;
        window.SetActive(false);
    }

    private IEnumerator StartDropCoroutine()
    {
        yield return new WaitForSeconds(dropStartDuration);

        Drop();
    }

    #region 인터페이스 구현

    public void OpenInteractUI(Player player)
    {
        if (isCanInteract == false) return;

        // InteractUI 띄우기
        currentInfoUI = infoUIPoolManager.GetInfoUI(InfoUIType.Interact, transform.position);

        if (currentInfoUI != null)
        {
            InteractUI interactUI = currentInfoUI.GetComponent<InteractUI>();
        }
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    public void CloseInteractUI(Player player)
    {
        if (isCanInteract == false) return;

        // InteractUI 반환
        if (currentInfoUI != null)
        {
            infoUIPoolManager.Release(InfoUIType.Interact, currentInfoUI);
        }
    }

    public void Interact(Player player)
    {
        if (isCanInteract == false) return;

        // 현재 Phase가 진행 중인지 체크
        if (GameManager.Instance.IsStart) return;

        foreach(var item in items)
        {
            float randomRadius = Random.Range(randomRadiusDist.x, randomRadiusDist.y);
            Vector3 randomTargetPosition = CalculateRandomCirclePosition(transform.position, randomRadius);
            dropItemPool.SpawnDropItemByDropAnimation(item, transform.position, randomTargetPosition, dropDuration);
        }

        // 소환하고 나서는 비활성화
        window.SetActive(false);
        isCanInteract = false;

        // InteractUI 반환
        if (currentInfoUI != null)
        {
            infoUIPoolManager.Release(InfoUIType.Interact, currentInfoUI);
        }
    }

    private void Drop()
    {
        // 현재 Phase가 진행 중인지 체크
        if (GameManager.Instance.IsStart) return;

        foreach (var item in items)
        {
            float randomRadius = Random.Range(randomRadiusDist.x, randomRadiusDist.y);
            Vector3 randomTargetPosition = CalculateRandomCirclePosition(transform.position, randomRadius);
            dropItemPool.SpawnDropItemByDropAnimation(item, transform.position, randomTargetPosition, dropDuration);
        }

        // 소환하고 나서는 비활성화
        window.SetActive(false);
        isCanInteract = false;

        // InteractUI 반환
        if (currentInfoUI != null)
        {
            infoUIPoolManager.Release(InfoUIType.Interact, currentInfoUI);
        }
    }

    // 원의 중심 위치를 기준으로 반지름 내의 랜덤 좌표를 계산하는 함수
    private Vector3 CalculateRandomCirclePosition(Vector3 center, float radius)
    {
        // Random.insideUnitCircle는 (0,0)을 중심으로 하는 반지름 1의 원 내부의 랜덤한 Vector2를 반환
        Vector2 randomOffset = Random.insideUnitCircle * radius;
        Vector3 randomPosition = center + new Vector3(randomOffset.x, randomOffset.y, 0f);

        return randomPosition;
    }

    public int GetPriority()
    {
        return Define.INTERACT_PRIORITY_01;
    }
    #endregion
}