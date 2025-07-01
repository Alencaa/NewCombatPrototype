// Chạy tất cả các node con theo thứ tự, thất bại nếu có 1 node thất bại
using System.Collections.Generic;

public class BTSequence : BTNode
{
    private List<BTNode> children = new List<BTNode>();

    public void AddChild(BTNode node) => children.Add(node);

    public override State Evaluate()
    {
        foreach (var child in children)
        {
            var result = child.Evaluate();
            if (result != State.Success) return result;
        }
        return State.Success;
    }
}
