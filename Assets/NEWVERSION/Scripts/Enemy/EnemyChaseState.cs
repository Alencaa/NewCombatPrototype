using CombatV2.Enemy;
using CombatV2.FSM;
using UnityEngine;

// 👣 Trạng thái đuổi theo player
public class EnemyChaseState : CharacterState<EnemyController>
{
    public EnemyChaseState(EnemyController owner, StateMachine<EnemyController> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Enemy: Entering CHASE state");
        Owner.PlayAnimation("Run"); // animation chạy
    }

    public override void Update()
    {
        if (Vector2.Distance(Owner.transform.position, Owner.player.position) < 1.2f)
        {
            // Gần đủ để tấn công
            stateMachine.ChangeState(new EnemyCombatAttackState(Owner, stateMachine));
        }
        else
        {
            // Đuổi theo
            Owner.MoveToward(Owner.player.position);
        }
    }
}
