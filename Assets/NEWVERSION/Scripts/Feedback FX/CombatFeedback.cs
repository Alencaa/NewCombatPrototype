using UnityEngine;

namespace CombatV2.Combat
{
    public class CombatFeedback : MonoBehaviour
    {
        public GameObject hitEffectPrefab;
        public AudioClip hitSound;
        public float shakeIntensity = 0.2f;
        public float shakeDuration = 0.1f;

        public void PlayFeedback(Vector2 position)
        {
            // 1. Spawn FX
            if (hitEffectPrefab)
                Instantiate(hitEffectPrefab, position, Quaternion.identity);

            // 2. Play SFX
            if (hitSound)
                AudioSource.PlayClipAtPoint(hitSound, position);

            // 3. Camera shake
            CameraShaker.Instance.Shake(shakeIntensity, shakeDuration);
        }
    }
}
