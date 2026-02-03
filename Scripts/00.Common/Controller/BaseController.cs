using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class BaseController : MonoBehaviour, IKnockbackable
{
    [Header("Sprite")]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Movement")]
    // 이동 방향
    [SerializeField] protected Vector2 moveDirection;
    // 기본 이동 속도
    [SerializeField] protected float baseSpeed;
    [SerializeField] protected float currentSpeed;

    [Header("넉백 저항(0 ~ 1)")]
    // 넉백 저항
    [SerializeField] protected float knockbackRegistance;
    [Header("넉백 지속시간(줄어들 수록 많이 밀림)")]
    // 넉백 감소되는 속도, 시간
    [SerializeField] protected float knockbackDecay;
    [SerializeField] protected Vector3 knockbackVelocity;

    [Header("Slow Debuff Icon")]
    [SerializeField] protected SpriteRenderer slowIcon;

    protected Dictionary<object, float> slowSources = new Dictionary<object, float>();

    //protected Vector2 colliderSize;
    protected Collider2D _collider;
    protected LayerMask wallMask;
    protected float skinWidth;

    #region 프로퍼티
    public Vector2 MoveDirection => moveDirection;
    public float BaseSpeed => baseSpeed;
    public float CurrentSpeed => currentSpeed;
    #endregion

    protected virtual void Reset()
    {
        moveDirection = Vector2.zero;
    }

    protected virtual void Awake()
    {
        wallMask = LayerMask.GetMask("Wall");
        skinWidth = 0.05f;
        _collider = GetComponent<Collider2D>();

        if (slowIcon != null)
            slowIcon.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        knockbackVelocity = Vector3.zero;
        moveDirection = Vector2.zero;
    }

    protected virtual void Start()
    {
        CalculateSpeed();
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.State != GameState.RUNNING)
            return;

        Move();
        Flip();
    }

    protected bool IsBlocked(Vector2 move)
    {
        float distance = move.magnitude * Time.deltaTime + skinWidth;

        RaycastHit2D hit = Physics2D.BoxCast(
            _collider.bounds.center,
            _collider.bounds.size,
            0f,
            move.normalized,
            distance,
            wallMask
            );

        return hit.collider != null;
    }

    /// <summary>
    /// Transform 기반 Move<br/>
    /// baseSpeed에 스탯에 관련된 moveSpeed 적용 필요 <br/>
    /// moveDirection 적용 필요
    /// </summary>
    protected virtual void Move()
    {
        Vector3 velocity = new Vector3();

        Vector3 moveDelta = moveDirection * currentSpeed;

        if (knockbackVelocity.magnitude > 0.01f)
            moveDelta = knockbackVelocity;

        if (Mathf.Abs(moveDelta.x) > 0.0001f)
        {
            Vector2 moveX = new Vector2(moveDelta.x, 0f);

            if (!IsBlocked(moveX))
                velocity += (Vector3)moveX;
        }

        if (Mathf.Abs(moveDelta.y) > 0.0001f)
        {
            Vector2 moveY = new Vector2(0, moveDelta.y);

            if (!IsBlocked(moveY))
                velocity += (Vector3)moveY;
        }

        transform.position += velocity * Time.deltaTime;

        // knockbackDecay 동안 넉백 knockbackVelocity 감소
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);
    }

    public void ApplyKnockback(Transform other, float knockbackPower)
    {
        // 밀려날 방향 계산
        Vector3 direction = (transform.position - other.position).normalized;

        // 넉백 저항 적용
        knockbackPower *= (1 - knockbackRegistance);

        // 넉백 제한(넉백 저항이 1이상으로 들어왔을 때 반대 방향으로 밀려나지 않도록)
        knockbackPower = Mathf.Max(0, knockbackPower);

        knockbackVelocity = direction * knockbackPower;
    }
    
    public void SetSpriteRenderer(SpriteRenderer renderer) 
    {
        this.spriteRenderer = renderer;
    }

    /// <summary>
    /// 이동 방향에 따른 Sprite Flip
    /// </summary>
    protected virtual void Flip()
    {
        if (spriteRenderer == null) return;

        // 이동 중이 아닐 때는 플립 금지
        if (Mathf.Abs(moveDirection.x) < 0.01f)
            return;

        if (moveDirection.x < 0)
            spriteRenderer.flipX = true;
        else if (moveDirection.x > 0)
            spriteRenderer.flipX = false;
    }

    protected virtual void Death()
    {

    }

    /// <summary>
    /// 이동속도 감소
    /// </summary>
    /// <param name="ratio">변화시킬 속도 비율(%)</param>
    /// <returns>감소된 속도</returns>
    public void ApplySlow(object source, float ratio)
    {
        slowIcon.gameObject.SetActive(true);
        slowSources[source] = Mathf.Clamp01(ratio);
        CalculateSpeed();
    }

    /// <summary>
    /// 속도 복원
    /// </summary>
    /// <param name="slowedSpeed"></param>
    public void RemoveSlow(object source)
    {
        slowIcon.gameObject.SetActive(false);
        if (slowSources.Remove(source))
            CalculateSpeed();
    }

    /// <summary>
    /// 현재의 속도 계산
    /// </summary>
    protected virtual void CalculateSpeed()
    {
        float speed = baseSpeed;

        foreach (float ratio in slowSources.Values)
        {
            speed *= (1f - ratio);
        }

        // 최소 속도 제한
        currentSpeed = Mathf.Max(speed, 0.1f);
    }
}
