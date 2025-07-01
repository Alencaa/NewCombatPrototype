using UnityEngine;
using CombatV2.FSM;
using System.Collections.Generic;

namespace CombatV2.Enemy
{
    /// <summary>
    /// Enemy thực hiện chuỗi combo tấn công, kiểm tra phản ứng player sau mỗi đòn.
    /// </summary>
    public class EnemyComboAttackState : CharacterState<EnemyController>
    {
        private List<string> comboSteps = new List<string> { "Attack_1", "Attack_2", "Attack_3" };
        private int currentStep = 0;
        private float timeBetweenHits = 0.6f;
        private float hitTimer;

        public EnemyComboAttackState(EnemyController owner, StateMachine<EnemyController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {
            currentStep = 0;
            hitTimer = 0f;
            PlayComboStep();
        }

        public override void Update()
        {
            hitTimer += Time.deltaTime;

            // Sau khi một đòn hoàn tất, chuyển sang đòn tiếp theo
            if (hitTimer >= timeBetweenHits)
            {
                hitTimer = 0f;

                // Kiểm tra phản ứng của Player
                EvaluatePlayerResponse();

                currentStep++;
                if (currentStep < comboSteps.Count)
                {
                    PlayComboStep();
                }
                else
                {
                    // Kết thúc combo → về Idle
                    stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine));
                }
            }
        }

        private void PlayComboStep()
        {
            Owner.PlayAnimation(comboSteps[currentStep]);
            // 👉 Có thể thêm trigger hitbox / VFX tại đây
        }

        /// <summary>
        /// Kiểm tra phản ứng của player sau mỗi đòn trong combo.
        /// </summary>
        private void EvaluatePlayerResponse()
        {
            var player = Owner.player.GetComponent<PlayerCombatMock>(); // tạm class mock để test phản ứng

            if (player == null) return;

            if (player.DidPerfectParryThisFrame)
            {
                // 👉 Parry hoàn hảo → Enemy bị stagger
                Owner.PlayAnimation("Enemy_Stagger");
                stateMachine.ChangeState(new EnemyStaggerState(Owner, stateMachine));
            }
            else if (player.IsBlocking)
            {
                // 👉 Block bình thường → Tiếp tục combo
                PlayBlockClashFeedback();
            }
            else if (player.IsGuardBroken)
            {
                // 👉 Bị phá thủ → Enemy có thể phản công hoặc lùi
                Owner.PlayAnimation("Enemy_ReelBack");
                stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine)); // Tạm thời quay lại Idle
            }
        }

        /// <summary>
        /// Gửi feedback visual/audio cho block clash.
        /// </summary>
        private void PlayBlockClashFeedback()
        {
            // ⚡ TODO: Add clash VFX / âm thanh
            Debug.Log("🔶 Weapon Clash Feedback Played");
        }
    }
}
