// Trong EnemyMovement.cs
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EnemyMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = 20f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float pushDustOffset = 0.5f; // Bụi sẽ cách chân bao xa
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Action Push Settings")]
    [SerializeField] private float pushDuration = 0.2f;

    private Vector2 currentVelocity;
    private bool isGrounded;
    private bool lockMovement = false;
    private Tween activeMomentumTween;

    [Tooltip("Kéo object chứa hình ảnh và animator (ví dụ: PlayerGFX) vào đây.")]
    [SerializeField] private Transform graphicsTransform; // THÊM DÒNG NÀY

    [Header("Effects")]
    [SerializeField] private float timeBetweenFootsteps = 0.3f; // Khoảng cách thời gian giữa mỗi lần tạo bụi chạy
    private float footstepTimer;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!lockMovement)
        {
            // AI sẽ điều khiển vận tốc x, ở đây ta chỉ áp dụng trọng lực
            if (!isGrounded)
            {
                currentVelocity.y -= gravity * Time.deltaTime;
            }
            else if (currentVelocity.y < 0)
            {
                currentVelocity.y = 0;
            }
        }
        HandleRunningDustEffect();

        // Cập nhật Animator
        animator.SetFloat("speed", Mathf.Abs(currentVelocity.x));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", currentVelocity.y);

        // Di chuyển
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

    // Các hàm public để AI điều khiển
    public void Move(float direction) // direction là -1 (trái), 1 (phải), hoặc 0 (đứng yên)
    {
        if (lockMovement) return;

        currentVelocity.x = direction * moveSpeed;

        SetFacingDirection(currentVelocity.x);
    }
    public void PlayDodgeAnimation()
    {
        animator.SetTrigger("dodgeDown");

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
    public void FaceTarget(Vector3 targetPosition)
    {
        if (lockMovement) return;

        float directionToTarget = targetPosition.x - transform.position.x;

        // Gọi hàm lật chuyên dụng mà chúng ta đã tạo
        SetFacingDirection(directionToTarget);
    }

    public void TriggerKnockback(Vector2 direction, float distance, float duration)
    {
        if (activeMomentumTween != null && activeMomentumTween.IsActive())
        {
            activeMomentumTween.Kill();
        }

        lockMovement = true;
        currentVelocity = Vector2.zero;

        Vector3 endPosition = transform.position + (Vector3)direction * distance;
        if (duration > 0)
        {
            activeMomentumTween = transform.DOMove(endPosition, duration).SetEase(Ease.OutQuad).OnComplete(UnlockMovement);
        }
        else
        {
            UnlockMovement(); // Nếu không có khoảng cách hoặc thời gian, ta vẫn phải unlock ngay
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

        if (pushDuration > 0)
        {
            SetFacingDirection(currentVelocity.x); // Cập nhật hướng trước khi di chuyển
            // Tween này KHÔNG có OnComplete
            activeMomentumTween = transform.DOMove(endPosition, pushDuration)
                                           .SetEase(Ease.OutQuad);
        }
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

    public void UnlockMovement()
    {
        if (activeMomentumTween != null && activeMomentumTween.IsActive())
        {
            activeMomentumTween.Kill();
        }
        lockMovement = false;
        currentVelocity = Vector2.zero;

        // Trong tương lai, AI cũng có thể có buffer
        // GetComponent<EnemyAI>().ExecuteBufferedAction();
    }

    public bool IsFacingRight()
    {
        if (graphicsTransform == null) return true;
        return graphicsTransform.localScale.x > 0;
    }

    public bool IsMovementLocked()
    {
        return lockMovement;
    }

    public void SnapToAnimationTime(float normalizedTime)
    {
        // Rebind giúp đảm bảo các thay đổi thuộc tính của animator được áp dụng trước khi Play
        animator.Rebind();
        // Play state hiện tại từ một thời điểm (normalized time) cụ thể
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, normalizedTime);
        Debug.Log(gameObject.name + " snapped animation to " + normalizedTime);
    }
}