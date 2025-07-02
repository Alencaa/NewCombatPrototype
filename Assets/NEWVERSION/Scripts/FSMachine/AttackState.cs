using CombatV2.Combat;
using UnityEngine;
using System.Collections;
using CombatV2.Player;

namespace CombatV2.FSM.States
{
    public class AttackState : CharacterState<PlayerController>
    {
        public AttackState(PlayerController owner, StateMachine<PlayerController> stateMachine)
            : base(owner, stateMachine) { }

        public override void Enter()
        {

            Debug.Log("Entered Attack");

            //var combat = Owner.GetComponent<CombatExecutor>();
            //if (combat != null)
            //{
            //    combat.ExecuteAttack(); // đồng thời gọi comboTracker.RegisterHit()
            //}

            //// Check combo logic
            //var combo = Owner.GetComponent<ComboTracker>();
            //if (combo != null)
            //{
            //    if (combo.IsFinisherReady())
            //    {
            //        Debug.Log("Combo max reached → switch to FinisherState");
            //        stateMachine.ChangeState(new FinisherState(Owner, stateMachine));
            //        return;
            //    }
            //}

            //// Nếu không đủ combo, tự động quay về Idle sau 0.5s
            //Owner.StartCoroutine(BackToIdleAfterDelay(0.5f));
        }
        private IEnumerator BackToIdleAfterDelay(float time)
        {
            yield return new WaitForSeconds(time);
            stateMachine.ChangeState(new IdleState(Owner, stateMachine));
        }
        public override void Update()
        {
            // TEMP: Simulate attack complete
            //if (Owner.HasFinishedAttackAnim)
            //{
            //    stateMachine.ChangeState(new IdleState(Owner, stateMachine));
            //}
        }
    }
}
