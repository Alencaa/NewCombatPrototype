using CombatV2.Combat;
using CombatV2.FSM;
using CombatV2.FSM.States;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private StateMachine<PlayerController> fsm;

    public bool HasFinishedAttackAnim => true; // TEMP stub
    public InputInterpreter InputInterpreter { get; private set; }

    void Awake()
    {
        if (InputInterpreter == null)
        {
            Debug.LogError("InputInterpreter component is missing on PlayerController.");
            InputInterpreter = GetComponent<InputInterpreter>();

        }
    }
    void Start()
    {
        fsm = new StateMachine<PlayerController>();
        fsm.Initialize(new IdleState(this, fsm));

    }

    void Update()
    {
        fsm.Update();
        GetComponent<CombatExecutor>().ExecuteAttack();


    }

    void FixedUpdate()
    {
        fsm.FixedUpdate();
    }
}
