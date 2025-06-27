using UnityEngine;
using CombatV2.Combat;

public class EnemyDummy : MonoBehaviour, IAttackable
{
    public int health = 100;
    public float posture = 50f;

    public void ReceiveHit(HitPayload payload)
    {
        Debug.Log($"EnemyDummy nhận đòn từ {payload.attacker.name} | Type: {payload.hitType} | Damage: {payload.damage}");

        health -= payload.damage;
        posture -= payload.postureDamage;

        if (posture <= 0)
        {
            Debug.Log("EnemyDummy bị gãy posture!");
        }

        if (health <= 0)
        {
            Debug.Log("EnemyDummy bị tiêu diệt!");
            // Destroy(gameObject); // tuỳ muốn
        }
    }
}
