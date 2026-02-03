using WhereAreYouLookinAt.Enum;

public class IsCooldownReadyNode : Node
{
    private BasicWeaponController controller;

    public IsCooldownReadyNode(BasicWeaponController controller)
    {
        this.controller = controller;
    }

    public override NodeState Run()
    {
        return controller.IsCooldownReady() ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
