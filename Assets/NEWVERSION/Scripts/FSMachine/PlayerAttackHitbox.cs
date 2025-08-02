using CombatV2.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerAttackHitbox : MonoBehaviour
{
    private AttackData attackData;
    private Transform owner;
    private bool active = false;
    private bool resolved = false;

    private List<EnemyHurtBox> hurtBoxesHit = new List<EnemyHurtBox>();

    public void Initialize(AttackData data, Transform ownerTransform)
    {
        attackData = data;
        owner = ownerTransform;
        active = true;
        resolved = false;
        hurtBoxesHit.Clear();

        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.offset = attackData.hitboxOffset;
            col.size = attackData.hitboxSize;
        }

        gameObject.SetActive(true);

        StartCoroutine(AutoDisableIfNoHit(attackData.activeTime));
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!active || resolved) return;

        if (other.CompareTag("EnemyHurtBox"))
        {
            var hurtBox = other.GetComponent<EnemyHurtBox>();
            if (hurtBox != null && !hurtBoxesHit.Contains(hurtBox))
            {
                hurtBoxesHit.Add(hurtBox);

                // Nếu đây là lần đầu tiên trúng => chờ 1 frame rồi resolve
                if (hurtBoxesHit.Count == 1)
                    StartCoroutine(ResolveNextFrame());
            }
        }
    }

    private IEnumerator ResolveNextFrame()
    {
        yield return null; // chờ 1 frame để các trigger khác được xử lý
        ResolveHit();
        DisableHitbox();
    }

    private IEnumerator AutoDisableIfNoHit(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (!resolved)
            DisableHitbox();
    }

    private void ResolveHit()
    {
        if (resolved || hurtBoxesHit.Count == 0) return;

        resolved = true;

        List<HitRegionType> regions = new List<HitRegionType>();
        foreach (var hb in hurtBoxesHit)
        {
            regions.Add(hb.Region);
        }

        HitRegionType chosenRegion = HitRegionResolver.ResolveHitRegion(regions, attackData.gestureRequired);
        var damageable = hurtBoxesHit[0].GetComponentInParent<IAttackable>();
        if (damageable != null)
        {
            damageable.OnHitReceived(attackData, chosenRegion, owner.position);
        }

        Debug.Log($"✅ Player hit resolved to: {chosenRegion} from {attackData.attackName}");
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
