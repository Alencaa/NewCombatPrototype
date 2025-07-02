using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedCombo
{
    public string comboName;
    public List<GestureType> pattern;
}

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

    [Header("Combo")]
    public int maxComboSteps = 3; // Maximum number of combo steps
    public List<NamedCombo> combos;
    public float maxComboInterval = 0.5f; // giây giữa swipe

    // Add more as needed
}
