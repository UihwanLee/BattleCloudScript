using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class CheckRangedAttackCooldown : Node
{
    private TrollController controller;

    public CheckRangedAttackCooldown(TrollController controller)
    {
        this.controller = controller;
    }

    public override NodeState Run()
    {
        state = controller.IsRangedAttackReady
               ? NodeState.SUCCESS
               : NodeState.FAILURE;

        return state;
    }
}
