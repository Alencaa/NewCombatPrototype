using UnityEngine;
using CombatV2.FSM;

namespace CombatV2.FSM.States
{
    public class IdleState : CharacterState<PlayerController>
    {
        public IdleState(PlayerController owner, StateMachine<PlayerController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entered Idle");

            // TODO: Play idle animation, unlock movement
        }

        public override void Update()
        {
            // Nếu có input → chuyển sang trạng thái khác
            if (Owner.InputInterpreter.DidAttack())
            {
                stateMachine.ChangeState(new AttackState(Owner, stateMachine));
            }

            // Có thể thêm điều kiện để chuyển sang Block, Dash, v.v.
        }
    }
}
