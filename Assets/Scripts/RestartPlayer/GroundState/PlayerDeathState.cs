using UnityEngine;
using RestartPlayer.HFSM;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.anim.PlayDeath();

        s.character.currentFocus = 0;
        s.ctx.CurrentFocus = 0;
        s.inputGate.Freeze(99999);
        s.character.attackBuffStack = 0;

        Debug.Log("进入死亡状态");
    }

    public override Transition LogicUpdate()
    {
        // 死亡动画播完后切到休息状态
        if (s.ctx.ResurrectPressedThisFrame)
        {
            Resurrect();
            return new Transition(PlayerStateId.Rest);
        }
        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.ctx.IsDead = false;
        Debug.Log("退出死亡状态");
    }

    public void Resurrect()
    {
        s.character.resurrectPoint = s.ctx.ResurrectPoint; // 更新角色的复活点位置,防止玩家没休息就直接打boss了
        s.character.Resurrect();
        s.anim.TriggerResurrect();
        s.inputGate.Freeze(0.5f);
    }
}
