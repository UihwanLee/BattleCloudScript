using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class Player : MonoBehaviour
{
    [Header("QA 방식 - PlayerSlot Toggle 방식")]
    [SerializeField] private SlotToggleQAType slotToggleQAType;

    [Header("QA 방식 - PlayerSlot Canvas 방식")]
    [SerializeField] private PlayerSlotQAType playerSlotQAType;

    [Header("플레이어 경험치 증가 값 (선형)")]
    [SerializeField] public float exp_amount;

    #region 프로퍼티
    public GameObject Prefab { get; set; }
    public PlayerTrait Trait { get; private set; }
    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public PlayerStat Stat { get; private set; }
    public PlayerSlotUI PlayerSlotUI { get; private set; }
    public PlayerSlotUI CanvasPlayerSlotUI { get; private set; }
    public PlayerEffectHandler PlayerEffectHandler { get; private set; }
    public PlayerInteractHandler PlayerInteractHandler { get; private set; }
    public WeaponManager WeaponManager { get; private set; }
    public PlayerAnimationHandler AnimationHandler { get; set; }
    public PlayerVisualEffect VisualEffect { get; set; }

    public IInteractable Interact { get; set; }
    public bool IsReinforcementMode { get; set; }

    public SlotToggleQAType ToggleQAType { get {  return slotToggleQAType; } }
    public PlayerSlotQAType CanvasQAType { get {  return playerSlotQAType; } } 

    #endregion

    private void Awake()
    {
        Trait = GetComponent<PlayerTrait>();    
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        Stat = GetComponent<PlayerStat>();
        PlayerSlotUI = GetComponentInChildren<PlayerSlotUI>();
        PlayerEffectHandler = GetComponentInChildren<PlayerEffectHandler>();
        PlayerInteractHandler = GetComponentInChildren<PlayerInteractHandler>();
        WeaponManager = GetComponentInChildren<WeaponManager>();
        AnimationHandler = GetComponentInChildren<PlayerAnimationHandler>();
    }
}
