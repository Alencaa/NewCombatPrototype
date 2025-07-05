using CombatV2.Combat;
using CombatV2.Enemy;
using UnityEngine;

namespace CombatV2.FSM.States
{
    public class EnemyStaggerState : CharacterState<EnemyController>
    {
        private readonly AttackData attackData;
        private readonly HitRegionType hitRegion;

        private float timer = 0f;
        private float duration;

        public EnemyStaggerState(EnemyController owner, StateMachine<EnemyController> stateMachine, AttackData data, HitRegionType region)
            : base(owner, stateMachine)
        {
            attackData = data;
            hitRegion = region;
        }

        public override void Enter()
        {
            base.Enter();

            // Lấy duration từ EnemyCombatConfig
            duration = Owner.config.staggerDuration;

            switch (hitRegion)
            {
                case HitRegionType.Head:
                    Owner.animator.Play("Enemy_Stagger_Head");
                    break;
                case HitRegionType.Body:
                    Owner.animator.Play("Enemy_Stagger_Body");
                    break;
                case HitRegionType.Leg:
                    Owner.animator.Play("Enemy_Stagger_Leg");
                    break;
            }

            Debug.Log($"😵 Enemy staggered at {hitRegion} by {attackData.attackName}");
        }

        public override void Update()
        {
            base.Update();

            timer += Time.deltaTime;
            if (timer >= duration)
            {
                stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine));
            }
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("➡ Enemy exits Stagger → back to Idle");
        }
    }
}
