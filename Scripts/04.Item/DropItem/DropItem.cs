using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class DropItem : MonoBehaviour, IInteractable
{
    [Header("아이템 정보")]
    [SerializeField] private ISlotItem item;                     // Item 정보
    [SerializeField] private DropItemType type;

    private InfoUIPoolManager infoUIPoolManager;
    private GameObject currentInfoUI;
    private SpriteRenderer spriteRenderer;
    private DropItemPool dropItemPool;

    private AnimationCurve snapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 이동 시 사용할 곡선
    private Coroutine dropAnimationCoroutine;

    private Coroutine fallingAnimationCoroutine;
    private ResourceDrop resourceDrop;

    private bool isMonsterDrop = false;
    private bool isFalling = false;
    private bool isDropping = false;

    private void Start()
    {
        infoUIPoolManager = InfoUIPoolManager.Instance;
        dropItemPool = DropItemPool.Instance;
        resourceDrop = GetComponent<ResourceDrop>();
    }

    public void SetDropItem(ISlotItem newItem)
    {
        // 아이템 설정
        this.item = newItem;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetMonsterDrop(bool _isMonsterDrop)
    {
        // 상호작용 가능한 상태인지 체크
        isMonsterDrop = _isMonsterDrop;
    }

    public void SetAttract(bool active)
    {
        // 몬스터 드랍이면 Attract 활성화 아니면 끄기
        if (resourceDrop == null) resourceDrop = GetComponent<ResourceDrop>();
        if (active) resourceDrop.OnAttract(); else resourceDrop.OffAttract();
    }

    public void InitItemIcon(ISlotItem _item)
    {
        // 이미지 재설정
        if (_item != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = DataManager.BaseDropItemSprite;
        }
    }

    public void OpenItem(ISlotItem _item)
    {
        // 상호작용 가능하게 설정
        SetMonsterDrop(false);

        // 이미지 재설정
        if (_item != null)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _item.GetIcon();
        }
    }

    public void OpenInteractUI(Player player)
    {
        if (isMonsterDrop) return;

        if (isDropping) return;

        if (isFalling) return;

        // ItemInfoUI 띄우기
        currentInfoUI = infoUIPoolManager.GetInfoUI(InfoUIType.Item);

        if (currentInfoUI != null)
        {
            ItemInfoUI itemInfoUI = currentInfoUI.GetComponent<ItemInfoUI>();

            // 초기 변수 초기화
            Color color = Color.white;
            string interactMessage = Define.LANAUAGE_INTERACT_DROPITEM_SUCCESS;

            // 무기/아이템 슬롯 칸 확인
            if (IsItemWeapon(item))
            {
                if (!player.PlayerSlotUI.IsCanPickUpWeapon())
                {
                    color = Color.red;
                    interactMessage = Define.LANAUAGE_INTERACT_DROPITEM_FAIL;
                }
            }
            else
            {
                if (!player.PlayerSlotUI.IsCanPickUpItem())
                {
                    color = Color.red;
                    interactMessage = Define.LANAUAGE_INTERACT_DROPITEM_FAIL;
                }
            }

            string tableName = (item.GetType() == DropItemType.Weapon) ? Define.TABLE_WEAPON_NAME : 
                (item.GetType() == DropItemType.ExclusiveItem) ? Define.TABLE_ITEM_EXCLUSIVE_NAME : Define.TABLE_ITEM_NAME;

            itemInfoUI.SetUI(item.GetName(), item.GetDesc(), tableName, interactMessage, 
                item.GetValueList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT), item.GetValueTypeList(Define.LANAUAGE_ITEM_INFO_MAX_VALUE_COUNT),
                transform.position, true, color);
        }
    }

    public void CloseInteractUI(Player player)
    {
        // InteractUI 반환
        if (currentInfoUI != null)
        {
            infoUIPoolManager.Release(InfoUIType.Interact, currentInfoUI);
        }
    }

    // 현재 이 아이템이 소지하고 있는 클래스가 Weapon인지 Item인지 확인하기
    private bool IsItemWeapon(ISlotItem item)
    {
        return item.GetType() == DropItemType.Weapon;
    }

    #region IInteractable 인터페이스 구현

    public void Interact(Player player)
    {
        if (isMonsterDrop) return;

        if(player.Controller.IsCricleInteract)
        {
            // CircleInteract 중이었다면 분해
            DisassembleItem();
        }
        else
        {
            GainItem(player);
        }
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    /// <param name="player"></param>
    private void GainItem(Player player)
    {
        if (IsItemWeapon(item))
        {
            // PlayerSlot에 Item 추가
            if (player.CanvasQAType == PlayerSlotQAType.인게임UI)
            {
                if (player.PlayerSlotUI.AddWeapon(item) != Define.NO_FIND_NUMBER)
                {
                    // 습득 파티클 효과
                    GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
                    AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
                    // 이 DropItem은 PoolManager에게 반환
                    dropItemPool.Release(this.gameObject);
                }
                else
                {
                    if (currentInfoUI != null)
                    {
                        ItemInfoUI itemInfoUI = currentInfoUI.GetComponent<ItemInfoUI>();
                        itemInfoUI.Shaking();
                    }
                }
            }
            else
            {
                if (player.CanvasPlayerSlotUI.AddWeapon(item) != Define.NO_FIND_NUMBER)
                {
                    // 습득 파티클 효과
                    GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
                    AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
                    // 이 DropItem은 PoolManager에게 반환
                    dropItemPool.Release(this.gameObject);
                }
                else
                {
                    if (currentInfoUI != null)
                    {
                        ItemInfoUI itemInfoUI = currentInfoUI.GetComponent<ItemInfoUI>();
                        itemInfoUI.Shaking();
                    }
                }
            }
        }
        else
        {
            // PlayerSlot에 Item 추가
            if (player.CanvasQAType == PlayerSlotQAType.인게임UI)
            {
                if (player.PlayerSlotUI.AddItem(item) != Define.NO_FIND_NUMBER)
                {
                    // 습득 파티클 효과
                    GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
                    AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
                    // 이 DropItem은 PoolManager에게 반환
                    dropItemPool.Release(this.gameObject);
                }
                else
                {
                    if (currentInfoUI != null)
                    {
                        ItemInfoUI itemInfoUI = currentInfoUI.GetComponent<ItemInfoUI>();
                        itemInfoUI.Shaking();
                    }
                }
            }
            else
            {
                if (player.CanvasPlayerSlotUI.AddItem(item) != Define.NO_FIND_NUMBER)
                {
                    // 습득 파티클 효과
                    GameManager.Instance.ParticleManager.PlayLootEffect(player.transform);
                    AudioManager.Instance.PlayClip(WhereAreYouLookinAt.Enum.SFXType.PlayerLootGold);
                    // 이 DropItem은 PoolManager에게 반환
                    dropItemPool.Release(this.gameObject);
                }
                else
                {
                    if (currentInfoUI != null)
                    {
                        ItemInfoUI itemInfoUI = currentInfoUI.GetComponent<ItemInfoUI>();
                        itemInfoUI.Shaking();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 아이템 분해
    /// </summary>
    public void DisassembleItem()
    {
        float cost = 0;
        if (IsItemWeapon(item))
        {
            // 게임 룰에서 무기 분해 비용 가져오기
            cost = GameRule.Instance.GetWeaponDisassemblyCostByLv(item.GetLv());

            // 분해 이펙트 호출 (아이템 위치 기준)
            GameManager.Instance.ParticleManager.PlayDismantleEffect(transform);

            // 이 DropItem은 PoolManager에게 반환
            dropItemPool.Release(this.gameObject);
        }
        else
        {
            
            // 게임 룰에서 아이템 분해 비용 가져오기
            cost = GameRule.Instance.GetItemDisassemblyCostByLv(item.GetLv());

            // 분해 이펙트 호출 (아이템 위치 기준)
            GameManager.Instance.ParticleManager.PlayDismantleEffect(transform);

            // 이 DropItem은 PoolManager에게 반환
            dropItemPool.Release(this.gameObject);
        }

        // 플레이어 골드 업데이트
        GameManager.Instance.Player.Condition.Add(AttributeType.Gold, cost);
    }

    public int GetPriority()
    {
        // 드롭 아이템은 상호작용 우선순위가 가장 높다
        return Define.INTERACT_PRIORITY_01;
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    #endregion

    #region 애니메이션 연출

    // DropAnimation 시작
    public void StartDropAnimation(Vector3 targetPosition, float dropDuration)
    {
        if(dropAnimationCoroutine != null) StopCoroutine(dropAnimationCoroutine);
        dropAnimationCoroutine = StartCoroutine(DropAnimationCoroutine(targetPosition, dropDuration)); 
    }

    // DropAnimation 코루틴
    private IEnumerator DropAnimationCoroutine(Vector3 targetPosition, float dropDuration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        isDropping = true;

        while (elapsedTime < dropDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dropDuration);

            // AnimationCurve을 적용하여 t값 적용
            float curveValue = snapCurve.Evaluate(t);

            // targetPosition으로 Lerp
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null; 
        }

        isDropping = false;

        // 위치 설정
        transform.position = targetPosition;
    }

    // 떨어지기 애니메이션 시작
    public void StartFallAnimation(float height, float duration)
    {
        if (fallingAnimationCoroutine != null) StopCoroutine(fallingAnimationCoroutine);
        fallingAnimationCoroutine = StartCoroutine(FallingAnimation(height, duration));
    }

    private IEnumerator FallingAnimation(float height, float duration)
    {
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position - new Vector3(0f, height, 0f);

        isFalling = true;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / duration);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        isFalling = false;
        //폴링 파티클
        GameManager.Instance.ParticleManager.PlayDropEffect(transform);
    }

    #endregion

    #region 프로퍼티

    public ISlotItem Item { get { return item; } }
    public bool IsMonsterDrop { get {  return isMonsterDrop; } } 

    #endregion
}
