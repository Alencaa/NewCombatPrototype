using UnityEngine;

namespace CombatV2.FSM
{
    public class StateMachine<T>
    {
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
