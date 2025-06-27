using UnityEngine;

namespace CombatV2.Combat
{
    public class CombatExecutor : MonoBehaviour
    {
        public LayerMask hittableLayers;
        public float hitRange = 1.5f;

        public HitType attackType = HitType.Normal;
        public int baseDamage = 10;
        public float postureDamage = 5f;

        public ComboTracker comboTracker;
        public void ExecuteAttack()
        {
            Vector2 origin = (Vector2)transform.position + (Vector2)transform.right * 0.5f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, hitRange, hittableLayers);

            foreach (var hit in hits)
            {
                var target = hit.GetComponent<IAttackable>();
                if (target != null)
                {
                    HitPayload payload = new()
                    {
                        attacker = gameObject,
                        direction = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized,
                        hitType = attackType,
                        damage = baseDamage,
                        postureDamage = postureDamage,
                        isParryable = true
                    };

                    target.ReceiveHit(payload);
                }
            }
            comboTracker.RegisterHit();

            if (comboTracker.IsFinisherReady())
            {
                Debug.Log("READY FOR FINISHER!");
                // chuyển FSM sang FinisherState nếu muốn
            }

            Debug.DrawLine(origin, origin + (Vector2)transform.right * hitRange, Color.red, 0.5f);
        }
    }
}
