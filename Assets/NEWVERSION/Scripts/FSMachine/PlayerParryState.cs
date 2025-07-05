using UnityEngine;

namespace CombatV2.FSM.States 
{
    public class PlayerParryState : CharacterState<PlayerController>
    {

        private readonly GestureData gesture;

        public PlayerParryState(PlayerController owner, StateMachine<PlayerController> stateMachine, GestureData gesture) : base(owner, stateMachine)
        {
            this.gesture = gesture;
        }

        public override void Enter()
        {
            base.Enter();
            string dirName = Owner.GetDirectionName(gesture.direction);
            string anim = $"Parry_{dirName}";
            Owner.Animator.Play(anim);
            Debug.Log($"▶ FSM Parry → {anim}");
            Owner.TextDebug.text = $"Parry: {anim}";
        }
        public override void Update()
        {
            base.Update();

            var animState = Owner.Animator.GetCurrentAnimatorStateInfo(0);
            if (animState.normalizedTime >= 1f)
            {
                Debug.Log($"✅ Parry completed");
                stateMachine.ChangeState(new PlayerIdleState(Owner, stateMachine));
            }
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log($"➡ Exit ParryState → back to Idle");
        }
    }
}

