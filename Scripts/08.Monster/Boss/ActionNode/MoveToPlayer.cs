using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class MoveToPlayer : Node
{
    private TrollController controller;
    private Animator animator;
    private float stopDistance;

    public MoveToPlayer(TrollController controller, float stopDistance = 0.5f)
    {
        this.controller = controller;
        this.stopDistance = stopDistance;
        animator = controller.Animator; 
    }

    public override NodeState Run()
    {
        if (controller.IsInPattern)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (controller.Player == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        float dist = Vector2.Distance(
            controller.transform.position,
            controller.Player.transform.position
            );

        if (dist > stopDistance)
        {
            controller.SetMovementEnabled(true);
            controller.MoveTowardsPlayer();
        }
        else
        {
            controller.SetMovementEnabled(false);
            animator.Play("Idle");
        }

        controller.MoveTowardsPlayer();
        controller.FacePlayer();

        state = NodeState.SUCCESS;
        return state;
    }
}
