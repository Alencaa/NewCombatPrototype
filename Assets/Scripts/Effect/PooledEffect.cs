using UnityEngine;

/// <summary>
/// Script này được gắn vào mỗi Prefab hiệu ứng có trong Pool.
/// Nó có thể tự động tắt mình đi sau khi Animation Clip chạy xong,
/// hoặc sau khi Particle System chạy xong.
/// </summary>
public class PooledEffect : MonoBehaviour
{
    // --- PHƯƠNG PHÁP 1: DÀNH CHO HIỆU ỨNG DÙNG ANIMATION ---

    /// <summary>
    /// Hàm này sẽ được gọi bởi một Animation Event đặt ở cuối animation clip.
    /// </summary>
    public void OnAnimationFinished()
    {
        // Khi animation chạy xong, nó sẽ tự tắt mình đi để quay về pool.
        gameObject.SetActive(false);
    }


    // --- PHƯƠNG PHÁP 2: DÀNH CHO HIỆU ỨNG DÙNG PARTICLE SYSTEM ---

    /// <summary>
    /// Hàm này được Unity tự động gọi khi một Particle System trên cùng object này dừng lại,
    /// với điều kiện Stop Action của nó được đặt là "Callback".
    /// </summary>
    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }
}