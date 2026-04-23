using RestartPlayer.HFSM;

public abstract class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerContext ctx;
    protected PlayerAnimatorDriver anim;
    protected PlayerStateRegistry stateRegistry;
    protected PlayerMotor2D motor;

    protected PlayerState(Player player, PlayerStateMachine stateMachine, PlayerContext ctx,
    PlayerAnimatorDriver anim, PlayerStateRegistry stateRegistry, PlayerMotor2D motor)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.ctx = ctx;
        this.anim = anim;
        this.stateRegistry = stateRegistry;
        this.motor = motor;
    }

    public virtual void Enter() { }

    /// <summary>
    /// 用 Transition 表达“想切到哪个状态”，不直接切。
    /// </summary>
    public virtual Transition LogicUpdate() => Transition.None;

    public virtual void PhysicsUpdate() { }
    public virtual void Exit() { }

    protected void Commit(Transition t)
    {
        if (t.HasTarget)
            stateMachine.RequestChangeState(t.Target);
    }
}