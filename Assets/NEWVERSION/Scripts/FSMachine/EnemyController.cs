using CombatV2.FSM;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace CombatV2.Enemy
{
    [RequireComponent(typeof(MovementController), typeof(EnemyCombatHandler), typeof(CharacterAnimator))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Combat Settings")]
        public bool isComboEnemy = false;
        public EnemyCombatConfig config;
        public Transform attackPoint;
        public Transform player;
        public float currentPosture;

        // 🔗 Sub-systems
        public MovementController movement { get; private set; }
        public EnemyCombatHandler combat { get; private set; }
        public CharacterAnimator animator { get; private set; }

        // 🧠 FSM
        private StateMachine<EnemyController> stateMachine;
        public StateMachine<EnemyController> StateMachine => stateMachine;

        public List<string> comboPattern => config.comboPattern;

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

        public void ApplyDamage(float damage, bool isHeavy)
        {
            if (isBlocking)
            {
                if (isInParryWindow)
                {
                    stateMachine.ChangeState(new EnemyParriedState(this, stateMachine));
                    return;
                }

                float postureDamage = isHeavy ? damage * config.heavyAttackPostureMultiplier : damage;
                currentPosture -= postureDamage;

                if (currentPosture <= config.guardBreakThreshold)
                {
                    stateMachine.ChangeState(new EnemyStaggerState(this, stateMachine));
                }
            }
            else
            {
                stateMachine.ChangeState(new EnemyStaggerState(this, stateMachine));
            }
        }
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

    }
}
