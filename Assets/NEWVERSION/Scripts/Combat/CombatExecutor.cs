using UnityEngine;
using CombatV2.FSM;

namespace CombatV2.Player
{
    public class CombatExecutor : MonoBehaviour
    {
        [SerializeField] private GestureInputHandler gestureInput;

        private void OnEnable()
        {
            gestureInput.OnGestureRecognized += HandleGesture;
        }

        private void OnDisable()
        {
            gestureInput.OnGestureRecognized -= HandleGesture;
        }

        void HandleGesture(GestureData gesture)
        {
            switch (gesture.type)
            {
                case GestureType.SlashUp:
                case GestureType.SlashDown:
                case GestureType.SlashLeft:
                case GestureType.SlashRight:
                case GestureType.SlashUpLeft:
                case GestureType.SlashUpRight:
                case GestureType.SlashDownLeft:
                case GestureType.SlashDownRight:
                    ExecuteAttack(gesture);
                    break;
                case GestureType.Parry:
                    TryParry(gesture);
                    break;
                case GestureType.Block:
                    HoldBlock(gesture);
                    break;
            }
        }

        void ExecuteAttack(GestureData gesture)
        {
            Debug.Log($"Attack executed: {gesture.type}");
            // play animation, send damage, etc.
        }

        void TryParry(GestureData gesture)
        {
            Debug.Log($"Parry attempted: {gesture.type} + {gesture.direction}");
            // activate parry logic
        }

        void HoldBlock(GestureData gesture)
        {
            Debug.Log($"Blocking toward: {gesture.direction}");
            // set blocking state, block direction, etc.
        }
    }

}
