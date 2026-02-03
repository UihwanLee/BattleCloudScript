using UnityEngine;

public class TrollAnimationHandler : MonoBehaviour
{
    private TrollController controller;

    private void Awake()
    {
        controller = GetComponentInParent<TrollController>();
    }

    public void OnWindUpFinished()
    {
        controller.OnWindUpFinished();
    }

    public void OnDashAttackFinished()
    {
        controller.OnDashAttackFinished();
    }
   
    public void OnThrowBoneWithMeleeAttack()
    {
        controller.ThrowBonesWithMeleeAttack();
    }

    public void OnRangedAttack()
    {
        controller.RangedAttack();
    }

    public void OnDeadFinished()
    {
        Destroy(controller.gameObject);
        GameManager.Instance.CheckClearGame();
    }
}
