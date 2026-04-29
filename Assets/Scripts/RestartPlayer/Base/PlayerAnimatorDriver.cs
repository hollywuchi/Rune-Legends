using UnityEngine;

/// <summary>
/// PlayerAnimatorDriver封装了对Animator的操作，提供了更语义化的方法来控制玩家的动画。
/// 同时配合Unity的动画系统（Animator Controller）来实现状态机和动画过渡。
/// </summary>
[RequireComponent(typeof(Animator))]
public sealed class PlayerAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void SetInputX(float absX) => animator.SetFloat("InputX", absX);
    public void SetIsGround(bool isGround) => animator.SetBool("IsGround", isGround);
    public void SetIsWall(bool isWall) => animator.SetBool("IsWall", isWall);
    public void SetVelocityY(float yClamped) => animator.SetFloat("VecocityY", yClamped);

    public void ResetCommonTriggers()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Ing");
    }

    public void TriggerIdle() => animator.SetTrigger("Idle");
    public void TriggerIng() => animator.SetTrigger("Ing");
    public void TriggerClimb() => animator.SetTrigger("Climb");
    public void TriggerCanLand() => animator.SetTrigger("CanLand");

    public void PlayToSprint() => animator.Play("ToSprint");
    public void PlayTrickTurn() => animator.Play("TrickTurn");
    public void PlayLand() => animator.Play("Land", 0, 0);

    public void CrossFadeToSJump(float duration = 0.05f) => animator.CrossFade("ToSJump", duration);
}