using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public static PlayerAnimation instance;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        ChooseAnimation();
    }

    private void ChooseAnimation()
    {
        anim.SetBool("IsIdle", false);
        anim.SetBool("IsRun", false);
        anim.SetBool("IsCrafting", false);
        anim.SetBool("IsHurt", false);
        anim.SetBool("IsDied", false);

        switch (PlayerController.instance.currentState)
        {
            case PlayerController.PlayerState.Idle:
                anim.SetBool("IsIdle", true);
                break;
            case PlayerController.PlayerState.Run:
                anim.SetBool("IsRun", true);
                break;
            case PlayerController.PlayerState.Crafting:
                anim.SetBool("IsCrafting", true);
                break;
            case PlayerController.PlayerState.Hurt:
                anim.SetBool("IsHurt", true);
                break;
            case PlayerController.PlayerState.Died:
                anim.SetBool("IsDied", true);
                break;
        }
    }
}
