using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class GroggyAction : Node
{
    private TrollController controller;
    private Animator animator;

    private float duration;
    private float startTime;
    private bool started = false;
    private bool loopStarted = false;

    private const string IDLE_STATE = "Idle";
    private const string GROGGY_ENTER_STATE = "Groggy_Enter";
    private const string GROGG_LOOP_STATE = "Groggy_Loop";

    public GroggyAction(TrollController controller, float duration = 3f)
    {
        this.controller = controller;
        animator = controller.Animator;
        this.duration = duration;
    }

    public override NodeState Run()
    {
        if (!started)
        {
            started = true;
            startTime = Time.time;

            animator.Play(GROGGY_ENTER_STATE, 0, 0f);
            controller.SetMovementEnabled(false);
        }

        if (!loopStarted && IsFinished(GROGGY_ENTER_STATE))
        {
            loopStarted = true;
            animator.Play(GROGG_LOOP_STATE);
        }

        if (Time.time < startTime + duration)
        {
            state = NodeState.RUNNING;
            return state;
        }

        controller.IsInPattern = false;
        controller.SetMovementEnabled(true);
        controller.ConsumePatternCooldown();

        controller.WindUpAction.Reset();
        controller.DashAttackAction.Reset();
        Reset();

        animator.Play(IDLE_STATE);
        state = NodeState.SUCCESS;
        return state;
    }

    public void Reset()
    {
        started = false;
        loopStarted = false;
    }

    public bool IsFinished(string str)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(str)
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }
}
