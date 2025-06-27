using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 10;
    public CharacterStats ownerStats;
    public ActionDirection attackDirection;
    public AttackIntent intent = AttackIntent.Normal;

    public bool isDealingDamage { get; private set; }
    public bool canBeParried { get; private set; }
    public bool wasParriedThisSwing;
    public bool isStartAttacking;

    private List<CharacterStats> processedCharacters;

    void Awake()
    {
        processedCharacters = new List<CharacterStats>();
    }

    public void ResetState()
    {
        processedCharacters.Clear();
        intent = AttackIntent.Normal;
        isDealingDamage = false;
        canBeParried = false;
        wasParriedThisSwing = false;
    }

    public void MarkAsParried()
    {
        wasParriedThisSwing = true;
    }

    public void EnableDamage() { isDealingDamage = true; }
    public void DisableDamage() { isDealingDamage = false; }
    public void OpenParryWindow() { canBeParried = true; }
    public void CloseParryWindow() { canBeParried = false; }
    public void StartAttacking() { isStartAttacking = true; }
    public void StopAttackingAndCheckForDamage() { isStartAttacking = false; }

    void OnTriggerStay2D(Collider2D other)
    {
        // Kiểm tra cơ bản
        if (ownerStats == null || !isDealingDamage) return;

        // --- LOGIC MỚI: TÌM KIẾM HURTBOX THAY VÌ CHARACTERSTATS ---
        Hurtbox targetHurtbox = other.GetComponent<Hurtbox>();
        CharacterStats targetStats = other.GetComponentInParent<CharacterStats>();

        // Nếu va chạm với một Hurtbox và nó không phải của mình
        if (targetHurtbox != null && targetHurtbox.characterStats != ownerStats)
        {
            // Kiểm tra xem đã xử lý nhân vật này chưa
            if (!processedCharacters.Contains(targetHurtbox.characterStats))
            {
                HandleCharacterCollision(targetHurtbox.characterStats);
            }
            return; // Xử lý xong hurtbox thì không làm gì nữa
        }

        // Ưu tiên kiểm tra và xử lý va chạm với hitbox khác
        AttackHitbox otherHitbox = other.GetComponent<AttackHitbox>();
        if (otherHitbox != null && otherHitbox.ownerStats != ownerStats)
        {
            // Kiểm tra xem đã xử lý nhân vật này chưa
            if (!processedCharacters.Contains(otherHitbox.ownerStats))
            {
                if (otherHitbox.isDealingDamage)
                {
                    HandleClash(otherHitbox, targetStats);
                }
                else// Chạm vào vũ khí "nguội" -> Bị chặn
                {
                    if (otherHitbox.isStartAttacking)
                    {
                        HandleBlock(otherHitbox.ownerStats);
                    }
                    
                }
            }
        }
    }
    private void HandleBlock(CharacterStats blockedTargetStats)
    {
        // Ghi nhận đã tương tác với nhân vật này
        processedCharacters.Add(blockedTargetStats);

        Debug.Log(ownerStats.name + "'s attack was BLOCKED by " + blockedTargetStats.name + "'s weapon.");

        // Tạo hiệu ứng "keng" và khựng hình nhẹ
        Vector3 effectPosition = (transform.position + blockedTargetStats.transform.position) / 2;
        if (EffectManager.Instance != null) EffectManager.Instance.PlayEffect("ParrySparkNormal", effectPosition, Quaternion.identity);
        if (HitStopManager.Instance != null) HitStopManager.Instance.Freeze(0.05f);

        // Có thể đẩy lùi nhẹ người tấn công
        ownerStats.TriggerKnockback((ownerStats.transform.position - blockedTargetStats.transform.position).normalized, 0.5f);
    }

    // Hàm này chỉ xử lý hiệu ứng "va chạm" vật lý, logic gameplay đã được quyết định từ trước
    private void HandleClash(AttackHitbox otherHitbox, CharacterStats targetStats)
    {
        // Ghi sổ để không xử lý lại va chạm này
        processedCharacters.Add(targetStats);
        processedCharacters.Add(this.ownerStats);

        Debug.Log("CLANG! Physical weapon collision.");

        // Luôn có khựng hình nhẹ khi hai vũ khí va vào nhau
        CharacterStats playerStats = ownerStats.CompareTag("Player") ? ownerStats : otherHitbox.ownerStats;
        if (HitStopManager.Instance != null) HitStopManager.Instance.Freeze(0.1f);
        AttackHitbox playerHitbox = (playerStats == ownerStats) ? this : otherHitbox;
        if (playerStats == null) return;
        // Có thể thêm hiệu ứng tia lửa ở đây
        // EffectManager.Instance.PlayEffect("ClashSpark", ...);
        Vector3 effectPosition = (this.transform.position + otherHitbox.transform.position) / 2f;
        switch (playerHitbox.intent)
        {
            case AttackIntent.NormalParry:
                EffectManager.Instance.PlayEffect("ParrySparkNormal", effectPosition, Quaternion.identity);
                break;
            case AttackIntent.PerfectParry:
                EffectManager.Instance.PlayEffect("ParrySparkPerfect", effectPosition, Quaternion.identity);
                break;
            case AttackIntent.Normal:
                EffectManager.Instance.PlayEffect("ParrySparkNormal", effectPosition, Quaternion.identity);
                break;
        }
        DisableDamage();
    }

    private void HandleCharacterCollision(CharacterStats targetStats)
    {
        // Nếu ý định của đòn đánh là để Parry, nó không gây sát thương.
        if (this.intent != AttackIntent.Normal)
        {
            processedCharacters.Add(targetStats);
            return;
        }

        // Ghi nhận đã xử lý
        processedCharacters.Add(targetStats);

        // Cho phép gây sát thương khi Enemy đang tấn công
        targetStats.TakeDamage(damage, ownerStats);
    }
}