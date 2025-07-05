using CombatV2.Combat;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAttackHitbox : MonoBehaviour
{
    private AttackData attackData;
    private Transform owner;
    private float timer = 0f;
    private bool active = false;

    private List<PlayerHurtBox> hurtBoxesHit = new List<PlayerHurtBox>();

    public void Initialize(AttackData data, Transform ownerTransform)
    {
        attackData = data;
        owner = ownerTransform;
        timer = 0f;
        active = true;
        hurtBoxesHit.Clear();

        // Set vị trí & kích thước collider
        transform.localPosition = attackData.hitboxOffset;

        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
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

        if (other.CompareTag("PlayerHurtBox"))
        {
            var hurtBox = other.GetComponent<PlayerHurtBox>();
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
            regions.Add(hb.Region); // cần expose Region trong PlayerHurtBox
        }

        HitRegionType chosenRegion = HitRegionResolver.ResolveHitRegion(regions, attackData.gestureRequired);

        var damageable = hurtBoxesHit[0].GetComponentInParent<IAttackable>();
        if (damageable != null)
        {
            damageable.OnHitReceived(attackData, chosenRegion, owner.position);
        }

        Debug.Log($"✅ Enemy hit resolved to: {chosenRegion} from {attackData.attackName}");
    }

    private void DisableHitbox()
    {
        active = false;
        gameObject.SetActive(false);
    }
    public void RegisterHurtBox(PlayerHurtBox hurtBox)
    {
        if (hurtBox != null && !hurtBoxesHit.Contains(hurtBox))
        {
            hurtBoxesHit.Add(hurtBox);
        }
    }

}
