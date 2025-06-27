using DG.Tweening; // --- THÊM DÒNG NÀY ĐỂ SỬ DỤNG DOTWEEN ---
using System.Collections;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = 20f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float pushDustOffset = 0.5f; // Bụi sẽ cách chân bao xa
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    // --- THAY ĐỔI Ở ĐÂY ---
    [Header("Animation Settings")]
    [SerializeField] private Animator animator; // Kéo Animator vào đây
    private SpriteRenderer spriteRenderer;

    private Vector2 currentVelocity;
    private bool isGrounded;

    [Header("Attack Momentum")]
    [SerializeField] private float durationMomentum = 0.2f;

    public bool lockMovement = false; // Cờ để khóa di chuyển và vật lý thông thường

    // Thêm tham chiếu đến PlayerAttack để có thể gọi hàm ExecuteBufferedAttack
    [Header("Components")]
    [SerializeField] private PlayerAttack playerAttack;

    [Tooltip("Kéo object chứa hình ảnh và animator (ví dụ: PlayerGFX) vào đây.")]
    [SerializeField] private Transform graphicsTransform; // THÊM DÒNG NÀY

    private Tween activeMomentumTween;

    [Header("Effects")]
    [SerializeField] private float timeBetweenFootsteps = 0.3f; // Khoảng cách thời gian giữa mỗi lần tạo bụi chạy
    private float footstepTimer;


    void Start()
    {
        // Giữ nguyên hoặc thêm GetComponent nếu cần
        if (animator == null) animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerAttack == null) playerAttack = GetComponent<PlayerAttack>();

    }

    void Update()
    {
        // 1. KIỂM TRA INPUT VÀ TÍNH TOÁN VẬN TỐC
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!lockMovement)
        {
            // Chỉ xử lý input và vật lý thông thường nếu không bị khóa
            float horizontalInput = Input.GetAxis("Horizontal");
            currentVelocity.x = horizontalInput * moveSpeed;

            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                currentVelocity.y = jumpForce;
            }

            // Áp dụng trọng lực
            if (!isGrounded)
            {
                currentVelocity.y -= gravity * Time.deltaTime;
            }
            // Reset vận tốc Y khi chạm đất (nguyên nhân gây lỗi)
            else if (currentVelocity.y < 0)
            {
                currentVelocity.y = 0;
            }
        }
        HandleRunningDustEffect();

        // Cập nhật Animator và lật sprite theo input
        animator.SetFloat("speed", Mathf.Abs(currentVelocity.x));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", currentVelocity.y);
        SetFacingDirection(currentVelocity.x);


        // Cập nhật các thông số animator khác luôn được thực hiện

        // Di chuyển nhân vật luôn được thực hiện dựa trên currentVelocity cuối cùng
        transform.Translate(currentVelocity * Time.deltaTime);
    }
    private void HandleRunningDustEffect()
    {
        // Kiểm tra xem nhân vật có đang di chuyển trên mặt đất không
        if (isGrounded && Mathf.Abs(currentVelocity.x) > 0.1f)
        {
            // Đếm ngược timer
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                // Khi timer hết, tạo hiệu ứng và reset timer
                footstepTimer = timeBetweenFootsteps;

                // Bụi sẽ xuất hiện phía sau lưng nhân vật
                Vector2 dustDirection = IsFacingRight() ? Vector2.left : Vector2.right;
                Vector3 dustPosition = groundCheck.position;
                Quaternion dustRotation = Quaternion.FromToRotation(Vector3.left, dustDirection);

                EffectManager.Instance.PlayEffect("RunDust", dustPosition, dustRotation);
            }
        }
    }
    private void SetFacingDirection(float horizontalDirection)
    {
        if (graphicsTransform == null) return;

        // Dùng ngưỡng nhỏ để tránh rung lắc khi vận tốc gần bằng 0
        if (horizontalDirection > 0.1f)
        {
            graphicsTransform.localScale = Vector3.one;
        }
        else if (horizontalDirection < -0.1f)
        {
            graphicsTransform.localScale = new Vector3(-1, 1, 1);
        }
        // Nếu không, giữ nguyên hướng cũ
    }
    /// <summary>
    /// Hàm công khai để các script khác có thể yêu cầu thêm một lực đẩy tức thời.
    /// </summary>
    /// <param name="direction">Hướng của lực đẩy.</param>
    /// <param name="force">Độ mạnh của lực đẩy.</param>
    /// <param name="duration">thoi gian day.</param>
    public void TriggerKnockback(Vector2 direction, float distance, float duration)
    {
        // Dừng tween cũ nếu có
        if (activeMomentumTween != null && activeMomentumTween.IsActive())
        {
            activeMomentumTween.Kill();
        }

        // 1. Khóa điều khiển
        lockMovement = true;

        currentVelocity = Vector2.zero;

        // 2. Tính toán điểm đến và thời gian di chuyển
        Vector3 endPosition = transform.position + (Vector3)direction * distance;

        // Nếu không có khoảng cách hoặc thời gian, không cần tạo tween, unlock ngay
        if (duration <= 0)
        {
            UnlockMovement();
            return;
        }
        // 3. TẠO TWEEN!
        activeMomentumTween = transform.DOMove(endPosition, duration)
                                       .SetEase(Ease.OutQuad).OnComplete(UnlockMovement); // Đây là Easing!
        // --- THÊM LOGIC TẠO HIỆU ỨNG ---
        // Chỉ tạo hiệu ứng nếu có di chuyển và đang ở trên mặt đất
        if (distance > 0 && isGrounded)
        {
            // Hướng của hiệu ứng sẽ ngược lại với hướng lướt đi
            Vector2 dustDirection = -direction.normalized;
            // Vị trí là ở dưới chân, lệch về phía đối diện với hướng lướt
            Vector3 dustPosition = groundCheck.position + (Vector3)dustDirection * pushDustOffset;
            // Góc xoay của hiệu ứng cũng ngược với hướng lướt
            Quaternion dustRotation = Quaternion.FromToRotation(Vector3.right, dustDirection);

            EffectManager.Instance.PlayEffect("SkidDust", dustPosition, dustRotation);
        }
    }

    public void StartActionPush(Vector2 direction, float distance)
    {
        if (activeMomentumTween != null && activeMomentumTween.IsActive())
        {
            activeMomentumTween.Kill();
        }
        lockMovement = true;
        currentVelocity = Vector2.zero;

        Vector3 endPosition = transform.position + (Vector3)direction * distance;

        if (durationMomentum > 0)
        {
            // Tween này KHÔNG có OnComplete
            SetFacingDirection(direction.x);

            activeMomentumTween = transform.DOMove(endPosition, durationMomentum)
                                           .SetEase(Ease.OutQuad);
        }
        // --- THÊM LOGIC TẠO HIỆU ỨNG ---
        // Chỉ tạo hiệu ứng nếu có di chuyển và đang ở trên mặt đất
        if (distance > 0 && isGrounded)
        {
            // Hướng của hiệu ứng sẽ ngược lại với hướng lướt đi
            Vector2 dustDirection = -direction.normalized;
            // Vị trí là ở dưới chân, lệch về phía đối diện với hướng lướt
            Vector3 dustPosition = groundCheck.position + (Vector3)dustDirection * pushDustOffset;
            // Góc xoay của hiệu ứng cũng ngược với hướng lướt
            Quaternion dustRotation = Quaternion.FromToRotation(Vector3.left, dustDirection);

            EffectManager.Instance.PlayEffect("SkidDust", dustPosition, dustRotation);
        }
        // Nếu không có di chuyển, ta vẫn phải chờ Animation Event để unlock
    }
    /// <summary>
    /// Hàm này được thiết kế để gọi bởi một Animation Event khi animation kết thúc.
    /// </summary>
    public void UnlockMovement()
    {
        // Dừng tween (nếu có) và mở khóa di chuyển như cũ
        if (activeMomentumTween != null && activeMomentumTween.IsActive())
        {
            activeMomentumTween.Kill();
        }
        lockMovement = false;
        currentVelocity = Vector2.zero;

        // --- DÒNG MỚI QUAN TRỌNG ---
        // Sau khi đã rảnh rỗi, kiểm tra xem có đòn tấn công nào đang chờ không
        if (playerAttack != null)
        {
            playerAttack.ExecuteBufferedAction();
        }
    }
    /// <summary>
    /// Hàm công khai để các script khác kiểm tra xem di chuyển có đang bị khóa không.
    /// </summary>
    /// <returns>True nếu di chuyển đang bị khóa, False nếu không.</returns>
    public bool IsMovementLocked()
    {
        return lockMovement;
    }
    public bool IsFacingRight()
    {
        // Nếu không có graphicsTransform, mặc định là đang quay sang phải để tránh lỗi
        if (graphicsTransform == null) return true;

        // Trả về true nếu scale.x là số dương (1), và false nếu là số âm (-1)
        return graphicsTransform.localScale.x > 0;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}