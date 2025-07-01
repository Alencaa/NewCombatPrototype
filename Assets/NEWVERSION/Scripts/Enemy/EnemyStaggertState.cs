using CombatV2.FSM;
using UnityEngine;

namespace CombatV2.Enemy
{
    public class EnemyStaggerState : CharacterState<EnemyController>
    {

        public EnemyStaggerState(EnemyController owner, StateMachine<EnemyController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {
            Owner.animator.Play("Stagger");
            Owner.combat.TriggerStaggerFeedback();
            Owner.movement.Stop();
            ChangeStateWithDelay(new EnemyIdleState(Owner, stateMachine), Owner.config.staggerDuration);
        }
    }
}
