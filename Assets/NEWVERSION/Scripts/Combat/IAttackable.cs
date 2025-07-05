using UnityEngine;

namespace CombatV2.Combat
{
    public interface IAttackable
    {
        void OnHitReceived(AttackData attackData, HitRegionType hitRegion, Vector2 attackerPosition);
    }
}
