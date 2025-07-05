using CombatV2.FSM.States;
using UnityEngine;

public class ComboEventsRelay : MonoBehaviour
{
    private PlayerController player;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void ComboAttackStep(int index)
    {
        if (player.StateMachine.CurrentState is PlayerComboState comboState)
        {
            comboState.ComboAttackStep(index);
        }
    }
}
