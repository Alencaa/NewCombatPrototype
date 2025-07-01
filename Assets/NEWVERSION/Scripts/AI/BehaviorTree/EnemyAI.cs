using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private BTNode rootNode;

    public Transform player;
    public Animator animator;

    void Start()
    {
        var sequence = new BTSequence();
        sequence.AddChild(new IsPlayerInRange(transform, player, 3f));
        sequence.AddChild(new DoAttack(animator));

        rootNode = sequence;
    }

    void Update()
    {
        rootNode.Evaluate();
    }
}
