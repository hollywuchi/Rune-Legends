using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 空中攻击状态
/// 继承PlayerAirState，复用空中状态的通用逻辑
/// 攻击结束后直接进入降落状态（Fall）
/// </summary>
public class PlayerAirAttackState : PlayerAirState
{
    private bool isAnimFinished;
    
    // 空中攻击配置
    private const float GravityScale = 2f;        // 攻击时重力（缓慢下落）
    // private const float HorizontalDeceleration = 0.8f; // 水平减速系数

    public PlayerAirAttackState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        isAnimFinished = false;

        // 设置攻击状态
        s.ctx.IsAttacking = true;
        s.anim.SetIsAttacking(true);
        s.anim.TriggerAttack();

        // 保存原始重力并设置攻击时重力
        s.motor.CaptureOriginalGravity();
        s.motor.GravityScale = GravityScale;

        Debug.Log("空中攻击状态");
    }

    public override Transition LogicUpdate()
    {
        base.LogicUpdate();

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

        // 注意：不调用base.LogicUpdate()，因为空中攻击时不应该触发二段跳、空中冲刺等
        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        // 空中攻击时保持轻微水平移动
        s.motor.SetVelocityX(s.motor.Velocity.x * 0.95f);
    }

    public override void Exit()
    {
        base.Exit();
        
        // 恢复原始重力
        s.motor.RestoreOriginalGravity();
        
        // 清除攻击状态
        s.ctx.IsAttacking = false;
        s.anim.SetIsAttacking(false);

        // s.anim.InterruptAnim("Idle");
    }

    /// <summary>
    /// 动画事件回调：空中攻击动画完成
    /// </summary>
    public void OnAirAttackAnimFinished()
    {
        isAnimFinished = true;
    }
}