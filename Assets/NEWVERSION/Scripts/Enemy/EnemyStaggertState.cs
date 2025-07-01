using UnityEngine;
using CombatV2.FSM;

namespace CombatV2.Enemy
{
    /// <summary>
    /// Enemy b? choáng do b? parry
    /// </summary>
    public class EnemyStaggerState : CharacterState<EnemyController>
    {
        private float staggerDuration = 0.8f;
        private float timer = 0f;

        public EnemyStaggerState(EnemyController owner, StateMachine<EnemyController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {
            timer = 0f;
            Owner.PlayAnimation("Enemy_Stagger"); // Animation b? choáng
        }

        public override void Update()
        {
            timer += Time.deltaTime;
            if (timer >= staggerDuration)
            {
                stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine));
            }
        }
    }
}
