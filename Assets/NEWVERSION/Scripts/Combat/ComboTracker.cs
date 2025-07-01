using System;
using UnityEngine;

namespace CombatV2.Combat
{
    public class ComboTracker : MonoBehaviour
    {
        [Header("Combo Config")]
        public float comboTimeWindow = 1.2f;
        public int maxComboLength = 3;

        [Header("Debug Info")]
        public int currentComboStep = 0;
        private float lastHitTime = -999f;

        public event Action OnComboReset;
        public event Action<int> OnComboProgress;

        public void RegisterHit()
        {
            float now = Time.time;

            if (now - lastHitTime <= comboTimeWindow)
            {
                currentComboStep++;
            }
            else
            {
                currentComboStep = 1; // restart combo
            }

            lastHitTime = now;
            OnComboProgress?.Invoke(currentComboStep);
        }

        public bool IsFinisherReady()
        {
            return currentComboStep >= maxComboLength;
        }

        public void ResetCombo()
        {
            currentComboStep = 0;
            lastHitTime = -999f;
            OnComboReset?.Invoke();
        }
    }
}
