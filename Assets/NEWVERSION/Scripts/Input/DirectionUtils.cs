using UnityEngine;

public static class DirectionUtils
{
    /// <summary>
    /// Kiểm tra 2 vector hướng có khớp nhau trong ngưỡng cho phép
    /// </summary>
    public static bool IsDirectionMatch(Vector2 gestureDir, Vector2 attackDir, float tolerance = 45f)
    {
        gestureDir.Normalize();
        attackDir.Normalize();

        float angle = Vector2.Angle(gestureDir, attackDir);
        return angle <= tolerance;
    }
}