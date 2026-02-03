using WhereAreYouLookinAt.Enum;

public class BehaviorTree
{
    private Node root;
    public Node CurrentRunningNode { get; private set; }
    /// <summary>
    /// BehaviorTree 생성
    /// </summary>
    /// <param name="root">Selector Node, Sequence Node들이 모인 Root Node</param>
    public BehaviorTree(Node root)
    {
        this.root = root;
        Node.OnNodeRunning += node =>
        {
            CurrentRunningNode = node;
        };
    }

    public void Tick()
    {
        CurrentRunningNode = null;
        root.Run();
    }
}