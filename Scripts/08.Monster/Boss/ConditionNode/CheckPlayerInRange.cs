using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class CheckPlayerInRange : Node
{
    private TrollController controller;
    private float range;

    public CheckPlayerInRange(TrollController controller, float range)
    {
        this.controller = controller;
        this.range = range;
    }

    public override NodeState Run()
    {
        float dist = Vector2.Distance(
            controller.transform.position, 
            controller.Player.transform.position
            );

        state = dist < range ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }
}
