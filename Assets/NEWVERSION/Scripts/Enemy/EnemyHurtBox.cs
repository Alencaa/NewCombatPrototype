using CombatV2.Combat;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyHurtBox : MonoBehaviour
{
    [SerializeField] private HitRegionType hitRegion;
    public HitRegionType Region => hitRegion;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerAttackHitbox")) return;

        // Tìm PlayerAttackHitbox từ chính object hoặc cha/con
        var playerHitbox = other.GetComponent<PlayerAttackHitbox>()
                         ?? other.GetComponentInParent<PlayerAttackHitbox>()
                         ?? other.GetComponentInChildren<PlayerAttackHitbox>();

        if (playerHitbox == null)
        {
            Debug.LogWarning($"[EnemyHurtBox] Không tìm thấy PlayerAttackHitbox trên object: {other.name}");
            return;
        }
        playerHitbox.RegisterHurtBox(this);
    }
}
