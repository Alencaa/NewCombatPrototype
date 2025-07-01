using System.Collections;
using UnityEngine;

/// <summary>
/// Xử lý hành vi phòng thủ và phản ứng sát thương của người chơi.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Combat Settings")]
    public int maxHP = 100;
    public float parryWindow = 0.3f;
    public bool isInvincible = false;

    private int currentHP;
    private bool isParryActive = false;
    private bool isHoldingBlock = false;

    public Vector2 CurrentBlockDirection { get; private set; } = Vector2.down;

    private void Start()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        HandleBlockInput();
        HandleParryInput(); // Tuỳ bạn trigger bằng swipe hay hotkey
    }

    /// <summary>
    /// Check nếu đang giữ block (RMB)
    /// </summary>
    private void HandleBlockInput()
    {
        isHoldingBlock = Input.GetMouseButton(1); // RMB

        // Cập nhật hướng block bằng chuột
        if (isHoldingBlock)
        {
            Vector2 mouseScreen = Input.mousePosition;
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
            Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
            UpdateBlockDirection(dir);
        }
    }

    /// <summary>
    /// Fake parry bằng nút Q (test) – bạn có thể gọi từ GestureInput
    /// </summary>
    private void HandleParryInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartParryWindow(parryWindow);
        }
    }

    public bool IsBlocking() => isHoldingBlock;
    public bool IsParrying() => isParryActive;

    /// <summary>
    /// Kích hoạt parry window trong khoảng thời gian ngắn
    /// </summary>
    public void StartParryWindow(float duration)
    {
        StopCoroutine(nameof(ParryWindowCoroutine));
        StartCoroutine(ParryWindowCoroutine(duration));
    }

    private IEnumerator ParryWindowCoroutine(float duration)
    {
        isParryActive = true;
        Debug.Log("Parry Window ACTIVE");
        yield return new WaitForSeconds(duration);
        isParryActive = false;
        Debug.Log("Parry Window END");
    }

    public void UpdateBlockDirection(Vector2 inputDir)
    {
        if (inputDir != Vector2.zero)
            CurrentBlockDirection = inputDir.normalized;
    }

    /// <summary>
    /// Gọi từ enemy → xử lý mất máu
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHP -= amount;
        Debug.Log($"Player took {amount} damage. Current HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            PlayHurtFeedback();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
        // TODO: Trigger death animation & restart
    }

    private void PlayHurtFeedback()
    {
        // TODO: Trigger hurt animation, flash, etc.
        Debug.Log("Player Hurt Feedback Triggered");
    }
}
