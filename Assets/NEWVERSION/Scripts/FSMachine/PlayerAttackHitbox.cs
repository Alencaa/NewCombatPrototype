using CombatV2.Combat;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerAttackHitbox : MonoBehaviour
{
    private AttackData attackData;
    private Transform owner;
    private float timer = 0f;
    private bool active = false;

    private List<EnemyHurtBox> hurtBoxesHit = new List<EnemyHurtBox>();

    public void Initialize(AttackData data, Transform ownerTransform)
    {
        attackData = data;
        owner = ownerTransform;
        timer = 0f;
        active = true;
        hurtBoxesHit.Clear();

        // Set vị trí và kích thước collider

        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.offset = attackData.hitboxOffset;
            col.size = attackData.hitboxSize;
        }

        gameObject.SetActive(true);
    }

    public AttackData GetAttackData() => attackData;

    private void Update()
    {
        if (!active) return;

        timer += Time.deltaTime;
        if (timer >= attackData.activeTime)
        {
            ResolveHit();
            DisableHitbox();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        if (other.CompareTag("EnemyHurtBox"))
        {
            var hurtBox = other.GetComponent<EnemyHurtBox>();
            if (hurtBox != null && !hurtBoxesHit.Contains(hurtBox))
            {
                hurtBoxesHit.Add(hurtBox);
            }
        }
    }

    private void ResolveHit()
    {
        if (hurtBoxesHit.Count == 0) return;

        List<HitRegionType> regions = new List<HitRegionType>();
        foreach (var hb in hurtBoxesHit)
        {
            regions.Add(hb.Region); // cần expose public HitRegionType Region => hitRegion;
        }

        HitRegionType chosenRegion = HitRegionResolver.ResolveHitRegion(regions, attackData.gestureRequired);

        // Gửi đòn duy nhất vào enemy (tạm chọn enemy đầu tiên trong danh sách)
        var damageable = hurtBoxesHit[0].GetComponentInParent<IAttackable>();
        if (damageable != null)
        {
            damageable.OnHitReceived(attackData, chosenRegion, owner.position);
        }

        Debug.Log($"✅ Hit resolved to: {chosenRegion} from {attackData.attackName}");
    }

    private void DisableHitbox()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public void RegisterHurtBox(EnemyHurtBox hurtBox)
    {
        if (hurtBox != null && !hurtBoxesHit.Contains(hurtBox))
        {
            hurtBoxesHit.Add(hurtBox);
        }
    }
}
