using UnityEngine;

public class NPC : MonoBehaviour
{
    public string shakeHeadTrigger = "shake";
    public string clapTrigger = "clapping";

    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void PlayRandomReaction()
    {
        if (anim == null) return;
        if (Random.value < 0.5f) anim.SetBool(shakeHeadTrigger,true);
        else anim.SetBool(clapTrigger,true);
    }
}
