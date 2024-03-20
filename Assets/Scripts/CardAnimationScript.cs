using System.Collections;
using UnityEngine;

public class CardAnimationScript : MonoBehaviour
{
    [Header("Card Animation Settings")]
    public Animator animator;

    public void Anim()
    {
        animator.SetBool("IsWrong", true);
    }

}
