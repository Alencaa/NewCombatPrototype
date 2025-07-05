using CombatV2.FSM;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CombatV2.FSM.States;

namespace CombatV2.Player
{
    public class CombatExecutor : MonoBehaviour
    {
        [SerializeField] private GestureInputHandler gestureInput;
        [SerializeField] private PlayerCombatConfig playerCombatConfig;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private TextMeshPro debugText;

        private Dictionary<string, List<GestureType>> comboLookup;
        private List<GestureData> currentActiveAttacks = new();

        private void Awake()
        {
            comboLookup = new Dictionary<string, List<GestureType>>();
            foreach (var namedCombo in playerCombatConfig.combos)
            {
                comboLookup[namedCombo.comboName] = namedCombo.pattern;
            }
        }

        private void OnEnable()
        {
            gestureInput.OnBlockStart += EnterBlockState;
            gestureInput.OnBlockDirectionChanged += UpdateBlockDirection;
            gestureInput.OnBlockEnd += ExitBlockState;

            gestureInput.OnComboRecognized += HandleCombo;
            gestureInput.OnGestureRecognized += HandleGesture;
        }

        private void OnDisable()
        {
            gestureInput.OnComboRecognized -= HandleCombo;
            gestureInput.OnGestureRecognized -= HandleGesture;
        }

        void HandleGesture(GestureData gesture)
        {
            switch (gesture.type)
            {
                case GestureType.Block:
                    HandleBlock(gesture);
                    break;

                case GestureType.Parry:
                    HandleParry(gesture);
                    break;

                default:
                    Debug.Log($"🗡️ Attack executed: {gesture.type}");
                    currentActiveAttacks.Add(gesture);
                    //gán buffer cho gesture
                    if (playerController.StateMachine.CurrentState is PlayerAttackState atkState &&  atkState.IsAcceptingBufferedInput == false || playerController.StateMachine.CurrentState is PlayerComboState comboState) // giai đoạn AttackActive
                    {
                        Debug.Log("🔄 Gesture buffered during AttackActive.");
                        playerController.InputBuffer.BufferGesture(gesture);
                    }
                    else
                    {
                        playerController.RequestAttackState(gesture);
                    }
                        break;
            }
        }

        #region COMBO HANDLING

        void HandleCombo(List<GestureData> combo)
        {
            if (MatchCombo(combo, out string comboName))
            {
                Debug.Log($"🔥 Combo triggered: {comboName}");
                CancelCurrentAttacks();

                playerController.StateMachine.ChangeState(
                    new PlayerComboState(playerController, playerController.StateMachine, comboName, combo)
                );

                // TODO insert SFX combo recording
            }
            else
            {
                Debug.Log("❌ Combo failed!");
                // TODO insert SFX combo fail
            }
        }

        bool MatchCombo(List<GestureData> combo, out string comboName)
        {
            foreach (var pair in comboLookup)
            {
                var pattern = pair.Value;
                if (pattern.Count != combo.Count) continue;

                bool match = true;
                for (int i = 0; i < pattern.Count; i++)
                {
                    if (combo[i].type != pattern[i])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    comboName = pair.Key;
                    return true;
                }
            }

            comboName = null;
            return false;
        }

        #endregion

        #region BLOCK / PARRY HANDLING

        void HandleBlock(GestureData gesture)
        {
            // do nothing – handled by FSM PlayerBlockState
            Debug.Log("⚠️ Unexpected: HandleBlock() called by gesture?");
        }

        void HandleParry(GestureData gesture)
        {
            playerController.LastParryGesture = gesture.type;
            playerController.StateMachine.ChangeState(
                new PlayerParryState(playerController, playerController.StateMachine, gesture)
            );
        }

        void EnterBlockState()
        {
            playerController.IsHoldingBlock = true;
            playerController.StateMachine.ChangeState(
                new PlayerBlockState(playerController, playerController.StateMachine)
            );
        }

        void UpdateBlockDirection(Vector2 dir)
        {
            playerController.CurrentBlockDirection = dir;

        }

        void ExitBlockState()
        {
            playerController.IsHoldingBlock = false;
        }

        #endregion

        void CancelCurrentAttacks()
        {
            currentActiveAttacks.Clear();
            // Optionally trigger animation cancel / reset
        }
    }
}
