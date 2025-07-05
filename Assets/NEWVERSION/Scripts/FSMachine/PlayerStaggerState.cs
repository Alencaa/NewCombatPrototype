using CombatV2.Combat;
using UnityEngine;

namespace CombatV2.FSM.States
{
    public class PlayerStaggerState : CharacterState<PlayerController>
    {
        private readonly AttackData attackData;
        private readonly HitRegionType hitRegion;
        private float timer = 0f;
        private float duration;

        public PlayerStaggerState(PlayerController owner, StateMachine<PlayerController> stateMachine, AttackData data, HitRegionType region)
            : base(owner, stateMachine)
        {
            attackData = data;
            hitRegion = region;
        }

        public override void Enter()
        {
            base.Enter();


            var config = Owner.PlayerCombatConfig;

            switch (hitRegion)
            {
                case HitRegionType.Head:
                    duration = config.staggerHeadDuration;
                    Owner.Animator.Play("Stagger_Head");
                    break;

                case HitRegionType.Body:
                    duration = config.staggerBodyDuration;
                    Owner.Animator.Play("Stagger_Body");
                    break;

                case HitRegionType.Leg:
                    duration = config.staggerLegDuration;
                    Owner.Animator.Play("Stagger_Leg");
                    break;
            }
            Debug.Log($"😵 Player staggered: {hitRegion} [{attackData.attackName}]");
            Owner.TextDebug.text = $"STAGGER: {hitRegion}";
        }

        public override void Update()
        {
            base.Update();

            timer += Time.deltaTime;
            if (timer >= duration)
            {
                stateMachine.ChangeState(new PlayerIdleState(Owner, stateMachine));
            }
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("➡ Exit Stagger → back to Idle");
        }
    }
}
