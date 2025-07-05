using CombatV2.Combat;
using CombatV2.Player;
using System.Collections;
using UnityEngine;

namespace CombatV2.FSM.States
{
    public class PlayerAttackState : CharacterState<PlayerController>
    {
        private readonly GestureData gesture;
        private AttackData attackData;

        private enum Phase { WindUp, AttackActive, Recovery }
        private Phase currentPhase;

        private float phaseTimer = 0f;
        private bool hitboxSpawned = false;
        public bool IsAcceptingBufferedInput => currentPhase != Phase.AttackActive;

        public PlayerAttackState(PlayerController owner, StateMachine<PlayerController> stateMachine, GestureData gesture) : base(owner, stateMachine)
        {
            this.gesture = gesture;
        }

        public override void Enter()
        {
            base.Enter();

            attackData = Owner.PlayerCombatConfig.GetAttackDataForGesture(gesture);
            if (attackData == null)
            {
                Debug.LogWarning($"❌ No AttackData found for gesture {gesture.type}");
                stateMachine.ChangeState(new PlayerIdleState(Owner, stateMachine));
                return;
            }

            currentPhase = Phase.WindUp;
            phaseTimer = 0f;
            hitboxSpawned = false;
            Owner.IsInWindUp = true;
            Owner.Animator.Play(attackData.animationName, -1, 0);
            Debug.Log($"▶ FSM Attack → {attackData.animationName}");
            Owner.TextDebug.text = $"Attack: {attackData.animationName}";
            Owner.TextDebug.text = "WIND UP";
        }

        public override void Update()
        {
            base.Update();
            Time.timeScale = 0.5f;
            phaseTimer += Time.deltaTime;

            switch (currentPhase)
            {
                case Phase.WindUp:
                    if (phaseTimer >= attackData.windUpTime)
                    {
                        currentPhase = Phase.AttackActive;
                        Owner.TextDebug.text = "ATTACK ACTIVE";
                        phaseTimer = 0f;
                        Owner.IsInWindUp = false;
                    }
                    break;

                case Phase.AttackActive:
                    if (!hitboxSpawned)
                    {
                        SpawnHitbox();
                        hitboxSpawned = true;
                    }
                    if (phaseTimer >= attackData.activeTime)
                    {
                        currentPhase = Phase.Recovery;
                        Owner.TextDebug.text = "RECOVERY COMPLETE";
                        phaseTimer = 0f;
                    }
                    break;

                case Phase.Recovery:

                    if (Owner.InputBuffer.HasGesture)
                    {
                        GestureData buffered = Owner.InputBuffer.ConsumeGesture();
                        Debug.Log($"🔁 Buffered gesture consumed: {buffered.type}");
                        stateMachine.ChangeState(new PlayerAttackState(Owner, stateMachine, buffered));
                    }
                    else
                    {
                        Debug.Log($"✅ Attack completed: {gesture.type}");
                        stateMachine.ChangeState(new PlayerIdleState(Owner, stateMachine));
                    }
                    break;
            }
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("➡ Exit AttackState → back to Idle");
        }

        private void SpawnHitbox()
        {
            // TODO: instantiate prefab and configure using attackData
            Owner.SlashHitbox.Initialize(attackData, Owner.transform);
            Debug.Log($"💥 Hitbox spawned for {attackData.attackName}");
        }
    }
}
