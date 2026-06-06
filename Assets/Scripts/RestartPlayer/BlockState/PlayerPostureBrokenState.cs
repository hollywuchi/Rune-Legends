using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 架势破碎（破防）状态：架势条满后进入，播放破防动画，硬直一段时间
/// </summary>
public class PlayerPostureBrokenState : PlayerState
{
    private float brokenTimer;

    public PlayerPostureBrokenState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.IsPostureBroken = true;

        // 播放破防动画
        s.anim.TriggerPostureBroken();

        // 停止移动
        s.motor.SetVelocityX(0f);

        // 冻结输入
        s.inputGate.Freeze(s.config.postureBrokenDuration);

        brokenTimer = 0f;
    }

    public override Transition LogicUpdate()
    {
        brokenTimer += Time.deltaTime;

        if (brokenTimer >= s.config.postureBrokenDuration)
        {
            // 破防结束，重置架势
            s.ctx.IsPostureBroken = false;
            s.ctx.CurrentPosture = 0f;
            return new Transition(PlayerStateId.Idle);
        }

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.ctx.IsPostureBroken = false;
    }
}
