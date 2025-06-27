using UnityEngine;
using DG.Tweening;       // Cần thiết cho DOTween
using System.Collections; // Cần thiết cho Coroutine

public class CharacterStats : MonoBehaviour
{
    [Header("Components")]
    private PlayerMovement playerMovement; // Chỉ có ở Player
    private EnemyMovement enemyMovement;   // Chỉ có ở Enemy

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Combat Settings")]
    [Tooltip("Tỉ lệ né đòn khi không trong trạng thái tấn công (0.0 = 0%, 1.0 = 100%)")]
    [Range(0f, 1f)] public float dodgeChance = 0.3f; // 30%
    public float knockbackDistance = 1f;
    public float knockbackDuration = 0.2f; // Thời gian đẩy lùi

    [Tooltip("Transform chứa hình ảnh chính để thực hiện hiệu ứng co dãn.")]
    [SerializeField] private Transform graphicsTransform;
    private SpriteRenderer characterSprite;
    private Color originalColor;

    [Header("Damage Effects")]
    // --- BIẾN MỚI ---
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float squishStrength = -0.2f; // Dùng số âm để co lại theo chiều dọc
    [SerializeField] private float squishDuration = 0.2f;

    [Header("Damage Effects")]
    [SerializeField] private Vector2 effectOffset = new Vector2(-1.3f, 0.5f); // Hiệu ứng sẽ cách tâm nhân vật bao xa

    void Awake()
    {
        currentHealth = maxHealth;
        // Lấy component movement tương ứng
        playerMovement = GetComponent<PlayerMovement>();
        enemyMovement = GetComponent<EnemyMovement>();

        if (graphicsTransform == null)
        {
            // Thử tìm object con có tên là "PlayerGFX" hoặc "EnemyAnimationController"
            Transform gfx = transform.Find("PlayerGFX") ?? transform.Find("EnemyAnimationController");
            if (gfx != null) graphicsTransform = gfx;
        }
        if (graphicsTransform != null)
        {
            characterSprite = graphicsTransform.GetComponent<SpriteRenderer>();
        }

        if (characterSprite != null)
        {
            // Lưu lại màu gốc để có thể quay trở lại
            originalColor = characterSprite.color;
        }
    }
    private IEnumerator FlashEffect()
    {
        if (characterSprite == null) yield break;

        // Đổi sang màu flash
        characterSprite.color = flashColor;

        // Chờ một khoảng thời gian ngắn
        yield return new WaitForSeconds(flashDuration);

        // Trở về màu gốc
        characterSprite.color = originalColor;
    }
    // Hàm để nhận sát thương
    /// <summary>
    /// Hàm nhận sát thương, giờ đây nhận vào thông tin của kẻ tấn công.
    /// </summary>
    /// <param name="damage">Lượng sát thương.</param>
    /// <param name="attackerStats">Component CharacterStats của kẻ tấn công.</param>
    public void TakeDamage(int damage, CharacterStats attackerStats)
    {
        // Xử lý né đòn cho Enemy
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.ClearBufferedAction();
        }

        if (enemyMovement != null && !enemyMovement.IsMovementLocked())
        {
            if (Random.value < dodgeChance)
            {
                Debug.Log("Enemy Dodged!");
                // Né đòn bằng cách lùi ra xa kẻ tấn công
                enemyMovement.PlayDodgeAnimation(); // Gọi animation né đòn
                Vector2 dodgeDirection = (transform.position - attackerStats.transform.position).normalized;
                enemyMovement.TriggerKnockback(dodgeDirection, knockbackDistance, knockbackDuration); // Lùi đi 1 khoảng
                EffectManager.Instance.PlayEffect("Dodge", new Vector2(transform.position.x, transform.position.y + 5.5f), Quaternion.identity, transform);
                return; // Không nhận sát thương
            }
        }
        StartCoroutine(FlashEffect());

        // 1. Lấy script movement của kẻ tấn công để biết nó đang quay mặt hướng nào
        // Chúng ta cần một cách chung để lấy script movement
        var attackerMovement = attackerStats.GetComponent<PlayerMovement>() as object ?? attackerStats.GetComponent<EnemyMovement>();
        bool attackerIsFacingRight = true; // Mặc định
        if (attackerMovement is PlayerMovement pm) attackerIsFacingRight = pm.IsFacingRight();
        if (attackerMovement is EnemyMovement em) attackerIsFacingRight = em.IsFacingRight();

        // 2. Hướng của nhát chém chính là hướng mà kẻ tấn công đang quay mặt
        Vector2 hitDirection = attackerIsFacingRight ? Vector2.right : Vector2.left;

        Vector3 effectPosition = transform.position; // Vị trí bắt đầu là tâm của nạn nhân
        float offsetX = -hitDirection.x * effectOffset.x;
        float offsetY = effectOffset.y;

        // Thêm độ lệch vào vị trí ban đầu
        effectPosition += new Vector3(offsetX, offsetY, 0);

        // 4. Tạo góc xoay để hiệu ứng phun ra theo hướng nhát chém
        Quaternion effectRotation = Quaternion.FromToRotation(Vector3.right, hitDirection);



        // --- PHẦN CÒN LẠI CỦA LOGIC ---

        if (HitStopManager.Instance != null) HitStopManager.Instance.Freeze(0.08f);

        currentHealth -= damage;
        Debug.Log(transform.name + " took " + damage + " damage.");

        // Lực đẩy lùi vẫn dựa trên hướng từ kẻ tấn công đến nạn nhân
        Vector2 knockbackDirection = (transform.position - attackerStats.transform.position).normalized;
        TriggerKnockback(knockbackDirection, knockbackDistance);

        if (currentHealth <= 0)
        {
            Die();
            if (EffectManager.Instance != null)
            {
                EffectManager.Instance.PlayEffect("BloodDie", effectPosition, effectRotation, transform);
                EffectManager.Instance.PlayEffect("SwordHit", new Vector2(transform.position.x, transform.position.y + 0.5f), effectRotation, transform);
            }
            return;
        }
        // 5. Gọi hiệu ứng
        if (EffectManager.Instance != null)
        {
            EffectManager.Instance.PlayEffect("BloodNormal", effectPosition, effectRotation, transform);
            EffectManager.Instance.PlayEffect("SwordHit", new Vector2(transform.position.x, transform.position.y + 0.5f), effectRotation, transform);
        }

    }

    public void TriggerKnockback(Vector2 direction, float distance)
    {
        if (playerMovement != null)
        {
            playerMovement.TriggerKnockback(direction, distance, knockbackDuration);
        }
        else if (enemyMovement != null)
        {
            enemyMovement.TriggerKnockback(direction, distance, knockbackDuration);
        }
    }

    private void Die()
    {
        Debug.Log(transform.name + " has died.");
        // Xử lý logic khi chết (bật animation chết, vô hiệu hóa đối tượng, v.v.)
        // animator.SetBool("IsDead", true);
        // this.enabled = false; // Vô hiệu hóa script này
    }

    // Hàm để các script khác kiểm tra xem nhân vật có đang tấn công không
    public bool IsInAction()
    {
        if (playerMovement != null) return playerMovement.IsMovementLocked();
        if (enemyMovement != null) return enemyMovement.IsMovementLocked();
        return false;
    }
}