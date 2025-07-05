using CombatV2.Combat;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
//public enum HitRegionType
//{
//    Head,
//    Body,
//    Leg
//}

[RequireComponent(typeof(Collider2D))]
public class PlayerHurtBox : MonoBehaviour
{
    [SerializeField] private HitRegionType hitRegion;
    public HitRegionType Region => hitRegion;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("EnemyAttackHitbox")) return;

        var enemyHitbox = other.GetComponent<EnemyAttackHitbox>()
                         ?? other.GetComponentInParent<EnemyAttackHitbox>()
                         ?? other.GetComponentInChildren<EnemyAttackHitbox>();
        if (enemyHitbox == null) return;
        enemyHitbox.RegisterHurtBox(this);

    }

    public void OnHitByEnemy(AttackData data, Vector2 attackerPos)
    {
        var damageable = GetComponentInParent<IAttackable>();
        if (damageable != null)
        {
            damageable.OnHitReceived(data, hitRegion, attackerPos);
        }
    }
}
