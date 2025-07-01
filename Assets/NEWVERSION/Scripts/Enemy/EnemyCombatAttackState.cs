using CombatV2.Enemy;
using CombatV2.FSM;
using UnityEngine;

/// <summary>
/// EnemyCombatAttackState xử lý hành vi tấn công của enemy (đơn hoặc combo),
/// và đánh giá phản ứng của người chơi (block, parry, dính đòn).
/// </summary>
public class EnemyCombatAttackState : CharacterState<EnemyController>
{
    private int currentStep = 0;
    private float hitTimer = 0f;

    // ⚙️ Tham số cấu hình
    private float comboInterval => Owner.config.comboInterval;
    private float blockFeedbackDuration => Owner.config.blockFeedbackDuration;
    private float timeScaleDuringClash => Owner.config.clashSlowTimeScale;

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

        if (hitTimer >= comboInterval)
        {
            EvaluatePlayerResponse(); // ⚔️ đánh giá phản ứng của người chơi
            currentStep++;

            if (currentStep >= Owner.comboPattern.Count)
            {
                Owner.TransitionToIdle(stateMachine); // Dùng hàm coroutine chuyển state
                return;
            }

            PlayComboStep();
        }
    }

    /// <summary>Đòn đánh đơn, không phải combo.</summary>
    private void PlaySingleAttack()
    {
        Owner.animator.Play("Attack1");

        EvaluatePlayerResponse();

        // Sau đòn đơn → idle
        Owner.TransitionToIdle(stateMachine, delay: 1.2f); // dùng coroutine cho delay mượt mà
    }

    /// <summary>Phát combo animation tiếp theo.</summary>
    private void PlayComboStep()
    {
        if (currentStep >= Owner.comboPattern.Count) return;

        string animName = Owner.comboPattern[currentStep];
        Owner.animator.Play(animName);
        hitTimer = 0f;
    }

    /// <summary>Đánh giá phản ứng của player: block, parry hay dính đòn.</summary>
    private void EvaluatePlayerResponse()
    {
        if (Owner.player == null) return;

        var playerController = Owner.player.GetComponent<PlayerController>();
        if (playerController == null) return;

        Vector2 attackDir = (Owner.player.position - Owner.transform.position).normalized;
        Vector2 playerBlockDir = playerController.CurrentBlockDirection;

        if (playerController.IsParrying())
        {
            stateMachine.ChangeState(new EnemyStaggerState(Owner, stateMachine));
            Debug.Log("🔺 Enemy bị parry!");
            return;
        }

        if (playerController.IsBlocking() && Vector2.Dot(attackDir, playerBlockDir) > 0.7f)
        {
            BlockClashFeedback();
            Debug.Log("🛡️ Enemy attack bị block.");
            return;
        }

        // Trúng đòn nếu không block/parry
        playerController.TakeDamage(10); // sau này có thể lấy từ combo damage config
        Debug.Log("💥 Player dính đòn.");
    }

    /// <summary>Gây hiệu ứng khi bị block như slow-motion, VFX,...</summary>
    private void BlockClashFeedback()
    {
        // VFXManager.Instance?.PlaySparkVFX(Owner.attackPoint.position);
        // AudioManager.Instance?.Play("Clash");

        Debug.Log("✨ Clash Feedback Triggered");

        Time.timeScale = timeScaleDuringClash;
        Owner.StartCoroutine(Owner.WaitAndDo(blockFeedbackDuration, Owner.ResetTimeScale));
    }
}
