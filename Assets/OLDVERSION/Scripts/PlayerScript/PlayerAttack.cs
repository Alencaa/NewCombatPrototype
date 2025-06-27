using UnityEngine;

// Định nghĩa các loại hành động mà người chơi có thể thực hiện
public enum PlayerActionType
{
    None,
    Attack,
    Dodge
}
public enum AttackIntent
{
    Normal,         // Đòn đánh thường
    NormalParry,    // Nỗ lực Parry thường
    PerfectParry    // Nỗ lực Parry hoàn hảo
}

public class PlayerAttack : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttackHitbox playerAttackHitbox;


    [Header("Input Settings")]
    [SerializeField] private float verticalBias = 1.2f;
    [SerializeField] private float minDragDistance = 50f;

    [Header("Action Settings")]
    [SerializeField] private float attackDistance = 0.5f;
    [SerializeField] private float dodgeDistance = 1.5f;

    [Header("Parry Settings")]
    [SerializeField] private float parryDetectionRange = 2.5f;
    public float clashFrameTime = 0.5f;

    private PlayerActionType bufferedAction = PlayerActionType.None;
    private Vector2 bufferedDragVector;
    private Vector2 mousePressPosition;
    private AttackIntent nextAttackIntent = AttackIntent.Normal;


    void Start()
    {
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerAttackHitbox == null) playerAttackHitbox = GetComponentInChildren<PlayerAttackHitbox>();
    }

    void Update()
    {
        HandleActionInput(0, PlayerActionType.Attack);
        HandleActionInput(1, PlayerActionType.Dodge);
    }

    private void HandleActionInput(int mouseButton, PlayerActionType actionType)
    {
        if (Input.GetMouseButtonDown(mouseButton)) mousePressPosition = Input.mousePosition;

        if (Input.GetMouseButtonUp(mouseButton))
        {
            Vector2 dragVector = (Vector2)Input.mousePosition - mousePressPosition;
            if (dragVector.magnitude < minDragDistance) return;

            if (playerMovement.IsMovementLocked())
            {
                bufferedAction = actionType;
                bufferedDragVector = dragVector;
                nextAttackIntent = AttackIntent.Normal;
                return;
            }

            // Mặc định là một đòn đánh thường
            nextAttackIntent = AttackIntent.Normal;

            if (actionType == PlayerActionType.Attack)
            {
                TryToDecideParry(dragVector);
            }

            // Luôn thực hiện animation, dù kết quả là gì
            PerformAction(actionType, dragVector);
        }
    }
    private void TryToDecideParry(Vector2 dragVector)
    {
        // Bước 1: Tìm kẻ địch đang tấn công ở gần
        EnemyAI enemyTarget = FindNearbyEnemy();
        if (enemyTarget == null) return;

        // Bước 2: Lấy CHÍNH XÁC EnemyAttackHitbox của kẻ địch
        EnemyAttackHitbox enemyHitbox = enemyTarget.GetComponentInChildren<EnemyAttackHitbox>();
        if (enemyHitbox == null) return;

        // Bước 3: Áp dụng bộ quy tắc (phần này không đổi)
        if (!enemyHitbox.wasParriedThisSwing)
        {
            bool inParryWindow = enemyHitbox.canBeParried;
            ActionDirection playerDir = GetDirectionFromVector(dragVector);
            ActionDirection enemyDir = enemyTarget.currentAttackDirection;
            bool directionsMatch = CheckParryDirection(playerDir, enemyDir);

            if (directionsMatch && inParryWindow)
            {
                // A: Perfect Parry
                nextAttackIntent = AttackIntent.PerfectParry;
                enemyHitbox.MarkAsParried();
                enemyTarget.TriggerParryStagger();
            }
            else if ((directionsMatch && !inParryWindow) || (!directionsMatch && inParryWindow))
            {
                // B: Normal Parry
                nextAttackIntent = AttackIntent.NormalParry;
                enemyHitbox.MarkAsParried();
            }
        }
        else // Spam Parry
        {
            nextAttackIntent = AttackIntent.NormalParry;
        }
    }
    private bool CheckParryDirection(ActionDirection playerDir, ActionDirection enemyDir)
    {
        bool sideParry = (playerDir == ActionDirection.Side && enemyDir == ActionDirection.Side);
        bool upDownParry = (playerDir == ActionDirection.Up && enemyDir == ActionDirection.Down) || (playerDir == ActionDirection.Down && enemyDir == ActionDirection.Up);
        return sideParry || upDownParry;
    }

    private void PerformAction(PlayerActionType actionType, Vector2 dragVector)
    {
        if (playerAttackHitbox != null) playerAttackHitbox.ResetState();


        ActionDirection animDirection = GetDirectionFromVector(dragVector);

        Vector2 pushDirection = Vector2.zero;
        float distance = 0f;
        string triggerName = "";

        switch (actionType)
        {
            case PlayerActionType.Attack:
                distance = attackDistance;
                if (animDirection == ActionDirection.Side)
                {

                    pushDirection = (dragVector.x > 0) ? Vector2.right : Vector2.left;
                    triggerName = "attackSide";
                }
                else // Nếu là đòn đánh dọc (Up hoặc Down)
                {
                    // Hướng lướt đi vẫn là phía trước mặt, logic này vẫn đúng.
                    pushDirection = playerMovement.IsFacingRight() ? Vector2.right : Vector2.left;
                    triggerName = (animDirection == ActionDirection.Up) ? "attackUp" : "attackDown";
                }
                break;

            case PlayerActionType.Dodge:
                distance = dodgeDistance;
                if (animDirection == ActionDirection.Side)
                {
                    pushDirection = (dragVector.x > 0) ? Vector2.right : Vector2.left;
                    triggerName = "dodgeSide";
                }
                else
                {
                    pushDirection = (animDirection == ActionDirection.Up) ? Vector2.up : Vector2.down;
                    distance = 0f; // Né dọc không di chuyển
                    triggerName = (animDirection == ActionDirection.Up) ? "dodgeUp" : "dodgeDown";
                }
                break;
        }

        animator.SetTrigger(triggerName);

        playerMovement.StartActionPush(pushDirection, distance);
    }

    public void EnableHitbox(ActionDirection direction)
    {
        if (playerAttackHitbox == null) return;
        playerAttackHitbox.attackDirection = direction;
        playerAttackHitbox.ownerStats = GetComponent<CharacterStats>();
        playerAttackHitbox.intent = nextAttackIntent;

        nextAttackIntent = AttackIntent.Normal;
    }

    public void ExecuteBufferedAction()
    {
        if (bufferedAction != PlayerActionType.None)
        {
            PerformAction(bufferedAction, bufferedDragVector);
            bufferedAction = PlayerActionType.None;
        }
    }

    public void ClearBufferedAction()
    {
        bufferedAction = PlayerActionType.None;
    }

    private ActionDirection GetDirectionFromVector(Vector2 dragVector)
    {
        float xAbs = Mathf.Abs(dragVector.x);
        float yAbs = Mathf.Abs(dragVector.y);
        if (xAbs > yAbs * verticalBias) return ActionDirection.Side;
        else return (dragVector.y > 0) ? ActionDirection.Up : ActionDirection.Down;
    }

    // HÀM NÀY GIỜ SẼ TÌM ĐÚNG COMPONENT
    private EnemyAI FindNearbyEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, parryDetectionRange);
        foreach (var hit in hits)
        {
            // Kiểm tra xem có phải là EnemyAttackHitbox không
            EnemyAttackHitbox enemyHitbox = hit.GetComponent<EnemyAttackHitbox>();
            if (enemyHitbox != null)
            {
                // Nếu đúng, lấy EnemyAI từ chủ nhân của nó
                EnemyAI enemy = enemyHitbox.ownerStats.GetComponent<EnemyAI>();
                if (enemy != null && enemy.GetCurrentState() == AIState.ExecutingCombo)
                {
                    return enemy;
                }
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, parryDetectionRange);
    }
}