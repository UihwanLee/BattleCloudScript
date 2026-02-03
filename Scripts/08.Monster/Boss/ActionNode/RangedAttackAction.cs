using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class RangedAttackAction : Node
{
    private TrollController controller;
    private Animator animator;
    private bool started;

    private const string RANGED_STATE = "RangedAttack";

    public RangedAttackAction(TrollController controller)
    {
        this.controller = controller;
        animator = controller.Animator;
    }

    public override NodeState Run()
    {
        if (!started)
        {
            started = true;
            controller.SetMovementEnabled(false);
            controller.FacePlayer();
            animator.Play(RANGED_STATE, 0, 0f);
        }

        if (!IsAnimationFinished())
        {
            state = NodeState.RUNNING;
            return state;
        }

        controller.ConsumeRangedCooldown();
        controller.SetMovementEnabled(true);

        started = false;

        state = NodeState.SUCCESS;
        return state;
    }

    private bool IsAnimationFinished()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(RANGED_STATE) && info.normalizedTime >= 1f;
    }
}
