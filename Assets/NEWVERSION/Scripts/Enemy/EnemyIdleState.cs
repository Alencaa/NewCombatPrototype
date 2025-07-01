using CombatV2.Enemy;
using CombatV2.FSM;
using UnityEngine;

// 😐 Trạng thái chờ (Idle)
public class EnemyIdleState : CharacterState<EnemyController>
{
    private float waitTime = 1.5f;
    private float timer;

    public EnemyIdleState(EnemyController owner, StateMachine<EnemyController> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Enemy: Entering IDLE state");
        Owner.PlayAnimation("Idle");
        timer = 0f;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            stateMachine.ChangeState(new EnemyChaseState(Owner, stateMachine));
        }
    }
}
