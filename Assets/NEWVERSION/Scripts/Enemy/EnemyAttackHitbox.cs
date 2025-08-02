using CombatV2.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyAttackHitbox : MonoBehaviour
{
    private AttackData attackData;
    private Transform owner;
    private bool active = false;
    private bool resolved = false;

    private List<PlayerHurtBox> hurtBoxesHit = new List<PlayerHurtBox>();

    public void Initialize(AttackData data, Transform ownerTransform)
    {
        attackData = data;
        owner = ownerTransform;
        active = true;
        resolved = false;
        hurtBoxesHit.Clear();

        transform.localPosition = attackData.hitboxOffset;

        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = attackData.hitboxSize;
        }

        gameObject.SetActive(true);
        StartCoroutine(AutoDisableIfNoHit(attackData.activeTime));
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!active || resolved) return;

        if (other.CompareTag("PlayerHurtBox"))
        {
            var hurtBox = other.GetComponent<PlayerHurtBox>();
            if (hurtBox != null && !hurtBoxesHit.Contains(hurtBox))
            {
                hurtBoxesHit.Add(hurtBox);

                if (hurtBoxesHit.Count == 1)
                    StartCoroutine(ResolveNextFrame());
            }
        }
    }

    private IEnumerator ResolveNextFrame()
    {
        yield return null; // Chờ 1 frame để gom hết hurtbox
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
