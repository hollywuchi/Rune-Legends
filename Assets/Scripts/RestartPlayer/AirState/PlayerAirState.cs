using RestartPlayer.HFSM;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerServices s) : base(s) { }

    public override Transition LogicUpdate()
    {
        // 空中冲刺（域内规则）
        if (s.ctx.SprintPressedThisFrame && s.ctx.CanSprint)
            return new Transition(PlayerStateId.AirSprint);

        // 空中转向（安全：只做朝向，不切状态）
        if (s.ctx.MoveInput.x != 0 && Mathf.Sign(s.ctx.MoveInput.x) != s.ctx.FacingDirection)
        {
            s.ctx.FlipFacing();
            s.motor.FlipFacing();
        }

        // BUG：在这个状态下，会有很严重的吞按键问题（因为这个状态的持续时间很短，玩家很难在它存在的这一帧按键）
        // 二段跳（注意：土狼时间在 FallState 里处理优先级，这一步先保持你逻辑）
        if (s.ctx.JumpPressedThisFrame && s.ctx.JumpCount < s.ctx.MaxJumpCount)
            return new Transition(PlayerStateId.Jump);

        return base.LogicUpdate();
    }
}