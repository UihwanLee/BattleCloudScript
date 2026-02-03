using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class DashAttackAction : Node
{
    private TrollController controller;
    private Animator animator;

    private bool started;

    private const string Attack_STATE = "Attack";

    public DashAttackAction(TrollController controller)
    {
        this.controller = controller;
        animator = controller.Animator;
    }

    public override NodeState Run()
    {
        if (!started)
        {
            started = true;
            controller.DashAttackFinished = false;
            controller.IsInPattern = true;
            controller.FacePlayer();
            controller.StartDashAttack();
            animator.Play(Attack_STATE, 0, 0f);
        }

        if (!controller.DashAttackFinished)
        {
            state = NodeState.RUNNING;
            return state;
        }

        controller.StopDashAttack();
        state = NodeState.SUCCESS;
        return state;
    }

    public void Reset()
    {
        started = false; 
        controller.DashAttackFinished = false;
    }
}
