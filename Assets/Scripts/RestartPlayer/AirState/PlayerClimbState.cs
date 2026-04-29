using UnityEngine;
using RestartPlayer.HFSM;

public class PlayerClimbState : PlayerAirState
{
    public PlayerClimbState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("进入攀爬状态");

        // 冻结重力
        s.motor.CaptureOriginalGravity();
        s.motor.GravityScale = 0f;

        // 冻结物理输入并清空速度
        s.motor.SetVelocity(Vector2.zero);
        s.inputGate.Freeze(0.7f);

        // 关掉rb的插值
        s.motor.SwtichInterpolate(false);

        // 播放动画
        s.anim.TriggerClimb();

        // TODO：理想状态：
        // 1) 先播放攀爬动画，动画事件在关键帧调用 Teleport_AfterClimb()，确保动画完整播放且位置正确
        // 2) Teleport_AfterClimb() 内部调用 motor.Teleport()

    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 和Fall状态一样的转换
        // BUG：状态转换太快，导致动画都没有播放完
        if (s.ctx.IsGrounded)
        {
            if (Mathf.Abs(s.ctx.MoveInput.x) < 0.1f)
            {
                s.anim.TriggerIdle();
                return new Transition(PlayerStateId.Idle);
            }
            else
            {
                s.anim.TriggerIng();
                return new Transition(PlayerStateId.Locomotion);
            }
        }
        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        // 恢复重力
        s.motor.RestoreOriginalGravity();
        // 恢复rb插值
        s.motor.SwtichInterpolate(true);
    }
}
