using UnityEngine;

namespace CombatV2.FSM.States
{
    /// <summary>
    /// Trạng thái Idle của người chơi, không có hành động gì đặc biệt.
    /// </summary>
    /// <remarks>
    /// Trạng thái này sẽ được sử dụng làm trạng thái mặc định khi không có hành động nào khác.
    /// </remarks>
    public class PlayerIdleState: CharacterState<PlayerController>
    {
        public PlayerIdleState(PlayerController owner, StateMachine<PlayerController> stateMachine)
            : base(owner, stateMachine) { }
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered Idle State");
            Owner.Animator.Play("Idle"); // Giả sử có animation "Idle"
        }
        public override void Update()
        {
            base.Update();
            // Có thể thêm logic kiểm tra điều kiện chuyển sang trạng thái khác nếu cần
        }
    }
}