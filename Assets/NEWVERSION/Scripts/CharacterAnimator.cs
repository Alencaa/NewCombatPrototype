using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Play(string animationName)
    {
        if (anim != null)
            anim.Play(animationName);
    }

    public void SetBool(string param, bool value)
    {
        if (anim != null)
            anim.SetBool(param, value);
    }

    public void SetTrigger(string param)
    {
        if (anim != null)
            anim.SetTrigger(param);
    }
}
