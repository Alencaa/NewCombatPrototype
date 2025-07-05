using UnityEngine;

namespace CombatV2.FSM.States
{
    public class PlayerBlockState : CharacterState<PlayerController>
    {
        private Vector2 lastDirection;
        private float directionUpdateThreshold = 15f; // degrees

        public PlayerBlockState(PlayerController owner, StateMachine<PlayerController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {
            base.Enter();
            lastDirection = Owner.CurrentBlockDirection;
            PlayBlockAnimation(lastDirection);
            //Owner.ShowBlockArrow(Owner.CurrentBlockDirection);

            Owner.TextDebug.text = $"Block: {Owner.GetDirectionName(lastDirection)}";

            Debug.Log($"🛡 Enter BlockState: {Owner.GetDirectionName(lastDirection)}");
        }

        public override void Update()
        {
            base.Update();

            if (!Owner.IsHoldingBlock)
            {
                stateMachine.ChangeState(new PlayerIdleState(Owner, stateMachine));
                return;
            }

            Vector2 newDir = Owner.CurrentBlockDirection;
            //Owner.RotateArrow(newDir);

            if (Vector2.Angle(lastDirection, newDir) > directionUpdateThreshold)
            {
                lastDirection = newDir;
                PlayBlockAnimation(newDir);
                Debug.Log($"↔ Update Block Dir: {Owner.GetDirectionName(newDir)}");
                Owner.TextDebug.text = $"Block: {Owner.GetDirectionName(newDir)}";
            }
        }

        public override void Exit()
        {
            base.Exit();
            Owner.Animator.Play("Idle");
            Debug.Log("⬅ Exit BlockState");
           // Owner.HideBlockArrow();
        }

        private void PlayBlockAnimation(Vector2 dir)
        {
            string dirName = Owner.GetDirectionName(dir);
            Owner.Animator.Play($"Block_{dirName}");
        }
    }
}

