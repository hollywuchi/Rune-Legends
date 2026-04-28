using UnityEngine;
using RestartPlayer.HFSM;

public class PlayerClimbState : PlayerAirState
{
    public PlayerClimbState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        // TODO: 冻结重力，物理输入并清空速度，并播放动画
        Debug.Log("进入攀爬状态");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;
        // TODO： 计算玩家的位置距离，让玩家传送到这个位置
        return Transition.None;
    }
}
