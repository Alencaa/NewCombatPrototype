using UnityEngine;

public class EnemyAttackHitbox : BaseAttackHitbox
{
    public bool canBeParried { get; private set; }
    public bool wasParriedThisSwing { get; private set; }

    public override void ResetState()
    {
        base.ResetState();
        canBeParried = false;
        wasParriedThisSwing = false;
    }

    public void OpenParryWindow() 
    {
        StartAttack();
        canBeParried = true; 
    }
    public void CloseParryWindow() 
    {
        EndAttack();
        canBeParried = false; 
    }
    public void MarkAsParried() { wasParriedThisSwing = true; }

    void OnTriggerStay2D(Collider2D other)
    {
         // Cổng bảo vệ chính
        if (!isAttackActive || ownerStats == null) return;

        // Enemy không cần logic Parry/Block phức tạp, nó chỉ cần biết có gây sát thương cho Player hay không.
        // Logic Parry sẽ do PlayerAttackHitbox xử lý khi nó va chạm với hitbox này.
        Hurtbox playerHurtbox = other.GetComponent<Hurtbox>();
        if (playerHurtbox != null && playerHurtbox.characterStats.CompareTag("Player"))
        {
            if (processedCharacters.Contains(playerHurtbox.characterStats)) return;

            // Chỉ gây sát thương khi đang trong giai đoạn "nóng"
            if(isDealingDamage)
            {
                AddProcessedCharacter(playerHurtbox.characterStats);
                playerHurtbox.characterStats.TakeDamage(damage, ownerStats);
            }
        }
    }
}