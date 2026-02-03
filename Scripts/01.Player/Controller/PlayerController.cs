using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using WhereAreYouLookinAt.Enum;

public class PlayerController : BaseController
{
    private Player player;

    [Header("움직임 제어")]
    [SerializeField] protected bool isMovementEnabled;

    [Header("Interaction")]
    private float currentInteractCoolTime;
    private bool isInteractable;
    private bool isCricleInteract;

    private Coroutine circleGauageCoroutine;
    private GameObject currentCircleInfoUI;
    private bool wasMoving = false;

    protected override void Awake()
    {
        isMovementEnabled = false;

        base.Awake();
        skinWidth = 0.02f;
    }

    protected override void Start()
    {
        player = GetComponent<Player>();
        baseSpeed = player.Stat.MoveSpeed.Value;

        currentInteractCoolTime = 0f;
        isInteractable = false;

        base.Start();
    }

    protected override void Update()
    {
        if (player.Condition.IsDead)
            return;

        base.Update();

        bool isMoving = moveDirection.sqrMagnitude > 0.0001f;

        if (!GameManager.Instance.Player.Condition.IsDead && isMoving && !wasMoving)
        {
            EventBus.OnPlayerMoveStart?.Invoke();
            wasMoving = true;
        }
        else if ((!GameManager.Instance.Player.Condition.IsDead && !isMoving && wasMoving) || (GameManager.Instance.Player.Condition.IsDead))
        {
            EventBus.OnPlayerMoveStop?.Invoke();
            wasMoving = false;
        }

        // InteractCoolTime 계산
        if (!isInteractable)
        {
            if (currentInteractCoolTime >= Define.PLAYER_INTERACT_COOLTIME)
            {
                currentInteractCoolTime = 0f;
                isInteractable = true;
            }

            currentInteractCoolTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// 움직임을 제한하는 메서드
    /// </summary>
    /// <param name="enabled">움직이게 할건지 여부</param>
    public virtual void SetMovementEnabled(bool enabled)
    {
        isMovementEnabled = enabled;

        if (!isMovementEnabled)
        {
            moveDirection = Vector2.zero;
        }
    }

    public bool GetMovementEnabled()
    {
        return isMovementEnabled;
    }

    protected override void Move()
    {
        Vector2 inputDir = moveDirection;

        if (inputDir.sqrMagnitude <= 0.0001f)
            return;

        inputDir = inputDir.normalized;

        Vector2 moveDelta = inputDir * player.Stat.MoveSpeed.Value/* * Time.deltaTime*/;
        Vector3 velocity = new Vector3();

        // x축 이동
        if (moveDelta.x != 0)
        {
            Vector2 moveX = new Vector2(moveDelta.x, 0f);

            if (!IsBlocked(moveX))
            {
                velocity += (Vector3)moveX;
            }
        }

        // y축 이동
        if (moveDelta.y != 0f)
        {
            Vector2 moveY = new Vector2(0f, moveDelta.y);

            if (!IsBlocked(moveY))
            {
                velocity += (Vector3)moveY;
            }
        }

        transform.position += velocity * Time.deltaTime;
    }

    public void Teleportation(Vector3 position)
    {
        StartCoroutine(TeleportationRoutine(position));
    }

    private IEnumerator TeleportationRoutine(Vector3 position)
    {
        //SetMovementEnabled(false);

        // 플레이어 이동
        transform.position = position;

        // 조언자 이동
        for (int i = 0; i < Define.MAX_WEAPONCOUNT; i++)
        {
            WeaponBase weapon = player.WeaponManager.GetWeapon(i);

            if (weapon != null)
            {
                weapon.Controller.TeleportationToPivot();
            }
        }

        // 카메라 대기
        yield return null;//CameraManager.Instance.WaitUntilCameraArrived();

        //SetMovementEnabled(true);
    }

    public void IsMoveToBase(bool isMoveToBase)
    {
        for (int i = 0; i < Define.MAX_WEAPONCOUNT; i++)
        {
            WeaponBase weapon = player.WeaponManager.GetWeapon(i);

            if (weapon != null)
            {
                weapon.SetIsInBase(isMoveToBase);
            }
        }
    }

    #region InputAction
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isMovementEnabled)
        {
            player.AnimationHandler?.OnMove(false);
            moveDirection = Vector2.zero;
            return;
        }

        if (context.phase == InputActionPhase.Performed)
        {
            // 키 입력 감지
            player.AnimationHandler.OnMove(true);
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            player.AnimationHandler.OnMove(false);
            moveDirection = Vector2.zero;
        }
    }

    public void OnSlotOpen(InputAction.CallbackContext context)
    {
        if (player.IsReinforcementMode) return;

        //if (player.ToggleQAType == SlotToggleQAType.Toggle방식)
        //{
        //    if (context.phase == InputActionPhase.Started)
        //    {
        //        if (player.CanvasQAType == PlayerSlotQAType.인게임UI)
        //        {
        //            player.PlayerSlotUI.SetPlayerSlotUI(!player.PlayerSlotUI.transform.GetChild(0).gameObject.activeSelf);
        //        }
        //        else
        //        {
        //            player.CanvasPlayerSlotUI.SetPlayerSlotUI(!player.CanvasPlayerSlotUI.transform.GetChild(0).gameObject.activeSelf);
        //        }
        //    }
        //}
        //else
        //{
        //    if (context.phase == InputActionPhase.Performed)
        //    {
        //        if (player.CanvasQAType == PlayerSlotQAType.인게임UI)
        //        {
        //            player.PlayerSlotUI.SetPlayerSlotWindow(true);
        //        }
        //        else
        //        {
        //            player.CanvasPlayerSlotUI.SetPlayerSlotWindow(true);
        //        }
        //    }
        //    else if (context.phase == InputActionPhase.Canceled)
        //    {
        //        if (player.CanvasQAType == PlayerSlotQAType.인게임UI)
        //        {
        //            player.PlayerSlotUI.SetPlayerSlotWindow(false);
        //        }
        //        else
        //        {
        //            player.CanvasPlayerSlotUI.SetPlayerSlotWindow(false);
        //        }
        //    }
        //}
    }

    public void OnIneteract(InputAction.CallbackContext context)
    {
        // Circle 상호작용 1초 꾹 눌렀을 때
        if (context.performed)
        {
            // Interact할 오브젝트가 있는지 확인
            if (player.Interact != null)
            {
                if (player.Interact is DropItem)
                {
                    CircleInteract();
                }
            }
            isCricleInteract = true;
        }

        // 줍기 로직 (2초가 되기 전에 키를 뗐을 때)
        if (context.canceled)
        {
            // Circle 상호작용 중이었다면 return
            if (isCricleInteract)
            {
                // 만약 코루틴 도중에 취소했다면 코루틴 멈추기
                if (circleGauageCoroutine != null)
                {
                    StopCoroutine(circleGauageCoroutine);
                }

                // InfoUI가 남겨져 있다면 비활성화
                if (currentCircleInfoUI != null)
                {
                    InfoUIPoolManager.Instance.Release(InfoUIType.Circle, currentCircleInfoUI.gameObject);
                }

                isCricleInteract = false;
                return;
            }

            // Interact할 오브젝트가 있는지 확인
            if (player.Interact != null)
            {
                // 상호작용 할 수 있는지 확인 (쿨타임)
                if (!isInteractable) return;

                // 상호작용
                player.Interact.Interact(player);

                // 상호작용 변수 초기화
                isInteractable = false;
            }
        }
    }

    private void CircleInteract()
    {
        currentCircleInfoUI = InfoUIPoolManager.Instance.GetInfoUI(InfoUIType.Circle);
        if (currentCircleInfoUI != null)
        {
            CircleInfoUI circleInfoUI = currentCircleInfoUI.GetComponent<CircleInfoUI>();
            StartCircleInteraction(circleInfoUI);
        }
    }

    public void StartCircleInteraction(CircleInfoUI circleInfoUI)
    {
        if (circleGauageCoroutine != null) StopCoroutine(circleGauageCoroutine);
        circleGauageCoroutine = StartCoroutine(CircleGauageCoroutine(circleInfoUI));
    }

    private IEnumerator CircleGauageCoroutine(CircleInfoUI circleInfoUI)
    {
        // 위치 지정
        if (player.Interact != null)
        {
            circleInfoUI.gameObject.transform.position = player.Interact.GetPosition();
        }

        float time = 0;

        while (time < circleInfoUI.Duration)
        {
            time += Time.deltaTime;
            float t = time / circleInfoUI.Duration;

            // Circle 채우기
            circleInfoUI.Circle.fillAmount = t;

            yield return null;
        }

        // 끝나면 변수 초기화
        circleGauageCoroutine = null;
        InfoUIPoolManager.Instance.Release(InfoUIType.Circle, currentCircleInfoUI.gameObject);
        currentCircleInfoUI = null;

        // CircleInteract 시작

        // Interact할 오브젝트가 있는지 확인
        if (player.Interact != null)
        {
            // 상호작용 할 수 있는지 확인 (쿨타임)
            if (isInteractable)
            {
                // 상호작용
                player.Interact.Interact(player);

                // 상호작용 변수 초기화
                isInteractable = false;
            }
        }
    }

    #endregion

    #region 프로퍼티

    public bool IsCricleInteract { get { return isCricleInteract; } }

    #endregion
}
