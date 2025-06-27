using UnityEngine;

namespace CombatV2.Combat
{
    public enum HitType { Normal, Heavy, Combo }

    public class HitPayload
    {
        public GameObject attacker;
        public Vector2 direction;
        public HitType hitType;
        public int damage;
        public float postureDamage;
        public bool isParryable;
    }
}
