using CombatV2.Enemy;
using CombatV2.FSM;
using UnityEngine;

// 🗡️ Trạng thái tấn công người chơi
public class EnemyAttackState : CharacterState<EnemyController>
{
    private float attackCooldown = 1.5f;
    private float lastAttackTime;

    public EnemyAttackState(EnemyController owner, StateMachine<EnemyController> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Enemy: Entering ATTACK state");
        Owner.PlayAnimation("AttackDown");
        lastAttackTime = Time.time;
    }

    public override void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Owner.PlayAnimation("AttackDown");
            lastAttackTime = Time.time;
        }

        // Nếu player chạy quá xa thì quay lại đuổi
        if (Vector2.Distance(Owner.transform.position, Owner.player.position) > 2f)
        {
            stateMachine.ChangeState(new EnemyChaseState(Owner, stateMachine));
        }
    }
}
