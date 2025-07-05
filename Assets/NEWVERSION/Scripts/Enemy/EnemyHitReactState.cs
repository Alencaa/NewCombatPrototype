using CombatV2.Combat;
using CombatV2.Enemy;
using UnityEngine;

namespace CombatV2.FSM.States
{
    public class EnemyHitReactState : CharacterState<EnemyController>
    {
        private readonly AttackData attackData;
        private readonly HitRegionType hitRegion;

        private float timer = 0f;
        private float duration = 0.4f;
        private bool knockbackApplied = false;

        public EnemyHitReactState(EnemyController owner, StateMachine<EnemyController> stateMachine, AttackData data, HitRegionType region)
            : base(owner, stateMachine)
        {
            attackData = data;
            hitRegion = region;
        }

        public override void Enter()
        {
            base.Enter();

            Owner.animator.Play("HitReact");
            Owner.IsInvicible = true;

            // Bạn có thể điều chỉnh duration theo EnemyCombatConfig nếu muốn
            Debug.Log($"😣 Enemy hit (normal): {attackData.attackName} on {hitRegion}");
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

            if (timer >= duration)
            {
                Owner.IsInvicible = false;
                stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine));
            }
        }

        private void ApplyKnockback()
        {
            Vector2 attackerPos = Owner.lastAttackerPosition;
            Vector2 dir = (Owner.transform.position - (Vector3)attackerPos).normalized;
            float force = 1.2f;

            Owner.transform.position += (Vector3)(dir * force);
        }

        public override void Exit()
        {
            base.Exit();
            Owner.IsInvicible = false;
            Debug.Log("➡ Enemy exits HitReact → back to Idle");
        }
    }
}
