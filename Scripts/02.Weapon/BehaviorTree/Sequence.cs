using System.Collections.Generic;
using WhereAreYouLookinAt.Enum;

public class Sequence : Node
{
    private List<Node> nodes = new List<Node>();
    private int currentIndex = 0;

    /// <summary>
    /// Sequence Node 생성
    /// </summary>
    /// <param name="nodes">순차 실행할 Node 배열</param>
    public Sequence(params Node[] nodes)
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

            if (result == NodeState.FAILURE)
            {
                currentIndex = 0;
                state = NodeState.FAILURE;
                return state;
            }

            currentIndex++; // SUCCESS → 다음 노드
        }

        currentIndex = 0;
        state = NodeState.SUCCESS;
        return state;
    }
}