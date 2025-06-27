using UnityEngine;

/// <summary>
/// Script đánh dấu một Collider là vùng có thể nhận sát thương (Hurtbox).
/// Nó chứa tham chiếu đến CharacterStats gốc của nhân vật.
/// </summary>
public class Hurtbox : MonoBehaviour
{
    // Kéo component CharacterStats của nhân vật (Player hoặc Enemy) vào đây
    public CharacterStats characterStats;
}