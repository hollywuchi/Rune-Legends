using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 受伤状态
/// 受伤时播放动画，冻结操作，不可被打断
/// </summary>
public class PlayerHurtState : PlayerState
{
    private float hurtTimer;

    public PlayerHurtState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        
        // 设置受伤状态
        s.ctx.IsHurt = true;
        
        // 播放受伤动画
        s.anim.TriggerHurt();
        
        // 冻结输入
        s.inputGate.Freeze(s.config.hurtFreezeDuration);
        
        // 设置计时器
        hurtTimer = s.config.hurtFreezeDuration;
        
    }

    public override Transition LogicUpdate()
    {
        // 计时器递减
        hurtTimer -= Time.deltaTime;
        
        // 时间到，退出受伤状态
        if (hurtTimer <= 0)
        {
            return new Transition(PlayerStateId.Idle);
        }
        
        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.ctx.IsHurt = false;
    }
}
