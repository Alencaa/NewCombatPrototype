using UnityEngine;

public class EnemyCombatHandler : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;
    public bool IsHitboxOpen => hitbox != null && hitbox.activeSelf;

    public void SetHitboxState(bool isOpen)
    {
        if (hitbox != null)
            hitbox.SetActive(isOpen);
    }

    public void TriggerStaggerFeedback()
    {
        // Add VFX, SFX, animation trigger etc.
        Debug.Log("Stagger feedback triggered!");
    }

    public void TriggerParryFeedback()
    {
        Debug.Log("Parry feedback triggered!");
    }

    public void ResetCombatState()
    {
        SetHitboxState(false);
    }
}
