using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class WindUpAction : Node
{
    private TrollController controller;
    private Animator animator;

    private bool started = false;

    private const string WINDUP_STATE = "WindUp";

    public WindUpAction(TrollController controller)
    {
        this.controller = controller;
        animator = controller.Animator;
    }   

    public override NodeState Run()
    {
        if (!started)
        {
            started = true;

            controller.IsInPattern = true;
            animator.Play(WINDUP_STATE, 0, 0);
            controller.SetMovementEnabled(false);
        }

        if (!controller.WindUpFinished)
        {
            state = NodeState.RUNNING;
            return state;
        }

        //Reset();

        state = NodeState.SUCCESS;
        return state;
    }

    public void Reset()
    {
        started = false;
        controller.WindUpFinished = false;
    }
}
