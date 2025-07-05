using CombatV2.FSM;
using UnityEngine;

namespace CombatV2.Enemy
{
    public class EnemyParriedState : CharacterState<EnemyController>
    {
        public EnemyParriedState(EnemyController owner, StateMachine<EnemyController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {
            Owner.animator.Play("Parried");
            Owner.movement.Stop();
            Owner.combat.TriggerParryFeedback();

            float delay = Owner.config != null ? Owner.config.parriedToStaggerDelay : 0.4f;
            //ChangeStateWithDelay(new EnemyStaggerState(Owner, stateMachine), delay);
        }

        
    }
}
