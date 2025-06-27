using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Hitbox dành riêng cho Player, kế thừa từ BaseAttackHitbox.
/// Nhiệm vụ chính: Thi hành các hậu quả va chạm (Parry, Block, Damage)
/// dựa trên "ý định" đã được PlayerAttack quyết định từ trước.
/// </summary>
public class PlayerAttackHitbox : BaseAttackHitbox
{
    // Biến này sẽ nhận "ý định" từ PlayerAttack.cs
    public AttackIntent intent = AttackIntent.Normal;

    // Ghi đè hàm ResetState của lớp cha để reset thêm 'intent'
    public override void ResetState()
    {
        base.ResetState(); // Gọi hàm của lớp cha để xóa danh sách processedCharacters
        intent = AttackIntent.Normal;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttackActive || ownerStats == null) return;

        CharacterStats targetStats = other.GetComponentInParent<CharacterStats>();
        if (targetStats == null || targetStats == ownerStats || processedCharacters.Contains(targetStats)) return;

        // --- HỆ THỐNG ƯU TIÊN MỚI ---
        EnemyAttackHitbox enemyHitbox = other.GetComponent<EnemyAttackHitbox>();
        Debug.Log("PlayerAttackHitbox OnTriggerStay2D: " + other.name + " | isDealingDamage: " + isDealingDamage + " | isAttackActive: " + isAttackActive);

        // ƯU TIÊN 1: VA CHẠM VỚI VŨ KHÍ CỦA ĐỊCH
        if (enemyHitbox != null)
        {
            // 1A: Nóng vs Nóng -> PARRY
            if (this.isDealingDamage && enemyHitbox.isDealingDamage)
            {
                HandleClash(enemyHitbox, targetStats);
            }
            // 1B: Nóng vs "Đang vung nhưng chưa nóng" -> BỊ CHẶN (BLOCK)
            else if (this.isDealingDamage && enemyHitbox.isAttackActive)
            {
                HandleBlock(enemyHitbox, targetStats);
            }
        }
        // ƯU TIÊN 2: VA CHẠM VỚI CƠ THỂ ĐỊCH
        else if (other.GetComponent<Hurtbox>() != null)
        {
            // Chỉ gây sát thương nếu hitbox của mình đang trong giai đoạn "nóng"
            if (this.isDealingDamage)
            {
                HandleCharacterCollision(targetStats);
            }
        }
    }

    /// <summary>
    /// Xử lý khi va chạm với một hitbox "nóng" khác (Parry).
    /// Chỉ chịu trách nhiệm cho hiệu ứng hình ảnh/âm thanh.
    /// </summary>
    private void HandleClash(EnemyAttackHitbox enemyHitbox, CharacterStats enemyStats)
    {
        // Ghi sổ cả hai để không xử lý lại
        this.AddProcessedCharacter(enemyStats);
        enemyHitbox.AddProcessedCharacter(this.ownerStats);

        Debug.Log("CLANG! Physical weapon collision confirmed.");

        Vector3 effectPosition = (this.transform.position + enemyHitbox.transform.position) / 2f;
        EnemyAI enemyAI = enemyStats.GetComponent<EnemyAI>();
        if (enemyAI == null) return;
        // Dựa vào "ý định" đã được PlayerAttack quyết định để tạo hiệu ứng
        if (this.intent == AttackIntent.PerfectParry)
        {
            if (EffectManager.Instance != null) EffectManager.Instance.PlayEffect("ParrySparkPerfect", effectPosition, Quaternion.identity);
            // Hiệu ứng khựng hình mạnh hơn đã được kích hoạt bởi PlayerAttack
        }
        else // Normal Parry và va chạm thường
        {
            if (EffectManager.Instance != null) EffectManager.Instance.PlayEffect("ParrySparkNormal", effectPosition, Quaternion.identity);
            // Hiệu ứng khựng hình nhẹ hơn đã được kích hoạt bởi PlayerAttack
        }
        //this.DisableDamage(); // Tắt sát thương
        //enemyHitbox.DisableDamage(); // Tắt sát thương của hitbox địch
    }

    /// <summary>
    /// Xử lý khi vũ khí "nóng" của Player va vào vũ khí "nguội" của Enemy.
    /// </summary>
    private void HandleBlock(EnemyAttackHitbox enemyHitbox, CharacterStats blockedTargetStats)
    {
        // Ghi nhận đã tương tác với nhân vật này
        this.AddProcessedCharacter(blockedTargetStats);

        Debug.Log(ownerStats.name + "'s attack was BLOCKED by " + blockedTargetStats.name + "'s idle weapon.");

        // Tạo hiệu ứng "keng" và khựng hình nhẹ
        Vector3 effectPosition = (transform.position + blockedTargetStats.transform.position) / 2;
        if (EffectManager.Instance != null) EffectManager.Instance.PlayEffect("ParrySparkNormal", effectPosition, Quaternion.identity);
        if (HitStopManager.Instance != null) HitStopManager.Instance.Freeze(0.05f);

        // Đẩy lùi nhẹ người tấn công
        ownerStats.TriggerKnockback((ownerStats.transform.position - blockedTargetStats.transform.position).normalized, 0.5f);
        this.DisableDamage(); // Tắt sát thương
        enemyHitbox.DisableDamage(); // Tắt sát thương của hitbox địch
    }

    /// <summary>
    /// Xử lý khi va chạm với cơ thể (Hurtbox) của đối phương.
    /// </summary>
    private void HandleCharacterCollision(CharacterStats targetStats)
    {
        // Ghi sổ mục tiêu
        this.AddProcessedCharacter(targetStats);

        // Nếu ý định là để Parry, không gây sát thương.
        if (this.intent != AttackIntent.Normal)
        {
            Debug.Log("Parry-intended swing connected with body, no damage dealt.");
            return;
        }

        // Nếu ý định là Normal Attack, gây sát thương.
        targetStats.TakeDamage(damage, ownerStats);
    }
}