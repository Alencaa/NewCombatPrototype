// Node điều kiện – ví dụ kiểm tra player ở gần
using UnityEngine;

public class IsPlayerInRange : BTNode
{
    private Transform enemy;
    private Transform player;
    private float range;

    public IsPlayerInRange(Transform enemy, Transform player, float range)
    {
        this.enemy = enemy;
        this.player = player;
        this.range = range;
    }

    public override State Evaluate()
    {
        float dist = Vector3.Distance(enemy.position, player.position);
        return dist <= range ? State.Success : State.Failure;
    }
}
