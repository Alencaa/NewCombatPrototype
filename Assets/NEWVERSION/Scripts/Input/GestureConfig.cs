using UnityEngine;

[CreateAssetMenu(fileName = "GestureConfig", menuName = "Combat/GestureConfig")]
public class GestureConfig : ScriptableObject
{
    [Header("Gesture Settings")]
    public float gestureThreshold = 50f;              // Minimum distance to recognize gesture
    public float parrySpeedThreshold = 800f;          // Minimum speed to detect parry
    public float comboResetDistance = 30f;            // Distance to continue combo
    public float holdBlockMinDuration = 0.2f;         // How long to hold before registering block
}
