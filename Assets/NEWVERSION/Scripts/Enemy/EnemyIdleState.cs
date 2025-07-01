using CombatV2.FSM;
using UnityEngine;

namespace CombatV2.Enemy
{
    public class EnemyIdleState : CharacterState<EnemyController>
    {
        public EnemyIdleState(EnemyController owner, StateMachine<EnemyController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {
            Owner.animator.Play("Idle");
        }

        public override void Update()
        {
            if (Owner.player != null)
            {
                float distance = Vector2.Distance(Owner.transform.position, Owner.player.position);

                if (distance < Owner.config.detectionRange)
                {
                    stateMachine.ChangeState(new EnemyChaseState(Owner, stateMachine));
                }
            }
        }
    }
}
