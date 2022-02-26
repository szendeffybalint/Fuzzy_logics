using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator Animator;

    private void Update()
    {
        Animator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        Animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
    }
}
