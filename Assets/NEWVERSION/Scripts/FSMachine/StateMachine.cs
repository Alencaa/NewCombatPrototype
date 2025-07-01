using CombatV2.Enemy;
using System.Collections;
using UnityEngine;

namespace CombatV2.FSM
{
    public class StateMachine<T> where T : MonoBehaviour
    {
        //private EnemyController enemyController;

        //public StateMachine(EnemyController enemyController)
        //{
        //    this.enemyController = enemyController;
        //}
        protected T Owner;
        protected CharacterState<T> currentState;
        public StateMachine(T owner)
        {
            this.Owner = owner;
        }


        public CharacterState<T> CurrentState { get; private set; }

        public void Initialize(CharacterState<T> startState)
        {
            CurrentState = startState;
            CurrentState.Enter();
        }

        public void ChangeState(CharacterState<T> newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
        public void ChangeState(CharacterState<T> newState, float delay)
        {
            Owner.StartCoroutine(ChangeStateAfterDelay(newState, delay));
        }

        private IEnumerator ChangeStateAfterDelay(CharacterState<T> newState, float delay)
        {
            yield return new WaitForSeconds(delay);
            ChangeState(newState);
        }

        public void Update()
        {
            CurrentState?.Update();
        }

        public void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }
    }
}
