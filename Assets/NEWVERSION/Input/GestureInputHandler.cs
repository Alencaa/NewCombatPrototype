using UnityEngine;
using System.Collections.Generic;

namespace CombatV2.InputSystem
{
    public class GestureInputHandler : MonoBehaviour
    {
        public float minSwipeDistance = 50f;
        public float maxGestureTime = 0.5f;

        private Vector2 startPos;
        private float startTime;
        private bool isDragging;
        private List<string> gestureSequence = new();

        public List<string> CurrentGesture => new List<string>(gestureSequence);

        public void ResetGesture()
        {
            gestureSequence.Clear();
        }

        void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1)) // RMB for gesture (block/parry)
            {
                isDragging = true;
                startPos = UnityEngine.Input.mousePosition;
                startTime = Time.time;
            }

            if (UnityEngine.Input.GetMouseButtonUp(1))
            {
                isDragging = false;
                Vector2 endPos = UnityEngine.Input.mousePosition;
                float gestureTime = Time.time - startTime;

                if (gestureTime <= maxGestureTime && Vector2.Distance(startPos, endPos) >= minSwipeDistance)
                {
                    Vector2 direction = endPos - startPos;
                    string dir = GetDirection(direction);
                    gestureSequence.Add(dir);
                    Debug.Log($"Gesture Swipe: {dir}");
                }
            }
        }

        private string GetDirection(Vector2 dir)
        {
            dir.Normalize();
            if (Vector2.Dot(dir, Vector2.up) > 0.7f) return "UP";
            if (Vector2.Dot(dir, Vector2.down) > 0.7f) return "DOWN";
            if (Vector2.Dot(dir, Vector2.left) > 0.7f) return "LEFT";
            if (Vector2.Dot(dir, Vector2.right) > 0.7f) return "RIGHT";
            return "NONE";
        }
    }
}
