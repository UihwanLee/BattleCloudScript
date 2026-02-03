using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class FindNearestTargetNode : Node
{
    private BasicWeaponController controller;

    public FindNearestTargetNode(BasicWeaponController controller)
    {
        this.controller = controller;
    }

    public override NodeState Run()
    {
        Transform target = controller.FindNearestTarget();
        return target == null ? NodeState.FAILURE : NodeState.SUCCESS;
    }
}
