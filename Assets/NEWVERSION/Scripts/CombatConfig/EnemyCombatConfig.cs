using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/EnemyCombatConfig")]
public class EnemyCombatConfig : ScriptableObject
{
    [Tooltip("Danh sách các animation combo")]
    public List<string> comboPattern;

    [Header("Posture & Guard")]
    [Tooltip("Tổng posture của enemy trước khi guard break")]
    public float maxPosture = 100f;

    [Tooltip("Ngưỡng posture ≤ sẽ trigger Guard Break")]
    public float guardBreakThreshold = 0f;

    [Tooltip("Tốc độ hồi posture mỗi giây khi không chặn")]
    public float postureRegenRate = 10f;

    [Header("Parry Settings")]
    [Tooltip("Khoảng thời gian cho phép parry chính xác")]
    public float parryWindowDuration = 0.3f;

    [Tooltip("Thời gian delay giữa parry và stagger")]
    public float parriedToStaggerDelay = 0.4f;

    [Header("Stagger Settings")]
    [Tooltip("Thời gian enemy bị choáng")]
    public float staggerDuration = 0.5f;

    [Header("Damage Multipliers")]
    [Tooltip("Hệ số damage từ đòn heavy khi tính posture")]
    public float heavyAttackPostureMultiplier = 2f;

    [Tooltip("Hệ số sát thương khi enemy bị parry")]
    public float parryDamageMultiplier = 1.5f;


    [Header("AI Logic")]
    public float detectionRange = 3f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;

    public float comboInterval = 0.8f;
    public float blockFeedbackDuration = 0.08f;
    public float clashSlowTimeScale = 0.05f;
}
