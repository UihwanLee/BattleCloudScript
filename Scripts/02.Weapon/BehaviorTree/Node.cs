using System;
using WhereAreYouLookinAt.Enum;

public abstract class Node
{
    protected NodeState state;
    public NodeState State => state;
    public virtual string Name => GetType().Name;
    public static Action<Node> OnNodeRunning;

    protected void ReportRunning()
    {
        OnNodeRunning?.Invoke(this);
    }

    public abstract NodeState Run();
}