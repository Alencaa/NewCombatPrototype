using CombatV2.Combat;
using CombatV2.FSM;
using CombatV2.FSM.States;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Xử lý hành vi phòng thủ và phản ứng sát thương của người chơi.
/// </summary>
public class PlayerController : MonoBehaviour, IAttackable
{


    [Header("Combat Settings")]
    public int maxHP = 100;
    public float parryWindow = 0.3f;
    public bool IsInvincible { get; set; }

    private int currentHP;
    private bool isParryActive = false;
    public bool IsHoldingBlock { get; set; }

    [Header("Block Arrow")]
    [SerializeField] private GameObject blockArrowPrefab;
    private GameObject blockArrowInstance;
    public Vector2 CurrentBlockDirection { get;  set; }

    public Animator Animator => animator; // Gán Animator từ Inspector
    public TextMeshPro TextDebug => textDebug; // Gán Animator từ Inspector

    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshPro textDebug;
    //STATE MACHINE
    private StateMachine<PlayerController> stateMachine;
    public StateMachine<PlayerController> StateMachine => stateMachine;

    [SerializeField] private PlayerCombatConfig playerCombatConfig;
    public PlayerCombatConfig PlayerCombatConfig => playerCombatConfig;

    public InputBuffer InputBuffer { get; private set; }

    [SerializeField] private PlayerAttackHitbox slashHitboxPrefab;
    public PlayerAttackHitbox SlashHitbox => slashHitboxPrefab;
    public Vector2 lastAttackerPosition { get; private set; }
    public GestureType LastParryGesture { get; set; }
    public bool IsInWindUp { get; set; } = false; // Biến để xác định player có đang trong trạng thái wind-up hay không



    private void Awake()
    {
        stateMachine = new StateMachine<PlayerController>(this);
        stateMachine.ChangeState(new PlayerIdleState(this, stateMachine));
        InputBuffer = new InputBuffer();
    }
    private void Start()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        InputBuffer.Update(Time.deltaTime);
        stateMachine.Update();
    }

   
    public bool IsBlocking() => IsHoldingBlock;
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
        if (IsInvincible) return;

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

    public void RequestAttackState(GestureData gesture)
    {
        stateMachine.ChangeState(new PlayerAttackState(this, stateMachine, gesture));
    }

    public string GetDirectionName(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(Vector2.right, dir);

        if (angle >= -22.5f && angle <= 22.5f) return "Right";
        if (angle > 22.5f && angle <= 67.5f) return "UpRight";
        if (angle > 67.5f && angle <= 112.5f) return "Up";
        if (angle > 112.5f && angle <= 157.5f) return "UpLeft";
        if (angle > 157.5f || angle <= -157.5f) return "Left";
        if (angle < -112.5f && angle >= -157.5f) return "DownLeft";
        if (angle < -67.5f && angle >= -112.5f) return "Down";
        if (angle < -22.5f && angle >= -67.5f) return "DownRight";

        return "None";
    }

    public void ShowBlockArrow(Vector2 direction)
    {
        if (blockArrowInstance == null)
        {
            blockArrowInstance = Instantiate(blockArrowPrefab, transform);
            blockArrowInstance.transform.localPosition = new Vector2(0, -0.2f);
        }

        blockArrowInstance.SetActive(true);
        RotateArrow(direction);
    }

    public void RotateArrow(Vector2 direction)
    {
        if (blockArrowInstance == null) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        blockArrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void HideBlockArrow()
    {
        if (blockArrowInstance != null)
            blockArrowInstance.SetActive(false);
    }

    public void OnHitReceived(AttackData data, HitRegionType region, Vector2 fromPos)
    {
        if (IsInvincible)
        {
            Debug.Log("Player is invincible, ignoring hit.");
            return;
        }
        // xử lý riêng cho player
        Debug.Log($"💢 Player hit: {data.attackName} → Region: {region}");
        lastAttackerPosition = fromPos;

        bool isCounterHit = IsInWindUp; // Kiểm tra xem có phải Counter Hit không
        // Example logic – bạn có thể mở rộng sau
        if (!isCounterHit)
        {
            // Đòn thường → react + i-frame
            StateMachine.ChangeState(new PlayerHitReactState(this, StateMachine, data, region));
        }
        else
        {
            // Counter Hit → Stagger theo vùng
            StateMachine.ChangeState(new PlayerStaggerState(this, StateMachine, data, region));
        }
    }
    public void OnSuperParry(AttackData data)
    {
        Debug.Log($"⚡ SUPER PARRY thành công! Chống lại: {data.attackName}");
        // TODO: Chuyển sang SuperParryState hoặc trả đòn đặc biệt
    }

    public void OnBlocked(AttackData data)
    {
        Debug.Log($"🛡 BLOCK thành công! Đỡ đòn: {data.attackName}");
        // TODO: Hiệu ứng flash, làm chậm, phản lực, v.v.
    }

    public void OnFailedParry(AttackData data)
    {
        Debug.Log($"❌ Parry sai hướng! Mất posture từ đòn: {data.attackName}");
        // TODO: Trừ posture, không gây damage trực tiếp
    }
}
