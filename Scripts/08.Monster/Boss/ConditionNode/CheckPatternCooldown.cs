using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class CheckPatternCooldown : Node
{
   private TrollController controller;

    public CheckPatternCooldown(TrollController controller)
    {
        this.controller = controller;
    }

    public override NodeState Run()
    {
        if (controller.IsInPattern)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = controller.IsPatternReady ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }
}
