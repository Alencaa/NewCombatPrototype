using UnityEngine;

/// <summary>
/// Script "Tổng đài" sự kiện.
/// Nó nằm trên object chứa Animator, nhận các lệnh gọi từ Animation Event
/// và chuyển tiếp chúng đến các script logic tương ứng (Player/Enemy) ở object cha.
/// </summary>
public class AnimationEventForwarder : MonoBehaviour
{
    // Tham chiếu đến các script logic ở object cha
    private PlayerAttack playerAttack;
    private EnemyAI enemyAI;
    private PlayerMovement playerMovement;
    private EnemyMovement enemyMovement;
    private Animator animator;
    private BaseAttackHitbox attackHitbox; // Dùng lớp cha để có thể áp dụng cho cả hai

    private float crashFrameTime;

    void Awake()
    {
        // Tự động tìm tất cả các component có thể có ở object cha
        playerAttack = GetComponentInParent<PlayerAttack>();
        enemyAI = GetComponentInParent<EnemyAI>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        enemyMovement = GetComponentInParent<EnemyMovement>();
        animator = GetComponent<Animator>();
        attackHitbox = GetComponentInChildren<BaseAttackHitbox>(true); // true để tìm cả object đang bị tắt
        Debug.Log(animator);
    }

    // --- CÁC HÀM TRUNG GIAN SẼ ĐƯỢC GỌI BỞI ANIMATION EVENT ---

    //public void DisableHitbox()
    //{
    //    if (playerAttack != null)
    //    {
    //        playerAttack.DisableHitbox();
    //    }
    //    else if (enemyAI != null)
    //    {
    //        enemyAI.DisableHitbox();
    //    }
    //}

    public void UnlockMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.UnlockMovement();
        }
        else if (enemyMovement != null)
        {
            enemyMovement.UnlockMovement();
        }
    }
    public void StartAttack()
    {
        if (attackHitbox != null) attackHitbox.StartAttack();
    }
    public void EndAttack()
    {
        if (attackHitbox != null) attackHitbox.EndAttack();
    }
    public void BackToIdle()
    {
        animator.SetTrigger("idle"); // Đặt lại trạng thái idle cho Enemy
    }
    public void EnableHitbox(ActionDirection direction)
    {
        if (playerAttack != null)
        {
            playerAttack.EnableHitbox(direction);
        }
    }

    // Dành cho cả hai
    public void ResetState()
    {
        if (attackHitbox != null) attackHitbox.ResetState();
    }

    public void EnableDamage()
    {
        if (attackHitbox != null) attackHitbox.EnableDamage();
    }

    public void DisableDamage()
    {
        if (attackHitbox != null) attackHitbox.DisableDamage();
    }

    // Dành cho Enemy
    public void OpenParryWindow()
    {
        // Chuyển sang dùng EnemyAttackHitbox để gọi
        EnemyAttackHitbox enemyHitbox = attackHitbox as EnemyAttackHitbox;
        if (enemyHitbox != null) enemyHitbox.OpenParryWindow();
    }

    public void CloseParryWindow()
    {
        EnemyAttackHitbox enemyHitbox = attackHitbox as EnemyAttackHitbox;
        if (enemyHitbox != null) enemyHitbox.CloseParryWindow();
    }
}