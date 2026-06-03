using RestartPlayer.HFSM;
using UnityEngine;

public abstract class PlayerState
{
    protected PlayerServices s;

    protected PlayerState(PlayerServices s)
    {
        this.s = s;
    }

    public virtual void Enter() { }

    /// <summary>
    /// 用 Transition 表达“想切到哪个状态”，不直接切。
    /// </summary>
    public virtual Transition LogicUpdate() 
    {
        if(s.ctx.IsHurt)
        {
            return new Transition(PlayerStateId.Hurt);
        }
        return Transition.None;
    }

    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }

    protected void Commit(Transition t)
    {
        if (t.HasTarget)
            s.stateMachine.RequestChangeState(t.Target);
    }
}