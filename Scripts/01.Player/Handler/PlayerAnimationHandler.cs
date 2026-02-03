using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private int isDead = Animator.StringToHash("IsDead");
    private int isMove = Animator.StringToHash("IsMove");

    [SerializeField] private Animator animator;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.Player.AnimationHandler = this;
    }

    public void OnMove(bool b)
    {
        animator.SetBool(isMove, b);
    }

    public void OnDead()
    {
        animator.SetBool(isDead, true);
    }

    public void OnDeadEvent()
    {
        GameManager.Instance.EndGame();
    }
}
