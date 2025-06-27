namespace CombatV2.FSM
{
    public abstract class CharacterState<T>
    {
        protected T Owner { get; }
        protected StateMachine<T> stateMachine;

        public CharacterState(T owner, StateMachine<T> stateMachine)
        {
            Owner = owner;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}
