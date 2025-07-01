using CombatV2.Combat;
using CombatV2.FSM;
using CombatV2.FSM.States;
using System.Collections;
using UnityEngine;

public class FinisherState : CharacterState<PlayerController>
{
    public FinisherState(PlayerController owner, StateMachine<PlayerController> stateMachine) : base(owner, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entered Finisher");

        // TODO: Chơi animation kết liễu, disable input...
        Owner.StartCoroutine(DoFinisher());
    }

    private IEnumerator DoFinisher()
    {
        // giả lập finisher delay
        yield return new WaitForSeconds(1.0f);

        // Reset combo sau khi kết thúc
        var combo = Owner.GetComponent<ComboTracker>();
        combo?.ResetCombo();

        // Về lại idle
        stateMachine.ChangeState(new IdleState(Owner, stateMachine));
    }
}
