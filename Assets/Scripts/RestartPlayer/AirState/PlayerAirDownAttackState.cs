using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 空中下劈攻击状态
/// 类似空洞骑士的下劈机制：
/// - 攻击时悬浮（重力降低）
/// - 命中敌人时弹跳
/// - 攻击结束进入降落状态
/// </summary>
public class PlayerAirDownAttackState : PlayerAirState
{
    private bool isAnimFinished;

    // 下劈配置
    private const float DownGravityScale = 2f;       // 下劈时重力（缓慢下落）
    private const float BounceForce = 12f;             // 命中敌人弹跳力

    public PlayerAirDownAttackState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        isAnimFinished = false;

        // 设置攻击状态
        s.ctx.IsAttacking = true;
        s.ctx.IsDownAttacking = true;
        s.anim.SetIsDownAttacking(true);
        s.anim.SetIsAttacking(true);

        // 保存原始重力并设置下劈重力
        s.motor.CaptureOriginalGravity();
        s.motor.GravityScale = DownGravityScale;

        // 下劈时给予轻微向下初速度
        s.motor.SetVelocityY(-2f);

    }

    public override Transition LogicUpdate()
    {
        // 命中敌人弹跳
        if (s.ctx.DownAttackBounced)
        {
            s.motor.Jump(BounceForce);
            s.ctx.DownAttackBounced = false;
        }
        // 攻击时间结束或动画完成，进入降落状态
        if (isAnimFinished)
        {
            return new Transition(PlayerStateId.Fall);
        }

        // 落地直接进入地面状态
        if (s.ctx.IsGrounded)
        {
            s.anim.PlayIdle();
            return new Transition(PlayerStateId.Idle);
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        // 下劈时保持水平移动但逐渐减速
        s.motor.SetVelocityX(s.motor.Velocity.x * 0.9f);
    }

    public override void Exit()
    {
        base.Exit();

        // 恢复原始重力
        s.motor.RestoreOriginalGravity();

        // 清除攻击状态
        s.ctx.IsAttacking = false;
        s.ctx.IsDownAttacking = false;
        s.anim.SetIsDownAttacking(false);
        s.anim.SetIsAttacking(false);
    }

    /// <summary>
    /// 动画事件回调：下劈动画完成
    /// </summary>
    public void OnAirDownAttackAnimFinished()
    {
        isAnimFinished = true;
    }
}