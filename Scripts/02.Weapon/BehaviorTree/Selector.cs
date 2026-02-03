using System.Collections.Generic;
using WhereAreYouLookinAt.Enum;

public class Selector : Node
{
    private List<Node> nodes = new List<Node>();
    private int currentIndex = 0;

    /// <summary>
    /// Selector Node 생성
    /// </summary>
    /// <param name="nodes">선택 실행할 Node 배열</param>
    public Selector(params Node[] nodes)
    {
        this.nodes = new List<Node>(nodes);
    }

    public override NodeState Run()
    {
        while (currentIndex < nodes.Count)
        {
            var result = nodes[currentIndex].Run();

            if (result == NodeState.RUNNING)
            {
                state = NodeState.RUNNING;
                return state;
            }

            if (result == NodeState.SUCCESS)
            {
                currentIndex = 0;
                state = NodeState.SUCCESS;
                return state;
            }

            currentIndex++;
        }

        currentIndex = 0;
        state = NodeState.FAILURE;
        return state;
    }
}