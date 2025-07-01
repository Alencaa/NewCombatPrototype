using CombatV2.Enemy;
using CombatV2.FSM;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// Xử lý tấn công (đánh đơn hoặc combo), đồng thời đánh giá phản ứng từ người chơi.
/// </summary>
public class EnemyCombatAttackState : CharacterState<EnemyController>
{
    private int currentStep = 0;
    private float hitTimer = 0f;
    private bool isCombo => Owner.isComboEnemy;

    public EnemyCombatAttackState(EnemyController owner, StateMachine<EnemyController> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        currentStep = 0;
        hitTimer = 0f;

        if (isCombo)
            PlayComboStep();
        else
            PlaySingleAttack();
    }

    public override void Update()
    {
        hitTimer += Time.deltaTime;

        if (!isCombo) return;

        if (hitTimer >= 0.8f)
        {
            EvaluatePlayerResponse(); // Kiểm tra phản ứng người chơi với từng đòn

            currentStep++;

            if (currentStep >= Owner.comboPattern.Count)
            {
                stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine));
                return;
            }

            PlayComboStep();
        }
    }

    private void PlaySingleAttack()
    {
        Owner.PlayAnimation("Attack1");

        EvaluatePlayerResponse(); // Đòn đơn cũng cần check

        stateMachine.ChangeState(new EnemyIdleState(Owner, stateMachine), 2f);
    }

    private void PlayComboStep()
    {
        if (currentStep >= Owner.comboPattern.Count) return;

        string anim = Owner.comboPattern[currentStep];
        Owner.PlayAnimation(anim);

        hitTimer = 0f;
    }

    /// <summary>
    /// Kiểm tra xem player có đang parry/block hay không.
    /// </summary>
    private void EvaluatePlayerResponse()
    {
        if (Owner.player == null) return;

        var playerController = Owner.player.GetComponent<PlayerController>();
        if (playerController == null) return;

        Vector2 attackDir = (Owner.player.position - Owner.transform.position).normalized;
        Vector2 playerBlockDir = playerController.CurrentBlockDirection;

        if (playerController.IsParrying())
        {
            // Player parry đúng hướng
            stateMachine.ChangeState(new EnemyStaggerState(Owner, stateMachine));
            Debug.Log("Enemy bị parry!");
            return;
        }

        if (playerController.IsBlocking() && Vector2.Dot(attackDir, playerBlockDir) > 0.7f)
        {
            BlockClashFeedback();
            Debug.Log("Đòn bị block.");
            return;
        }

        // Nếu không parry/block → dính đòn
        playerController.TakeDamage(10); // hoặc tuỳ theo damage mỗi comboStep
        Debug.Log("Player dính đòn.");
    }

    /// <summary>
    /// Hiệu ứng khi bị block: spark, slow-mo, âm thanh.
    /// </summary>
    private void BlockClashFeedback()
    {
        //if (Owner.attackPoint != null)
        //{
        //    VFXManager.Instance?.PlaySparkVFX(Owner.attackPoint.position);
        //}

        //AudioManager.Instance?.Play("Clash");
        Debug.Log("Block clash effect!");
        Time.timeScale = 0.05f;
        Owner.Invoke("ResetTimeScale", 0.08f); // Khôi phục thời gian
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
}
