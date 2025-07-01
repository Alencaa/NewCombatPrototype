using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Stop()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    public void Move(Vector2 direction, float speed)
    {
        if (rb != null)
            rb.linearVelocity = direction.normalized * speed;
    }
}
