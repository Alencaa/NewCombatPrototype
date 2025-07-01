// Chạy lần lượt các node con, thành công nếu có 1 node thành công
using System.Collections.Generic;

public class BTSelector : BTNode
{
    private List<BTNode> children = new List<BTNode>();

    public void AddChild(BTNode node) => children.Add(node);

    public override State Evaluate()
    {
        foreach (var child in children)
        {
            var result = child.Evaluate();
            if (result == State.Success) return State.Success;
            if (result == State.Running) return State.Running;
        }
        return State.Failure;
    }
}
