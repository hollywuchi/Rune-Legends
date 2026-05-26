using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 空中上劈攻击状态
/// - 攻击时给予上升初速度
/// - 攻击期间重力降低（悬停效果）
/// - 攻击结束进入降落状态
/// </summary>
public class PlayerAirUpAttackState : PlayerAirState
{
    private bool isAnimFinished;
    
    // 上劈配置
    private const float UpGravityScale = 2f;         // 上劈时重力（悬停效果）
    public PlayerAirUpAttackState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        isAnimFinished = false;

        // 设置攻击状态
        s.ctx.IsAttacking = true;
        s.ctx.IsUpAttacking = true;
        s.anim.SetIsUpAttacking(true);
        s.anim.SetIsAttacking(true);

        // 保存原始重力并设置上劈重力
        s.motor.CaptureOriginalGravity();
        s.motor.GravityScale = UpGravityScale;

        // 水平速度归零
        s.motor.SetVelocityX(0f);

    }

    public override Transition LogicUpdate()
    {

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
        // 上劈时水平移动受限
        s.motor.SetVelocityX(s.motor.Velocity.x * 0.8f);
    }

    public override void Exit()
    {
        base.Exit();
        
        // 恢复原始重力
        s.motor.RestoreOriginalGravity();
        
        // 清除攻击状态
        s.ctx.IsAttacking = false;
        s.ctx.IsUpAttacking = false;
        s.anim.SetIsUpAttacking(false);
        s.anim.SetIsAttacking(false);
    }

    /// <summary>
    /// 动画事件回调：上劈动画完成
    /// </summary>
    public void OnAirUpAttackAnimFinished()
    {
        isAnimFinished = true;
    }
}