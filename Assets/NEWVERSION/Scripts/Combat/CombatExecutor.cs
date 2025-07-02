using UnityEngine;
using System.Collections.Generic;
using CombatV2.FSM;

namespace CombatV2.Player
{
    public class CombatExecutor : MonoBehaviour
    {
        [SerializeField] private GestureInputHandler gestureInput;
        [SerializeField] private PlayerCombatConfig playerCombatConfig;

        private Dictionary<string, List<GestureType>> comboLookup;

        private List<GestureData> currentActiveAttacks = new(); // optional: để cancel nếu combo triggered

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
            Debug.Log($"🗡️ Attack executed: {gesture.type}");
            currentActiveAttacks.Add(gesture);
            ExecuteAttack(gesture);
        }

        void HandleCombo(List<GestureData> combo)
        {
            if (MatchCombo(combo, out string comboName))
            {
                Debug.Log($"🔥 Combo triggered: {comboName}");

                // Optional: Hủy animation đòn đơn trước đó (tuỳ hệ thống animator của bạn)
                CancelCurrentAttacks();

                ExecuteCombo(comboName, combo);
                // TODO insert SFX combo recording
            }
            else
            {
                Debug.Log("❌ Combo failed!");
                // TODO insert SFX combo fail
                // Không gọi lại attack đơn
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

        void ExecuteCombo(string comboName, List<GestureData> comboSteps)
        {
            Debug.Log($"[EXECUTE COMBO] {comboName} with {comboSteps.Count} steps");
            // Play combo animation, apply damage pattern, etc.
        }

        void ExecuteAttack(GestureData gesture)
        {
            // Play single attack animation, apply hitbox, etc.
        }

        void CancelCurrentAttacks()
        {
            currentActiveAttacks.Clear();
            // Optionally trigger animation cancel / reset
        }
    }
}
