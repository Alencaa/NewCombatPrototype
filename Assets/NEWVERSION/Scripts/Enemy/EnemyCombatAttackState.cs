using CombatV2.Combat;
using CombatV2.Enemy;
using CombatV2.FSM;
using UnityEngine;
using System.Collections.Generic;

public class EnemyCombatAttackState : CharacterState<EnemyController>
{
    private int currentStep = 0;
    private float phaseTimer = 0f;
    private AttackData currentAttack;

    private enum Phase { WindUp, AttackActive, Recovery }
    private Phase currentPhase;

    private List<AttackData> comboSteps;
    private bool isCombo => Owner.isComboEnemy;

    public EnemyCombatAttackState(EnemyController owner, StateMachine<EnemyController> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        comboSteps = Owner.config.enemyComboData;

        currentStep = 0;
        BeginStep();
    }

    public override void Update()
    {
        phaseTimer += Time.deltaTime;

        switch (currentPhase)
        {
            case Phase.WindUp:
                if (phaseTimer >= currentAttack.windUpTime)
                {
                    currentPhase = Phase.AttackActive;
                    phaseTimer = 0f;
                    Owner.combat.SpawnHitbox(currentAttack, Owner.transform); 
                    Owner.IsInWindUp = false; // reset windup state
                    EvaluatePlayerGesture();
                    Debug.Log("🔸 Enemy AttackActive");
                }
                break;

            case Phase.AttackActive:
                if (phaseTimer >= currentAttack.activeTime)
                {
                    currentPhase = Phase.Recovery;
                    phaseTimer = 0f;
                    Debug.Log("🔻 Enemy Recovery");
                }
                break;

            case Phase.Recovery:
                if (phaseTimer >= currentAttack.recoveryTime)
                {
                    currentStep++;
                    if (currentStep >= comboSteps.Count)
                    {
                        Owner.TransitionToIdle(stateMachine); // end combo
                    }
                    else
                    {
                        BeginStep(); // next attack step
                    }
                }
                break;
        }
    }

    private void BeginStep()
    {
        currentAttack = comboSteps[currentStep];
        currentPhase = Phase.WindUp;
        phaseTimer = 0f;

        Owner.IsInWindUp = true;
        Owner.animator.Play(currentAttack.animationName);

        Debug.Log($"▶ Enemy Attack Step {currentStep} → {currentAttack.attackName}");
    }

    public override void Exit()
    {
        base.Exit();
        Owner.IsInWindUp = false;
    }
    private void EvaluatePlayerGesture()
    {
        var player = Owner.player?.GetComponent<PlayerController>();
        if (player == null || player.IsInvincible) return;

        var gesture = currentAttack.gestureRequired;

        if (player.IsParrying())
        {
            if (player.LastParryGesture == gesture)
            {
                Debug.Log("⚡ Super Parry!");
                player.OnSuperParry(currentAttack);
                return;
            }
            else
            {
                Debug.Log("⛔ Wrong-direction parry → posture damage");
                player.OnFailedParry(currentAttack);
                return;
            }
        }

        if (player.IsBlocking())
        {
            Vector2 expectedDir = GetDirectionFromGesture(currentAttack.gestureRequired);
            if (Vector2.Dot(player.CurrentBlockDirection.normalized, expectedDir.normalized) > 0.9f)
            {
                player.OnBlocked(currentAttack);
            }
            else
            {
                Debug.Log("💢 Block in wrong direction → hit");
                player.OnHitReceived(currentAttack, HitRegionType.Body, Owner.transform.position);
            }

            return;
        }

        // Nếu không parry hoặc block gì cả
        Debug.Log("💀 Hit landed clean");
        player.OnHitReceived(currentAttack, HitRegionType.Body, Owner.transform.position);
    }
    private Vector2 GetDirectionFromGesture(GestureType gesture)
    {
        return gesture switch
        {
            GestureType.SlashUp => Vector2.up,
            GestureType.SlashDown => Vector2.down,
            GestureType.SlashLeft => Vector2.left,
            GestureType.SlashRight => Vector2.right,
            _ => Vector2.zero
        };
    }
}
