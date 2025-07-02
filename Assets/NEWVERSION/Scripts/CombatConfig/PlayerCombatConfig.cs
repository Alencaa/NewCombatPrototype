using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCombatConfig", menuName = "Combat/PlayerConfig")]
public class PlayerCombatConfig : ScriptableObject
{
    [Header("Posture")]
    public float maxPosture = 100f;
    public float postureRegenRate = 5f;

    [Header("Attack")]
    public float lightAttackDamage = 10f;
    public float heavyAttackDamage = 20f;

    [Header("Parry")]
    public float parryWindow = 0.3f;
    public float parryStaggerDuration = 0.5f;

    [Header("Animation")]
    public float attackMoveSpeed = 2f;
    public float comboResetTime = 1.5f;

    // Add more as needed
}
