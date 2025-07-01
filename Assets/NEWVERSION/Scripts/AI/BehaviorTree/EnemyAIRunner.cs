using CombatV2.Combat;
using UnityEngine;

/// <summary>
/// Mock AI runner for enemy to simulate basic combat behavior
/// </summary>
public class EnemyAIRunner : MonoBehaviour
{
    public Transform player;               // Reference to player
    public float attackRange = 2f;         // Distance to trigger attack
    public float attackCooldown = 1.5f;    // Delay between attacks

    private float lastAttackTime;
    private Animator animator;
    private CombatExecutor executor;

    private enum EnemyState
    {
        Idle,
        Attacking
    }

    private EnemyState currentState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        executor = GetComponent<CombatExecutor>();
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                currentState = EnemyState.Attacking;

                // Option 1: Trigger combat executor if available
                if (executor != null)
                {
                    executor.ExecuteAttack();  // your custom attack logic
                }
                else
                {
                    animator.SetTrigger("Attack");  // fallback to animation
                }
            }
        }
        else
        {
            currentState = EnemyState.Idle;
            animator.Play("Idle");
        }
    }
}
