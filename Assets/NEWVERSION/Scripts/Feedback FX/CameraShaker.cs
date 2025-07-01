using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;
    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.position;
    }

    public void Shake(float intensity, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private System.Collections.IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * intensity;
            offset.z = 0f;
            transform.position = originalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
