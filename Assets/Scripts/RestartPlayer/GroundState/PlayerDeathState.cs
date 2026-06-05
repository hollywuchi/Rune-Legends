using UnityEngine;
using RestartPlayer.HFSM;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.anim.TriggerDeath();

        s.character.currentFocus = 0;
        s.ctx.CurrentFocus = 0;

        s.character.attackBuffStack = 0;

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
     }

     public void Resurrect()
     {
         s.character.Resurrect();
     }
}
