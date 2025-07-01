using CombatV2.Enemy;
using CombatV2.FSM;
using UnityEngine;

public class EnemyChaseState : CharacterState<EnemyController>
{
    public EnemyChaseState(EnemyController owner, StateMachine<EnemyController> stateMachine)
        : base(owner, stateMachine) { }

    public override void Update()
    {
        if (Owner.player != null)
        {
            Owner.movement.Move(Owner.player.position - Owner.transform.position, Owner.config.detectionRange);

            float dist = Vector2.Distance(Owner.transform.position, Owner.player.position);
            if (dist < Owner.config.attackRange)
            {
                stateMachine.ChangeState(new EnemyCombatAttackState(Owner, stateMachine));
            }
        }
        else
        {
            Debug.LogWarning("Player not found! Cannot chase.");
            stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine));
        }
    }
}
