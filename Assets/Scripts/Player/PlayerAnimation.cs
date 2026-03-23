using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    PhysicsCheck check;
    PlayerController playerController;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        check = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        SetAnimations();
    }

    public void SetAnimations()
    {
        animator.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("velocityY", rb.velocity.y);
        animator.SetBool("IsGround", check.IsGround);
        animator.SetBool("IsDeath", playerController.IsDeath);
        animator.SetBool("IsAttack", playerController.IsAttack);
        animator.SetBool("Dash", playerController.IsDash);
    }
    /// <summary>
    /// 播放受伤的动画
    /// </summary>
    public void Hurt()
    {
        animator.SetTrigger("Hurt");
    }
    /// <summary>
    /// 播放攻击动画
    /// </summary>
    public void Attack()
    {
        animator.SetTrigger("attack");
    }


    /// <summary>
    /// 第三段攻击距离额外补偿
    /// </summary>
    public void Attack_Move()
    {
        gameObject.transform.position = new Vector3(transform.position.x + transform.localScale.x * 0.5f, transform.position.y, transform.position.z);
    }
}
