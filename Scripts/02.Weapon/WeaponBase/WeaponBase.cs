using UnityEngine;
using WhereAreYouLookinAt.Enum;

/// <summary>
/// Manager 관리용 base class
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected bool isInBase = true;

    #region 프로퍼티
    public WeaponBaseController Controller { get; protected set; }
    public WeaponCondition Condition { get; protected set; }
    public WeaponStat Stat { get; protected set; }
    public WeaponTrait Trait { get; protected set; }
    public WeaponEffectController EffectController { get; protected set; }
    public WeaponType WeaponType => weaponType;
    public bool IsInBase => isInBase;
    #endregion

    public virtual void Initialize(Transform pivot)
    {
        Controller.SetPivot(pivot);
    }

    public void SetIsInBase(bool isInBase)
    {
        this.isInBase = isInBase;
    }
}

/// <summary>
/// 실제 무기 class
/// </summary>
/// <typeparam name="TController">실제 무기에 사용되는 Controller class</typeparam>
public class WeaponBase<TController> : WeaponBase where TController : WeaponBaseController
{
    public TController TypedController => Controller as TController;
    protected virtual void Awake()
    {
        Controller = GetComponent<TController>();
        Condition = GetComponent<WeaponCondition>();
        Stat = GetComponent<WeaponStat>();
        Trait = GetComponent<WeaponTrait>();
        EffectController = GetComponent<WeaponEffectController>();
    }
}
