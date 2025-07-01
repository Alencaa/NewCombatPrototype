using CombatV2.FSM;
using System.Collections.Generic;
using UnityEngine;

namespace CombatV2.Enemy
{
    // 🎮 Controller chính cho enemy, điều khiển trạng thái và hành vi AI
    public class EnemyController : MonoBehaviour
    {
        public Transform attackPoint; // Điểm va chạm

        public bool isComboEnemy = false; // Gán qua Inspector hoặc runtime
        public List<string> comboPattern; // Optional: để chỉ combo nào đánh

        [Header("References")]
        public Transform player; // Reference tới player để AI theo dõi

        private Animator animator;
        private Rigidbody2D rb;

        // 💡 FSM quản lý các trạng thái của enemy (Idle, Chase, Attack,...)
        private StateMachine<EnemyController> stateMachine;

        // 🧠 Property dùng bởi các state để truy cập controller
        public StateMachine<EnemyController> StateMachine => stateMachine;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();

            // ⚙️ Khởi tạo FSM
            stateMachine = new StateMachine<EnemyController>(this);
        }

        private void Start()
        {
            // 🌟 Bắt đầu với trạng thái Idle
            stateMachine.ChangeState(new EnemyIdleState(this, stateMachine));
        }

        private void Update()
        {
            // 🔁 Gọi update của FSM mỗi frame
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        // 🔄 Hàm hỗ trợ chơi animation theo tên
        public void PlayAnimation(string animName)
        {
            if (animator != null)
            {
                animator.Play(animName);
            }
        }

        // 🚶 Di chuyển enemy về phía target
        public void MoveToward(Vector3 target)
        {
            Vector2 direction = (target - transform.position).normalized;
            rb.MovePosition(rb.position + direction * 2f * Time.deltaTime); // tốc độ có thể chỉnh sửa
        }
    }
}
