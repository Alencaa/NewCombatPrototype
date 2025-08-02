using CombatV2.Combat;
using UnityEngine;

namespace CombatV2.FSM.States
{
    public class PlayerHitReactState : CharacterState<PlayerController>
    {
        private readonly AttackData attackData;
        private readonly HitRegionType hitRegion;

        private float timer = 0f;
        private bool knockbackApplied = false;

        public PlayerHitReactState(PlayerController owner, StateMachine<PlayerController> stateMachine, AttackData data, HitRegionType region)
            : base(owner, stateMachine)
        {
            attackData = data;
            hitRegion = region;
        }

        public override void Enter()
        {
            base.Enter();

            Owner.Animator.Play("HitReact");
            Owner.TextDebug.text = $"HIT REACT: {hitRegion}";
            Debug.Log($"😖 Player hit (normal): {attackData.attackName}");

            Owner.IsInvincible = true;
        }

        public override void Update()
        {
            base.Update();

            timer += Time.deltaTime;

            if (!knockbackApplied)
            {
                ApplyKnockback();
                knockbackApplied = true;
            }

            if (timer >= Owner.PlayerCombatConfig.invincibilityDuration)
            {
                Owner.IsInvincible = false;
                stateMachine.ChangeState(new PlayerIdleState(Owner, stateMachine));
            }
        }

        private void ApplyKnockback()
        {
            Vector2 attackerPos = attackData != null ? Owner.lastAttackerPosition : Owner.transform.position;
            Vector2 dir = new Vector2(
                  Owner.transform.position.x - attackerPos.x,
                  0f
              ).normalized;
            float force = attackData != null ? attackData.knockbackForce : 1.2f;
            Owner.transform.position += (Vector3)(dir * 0);
        }

        public override void Exit()
        {
            base.Exit();
            Owner.IsInvincible = false;
            Debug.Log("➡ Exit HitReact → back to Idle");
        }
    }
}
