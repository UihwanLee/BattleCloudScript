using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class IsTargetInRangeNode : Node
{
    private BasicWeaponController controller;

    public IsTargetInRangeNode(BasicWeaponController controller)
    {
        this.controller = controller;
    }

    public override NodeState Run()
    {
        if (controller.CurrentTarget == null)
            return NodeState.FAILURE;

        float dist = Vector2.Distance(
            controller.transform.position,
            controller.CurrentTarget.transform.position
            );

        return dist <= controller.AttackRange ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
