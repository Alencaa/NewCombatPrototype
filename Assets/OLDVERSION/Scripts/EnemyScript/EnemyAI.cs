using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Các định nghĩa này có thể đặt ở đầu file hoặc trong một file riêng
[System.Serializable]
public class AICombo
{
    public string comboName = "New Combo";
    public List<ActionDirection> sequence;
    [Tooltip("Tỉ lệ ra đòn của combo này so với các combo khác.")]
    public float weight = 1f;
}

public enum AIState
{
    Patrolling,
    Chasing,
    ExecutingCombo,
    Vulnerable
}

public enum ActionDirection
{
    Up,
    Down,
    Side
}


public class EnemyAI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private AttackHitbox attackHitbox; // Thêm tham chiếu này

    [Header("AI Behavior")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float vulnerableTime = 2f;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Combat")]
    public List<AICombo> comboList;
    [SerializeField] private float attackDistance = 0.5f;
    public float clashFrameTime = 0.5f;

    private AIState currentState;
    private float vulnerableTimer;
    private Coroutine activeComboCoroutine;
    public ActionDirection currentAttackDirection { get; private set; }

    void Start()
    {
        // Tự động lấy các component nếu chưa gán trong Inspector
        if (enemyMovement == null) enemyMovement = GetComponent<EnemyMovement>();
        if (animator == null) animator = GetComponent<Animator>();
        if (attackHitbox == null) attackHitbox = GetComponentInChildren<AttackHitbox>();
        if (playerTransform == null)
        {
            // Tự động tìm GameObject có tag "Player"
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogError("Không tìm thấy Player! Hãy chắc chắn object người chơi có tag 'Player'.", this);
            }
        }

        currentState = AIState.Patrolling;
    }

    void Update()
    {
        if (playerTransform == null || (enemyMovement != null && enemyMovement.IsMovementLocked()))
        {
            return;
        }

        // Máy trạng thái chính
        switch (currentState)
        {
            case AIState.Patrolling:
                UpdatePatrolState();
                break;
            case AIState.Chasing:
                UpdateChaseState();
                break;
            case AIState.Vulnerable:
                UpdateVulnerableState();
                break;
        }
    }

    private void UpdatePatrolState()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) < detectionRange)
        {
            currentState = AIState.Chasing;
            return;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            enemyMovement.Move(0);
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        else
        {
            enemyMovement.FaceTarget(targetPoint.position);
            float direction = Mathf.Sign(targetPoint.position.x - transform.position.x);
            enemyMovement.Move(direction);
        }
    }

    private void UpdateChaseState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > detectionRange)
        {
            currentState = AIState.Patrolling;
            enemyMovement.Move(0);
            return;
        }

        if (distanceToPlayer < attackRange)
        {
            currentState = AIState.ExecutingCombo;
            enemyMovement.Move(0);
            activeComboCoroutine = StartCoroutine(PerformCombo());
        }
        else
        {
            enemyMovement.FaceTarget(playerTransform.position);
            float direction = (playerTransform.position.x > transform.position.x) ? 1 : -1;
            enemyMovement.Move(direction);
        }
    }

    private void UpdateVulnerableState()
    {
        vulnerableTimer -= Time.deltaTime;
        if (vulnerableTimer <= 0)
        {
            currentState = AIState.Chasing;
        }
    }

    private IEnumerator PerformCombo()
    {
        AICombo chosenCombo = ChooseRandomCombo();
        if (chosenCombo == null)
        {
            currentState = AIState.Vulnerable;
            vulnerableTimer = vulnerableTime / 2;
            yield break;
        }

        foreach (ActionDirection attackDir in chosenCombo.sequence)
        {
            if (currentState != AIState.ExecutingCombo) yield break;

            enemyMovement.FaceTarget(playerTransform.position);
            PerformSingleAttack(attackDir);
            yield return new WaitUntil(() => !enemyMovement.IsMovementLocked());
            yield return new WaitForSeconds(0.2f);
        }

        currentState = AIState.Vulnerable;
        vulnerableTimer = vulnerableTime;
    }

    private void PerformSingleAttack(ActionDirection direction)
    {
        if (attackHitbox != null)
        {
            attackHitbox.ResetState();
        }

        this.currentAttackDirection = direction;

        Vector2 pushDirection = enemyMovement.IsFacingRight() ? Vector2.right : Vector2.left;
        string triggerName = "";

        switch (direction)
        {
            case ActionDirection.Up: triggerName = "attackUp"; break;
            case ActionDirection.Down: triggerName = "attackDown"; break;
            case ActionDirection.Side: triggerName = "attackSide"; break;
        }

        animator.SetTrigger(triggerName);
        enemyMovement.StartActionPush(pushDirection, attackDistance);
    }

    private AICombo ChooseRandomCombo()
    {
        if (comboList == null || comboList.Count == 0) return null;
        float totalWeight = comboList.Sum(c => c.weight);
        float randomPoint = Random.Range(0, totalWeight);
        foreach (AICombo combo in comboList)
        {
            if (randomPoint < combo.weight) return combo;
            randomPoint -= combo.weight;
        }
        return comboList.Last();
    }

    // --- CÁC HÀM CÔNG KHAI CHO HỆ THỐNG PARRY ---

    public AIState GetCurrentState() => currentState;

    public void TriggerParryStagger()
    {

        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        int stateHash = currentStateInfo.fullPathHash;
        //animator.Play(stateHash, -1, this.clashFrameTime);

        Debug.Log("Enemy animation snapped to " + this.clashFrameTime + " due to Perfect Parry.");
    }
    public void SetClashFrameTime(float time)
    {
        this.clashFrameTime = time;
    }
}