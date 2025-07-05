using CombatV2.Combat;
using UnityEngine;

public class EnemyCombatHandler : MonoBehaviour
{
    [SerializeField] private EnemyAttackHitbox hitbox; // Gắn sẵn trong prefab
 
    public void TriggerStaggerFeedback()
    {
        // Add VFX, SFX, animation trigger etc.
        Debug.Log("Stagger feedback triggered!");
    }

    public void TriggerParryFeedback()
    {
        Debug.Log("Parry feedback triggered!");
    }
    public void SpawnHitbox(AttackData data, Transform owner)
    {
        if (hitbox != null)
        {
            hitbox.Initialize(data, owner);
        }
    }
    public void OnHitReceived(int damage, Vector2 attackerPosition, CounterHitType hitType)
    {
        Debug.Log($"💢 Enemy hit! Damage: {damage}, CH Type: {hitType}");

        // TODO:
        // - Subtract HP
        // - Trigger stagger / knockdown / stun theo hitType
        // - Play animation / flash / SFX
    }
}
