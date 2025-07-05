using CombatV2.Combat;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedCombo
{
    public string comboName;
    public List<GestureType> pattern;
    public List<AttackData> attackSteps;
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

    [Header("Stagger Duration (Counter Hit)")]
    public float staggerHeadDuration = 1.0f;
    public float staggerBodyDuration = 0.6f;
    public float staggerLegDuration = 0.8f;
    public float invincibilityDuration = 0.5f; // Thời gian bất khả xâm phạm sau khi bị đánh

    [Header("Animation")]
    public float attackMoveSpeed = 2f;
    public float comboResetTime = 1.5f;

    [Header("Combo")]
    public int maxComboSteps = 3; // Maximum number of combo steps
    public List<NamedCombo> combos;

    public float maxComboInterval = 0.5f; // giây giữa swipe

    [Header("Attack List")]
    public List<AttackData> attacks;

    public AttackData GetAttackDataForGesture(GestureData gesture)
    {
        foreach (var atk in attacks)
        {
            if (atk.gestureRequired == gesture.type)
                return atk;
        }
        Debug.LogWarning($"⚠️ No AttackData matched for gesture: {gesture.type}");
        return null;
    }
    public NamedCombo GetNamedComboByName(string comboName)
    {
        return combos.Find(c => c.comboName == comboName);
    }

    public AttackData[] GetComboAttackSteps(string comboName)
    {
        var combo = GetNamedComboByName(comboName);
        return combo != null ? combo.attackSteps.ToArray() : null;
    }

    // Add more as needed
}
