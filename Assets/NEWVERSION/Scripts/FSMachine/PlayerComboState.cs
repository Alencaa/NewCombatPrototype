using CombatV2.Combat;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CombatV2.FSM.States
{
    public class PlayerComboState : CharacterState<PlayerController>
    {
        private readonly string comboName;
        private readonly List<GestureData> comboSteps;
        private List<AttackData> attackSteps;


        public PlayerComboState(PlayerController owner, StateMachine<PlayerController> stateMachine, string comboName, List<GestureData> steps)
            : base(owner, stateMachine)
        {
            this.comboName = comboName;
            this.comboSteps = steps;
        }

        public override void Enter()
        {
            base.Enter();

            string animName = $"Combo_{comboName}";
            Owner.Animator.Play(animName);
            Debug.Log($"💥 FSM Combo → Play {animName} ({comboSteps.Count} steps)");
            Owner.TextDebug.text = $"COMBO: {comboName}";
            attackSteps = Owner.PlayerCombatConfig.GetComboAttackSteps(comboName)?.ToList();

            if (attackSteps == null || attackSteps.Count == 0)
            {
                Debug.LogWarning($"❌ Combo '{comboName}' has no attack steps!");
            }
        }

        public override void Update()
        {
            base.Update();

            var animState = Owner.Animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName($"Combo_{comboName}") && animState.normalizedTime >= 0.95f)
            {
                if (Owner.InputBuffer.HasGesture)
                {
                    GestureData buffered = Owner.InputBuffer.ConsumeGesture();
                    Debug.Log($"🔁 Combo buffer → next gesture: {buffered.type}");
                    stateMachine.ChangeState(new PlayerAttackState(Owner, stateMachine, buffered));
                }
                else
                {
                    Debug.Log($"✅ Combo {comboName} completed");
                    stateMachine.ChangeState(new PlayerIdleState(Owner, stateMachine));
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log($"➡ Exit ComboState → back to Idle");
        }

        public void ComboAttackStep(int index)
        {
            if (attackSteps == null || index < 0 || index >= attackSteps.Count) return;

            var atk = attackSteps[index];
            Owner.SlashHitbox.Initialize(atk, Owner.transform);
            Debug.Log($"💥 Combo step {index} triggered: {atk.attackName}");
        }

    }
}
