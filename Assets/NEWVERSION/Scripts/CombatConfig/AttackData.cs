using UnityEngine;
namespace CombatV2.Combat
{
    public enum CounterHitType { None, Head, Body, Leg }

    [CreateAssetMenu(fileName = "NewAttackData", menuName = "Combat/Attack Data")]
    public class AttackData : ScriptableObject
    {
        [Header("General Info")]
        public string attackName;
        public GestureType gestureRequired;
        public int damage = 10;

        [Header("Phases Timing")]
        public float windUpTime = 0.15f;
        public float activeTime = 0.1f;
        public float recoveryTime = 0.3f;
        public bool canCancelDuringRecovery = true;

        [Header("Hitbox Config")]
        public Vector2 hitboxOffset;
        public Vector2 hitboxSize;

        [Header("Animation / FX")]
        public string animationName;
        public AudioClip sfx;
        public GameObject vfxPrefab;

        [Header("Combat Reaction")]
        public CounterHitType counterHitType = CounterHitType.Body;
    }

}
