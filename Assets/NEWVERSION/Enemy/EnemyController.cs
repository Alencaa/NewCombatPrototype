using UnityEngine;
using CombatV2.Combat;

public class EnemyController : MonoBehaviour, IAttackable
{
    public int health = 100;
    public float posture = 50f;
    public bool isBlocking = false;
    public Vector2 blockDirection = Vector2.right;

    public void ReceiveHit(HitPayload payload)
    {
        Vector2 hitDir = payload.direction.normalized;

        if (isBlocking)
        {
            float dot = Vector2.Dot(blockDirection.normalized, -hitDir);

            if (dot > 0.7f)
            {
                // Block success
                Debug.Log("Enemy BLOCKED the attack");

                posture -= payload.postureDamage;

                if (posture <= 0)
                {
                    Debug.Log("Guard Break! Enemy staggered");
                    // TODO: Trigger Stagger State / animation
                }

                return;
            }
        }

        // If not blocked:
        health -= payload.damage;
        posture -= payload.postureDamage;

        Debug.Log($"Enemy TOOK DAMAGE: {payload.damage} | Posture: {posture}");

        if (health <= 0)
        {
            Debug.Log("Enemy DIED");
            // TODO: Trigger Death
        }
        FindFirstObjectByType<CombatFeedback>()?.PlayFeedback(transform.position);

    }
}
