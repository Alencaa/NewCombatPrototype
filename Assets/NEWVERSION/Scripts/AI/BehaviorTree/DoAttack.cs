// Node hành động – gọi animation attack
using UnityEngine;

public class DoAttack : BTNode
{
    private Animator animator;

    public DoAttack(Animator animator)
    {
        this.animator = animator;
    }

    public override State Evaluate()
    {
        animator.SetTrigger("DoAttack");
        return State.Success;
    }
}
