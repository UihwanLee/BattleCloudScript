using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class ShootProjectileNode : Node
{
    private BasicWeaponController controller;

    public ShootProjectileNode(BasicWeaponController controller)
    {
        this.controller = controller;
    }

    public override NodeState Run()
    {
        controller.StartFire();
        controller.ResetCooldown();
        return NodeState.SUCCESS;
    }
}
