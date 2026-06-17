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

    }

    public override Transition LogicUpdate()
    {
        // 死亡动画播完后切到休息状态
        if (s.ctx.CanResurrect)
        {
            Resurrect();
            return new Transition(PlayerStateId.Rest);
        }
        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.inputGate.Freeze(0.5f);
        Debug.Log("退出死亡状态");
        s.ctx.IsDead = false;
    }

    public void Resurrect()
    {
        s.character.resurrectPoint = s.ctx.ResurrectPoint;
        s.character.Resurrect();
        s.ctx.CanResurrect = false;
        s.anim.TriggerResurrect();
        s.inputGate.Freeze(0.5f);
    }
}
