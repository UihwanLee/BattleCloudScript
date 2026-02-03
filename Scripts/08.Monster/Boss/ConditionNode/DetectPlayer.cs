using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class DetectPlayer : Node
{
    private TrollController controller;

    public DetectPlayer(TrollController controller)
    {
        this.controller = controller;
    }

    public override NodeState Run()
    {
        if (controller.Player == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
