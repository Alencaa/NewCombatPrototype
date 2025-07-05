using CombatV2.Combat;
using CombatV2.FSM;
using CombatV2.FSM.States;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace CombatV2.Enemy
{
    [RequireComponent(typeof(MovementController), typeof(EnemyCombatHandler), typeof(CharacterAnimator))]
    public class EnemyController : MonoBehaviour, IAttackable
    {
        [Header("Combat Settings")]
        public bool isComboEnemy = false;
        public EnemyCombatConfig config;
        public Transform player;
        public float currentPosture;

        public bool IsInvicible { get; set; } = false; // Biến để xác định enemy có đang trong trạng thái bất khả xâm phạm hay không
        // 🔗 Sub-systems
        public MovementController movement { get; private set; }
        public EnemyCombatHandler combat { get; private set; }
        public CharacterAnimator animator { get; private set; }

        public Vector2 lastAttackerPosition { get; private set; } // Lưu vị trí của kẻ tấn công cuối cùng

        // 🧠 FSM
        private StateMachine<EnemyController> stateMachine;
        public StateMachine<EnemyController> StateMachine => stateMachine;


        public bool IsInWindUp { get; set; } = false; // Biến để xác định enemy có đang trong trạng thái wind-up hay không

        // 📍 State flags
        public bool isBlocking;
        public bool isInParryWindow;

        private void Awake()
        {
            animator = GetComponent<CharacterAnimator>();
            combat = GetComponent<EnemyCombatHandler>();
            movement = GetComponent<MovementController>();

            stateMachine = new StateMachine<EnemyController>(this);
        }

        private void Start()
        {
            currentPosture = config.maxPosture;
            stateMachine.ChangeState(new EnemyIdleState(this, stateMachine));
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }

        //public void ApplyDamage(float damage, bool isHeavy)
        //{
        //    if (isBlocking)
        //    {
        //        if (isInParryWindow)
        //        {
        //            stateMachine.ChangeState(new EnemyParriedState(this, stateMachine));
        //            return;
        //        }

        //        float postureDamage = isHeavy ? damage * config.heavyAttackPostureMultiplier : damage;
        //        currentPosture -= postureDamage;

        //        if (currentPosture <= config.guardBreakThreshold)
        //        {
        //            stateMachine.ChangeState(new EnemyStaggerState(this, stateMachine));
        //        }
        //    }
        //    else
        //    {
        //        stateMachine.ChangeState(new EnemyStaggerState(this, stateMachine));
        //    }
        //}
        // 🔁 Chuyển về Idle state bằng coroutine delay
        public void TransitionToIdle(StateMachine<EnemyController> stateMachine, float delay = 0f)
        {
            StartCoroutine(WaitAndDo(delay, () =>
            {
                stateMachine.ChangeState(new EnemyIdleState(this, stateMachine));
            }));
        }

        // ⏳ Coroutine chờ rồi thực thi
        public System.Collections.IEnumerator WaitAndDo(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        public void OnHitReceived(AttackData data, HitRegionType region, Vector2 fromPos)
        {
            if (IsInvicible)
            {
                Debug.Log("Enemy is invincible, ignoring hit.");
                return;
            }

            lastAttackerPosition = fromPos;

            // 🔍 Xác định có phải Counter Hit không
            // 🔥 CHỈ Counter Hit nếu bị đánh trong wind-up window của enemy
            bool isCounterHit = IsInWindUp;

            if (isCounterHit)
            {
                Debug.Log($"🔥 Counter Hit vào {region} bởi {data.attackName}");
                StateMachine.ChangeState(new EnemyStaggerState(this, StateMachine, data, region));
            }
            else
            {
                Debug.Log($"💢 Enemy hit (normal) vào {region} bởi {data.attackName}");
                StateMachine.ChangeState(new EnemyHitReactState(this, StateMachine, data, region));
            }
        }


    }
}
