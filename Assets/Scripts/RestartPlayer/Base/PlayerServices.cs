using RestartPlayer.HFSM;
public sealed class PlayerServices
{
    // WORKFLOW：在这里添加模块
    // public Player player;
    public PlayerConfig config;
    public PlayerStateMachine stateMachine;
    public PlayerContext ctx;
    public PlayerAnimatorDriver anim;
    public PlayerStateRegistry stateRegistry;
    public PlayerMotor2D motor;
    public PlayerFxSpeaker fxSpeaker;
    public PlayerInputGate inputGate;

    
    public PlayerServices(
        PlayerConfig config,
        PlayerStateMachine stateMachine,
        PlayerContext ctx,
        PlayerAnimatorDriver anim,
        PlayerStateRegistry stateRegistry,
        PlayerMotor2D motor,
        PlayerFxSpeaker fxSpeaker,
        PlayerInputGate inputGate)
    {
        // this.player = player;
        this.config = config;
        this.stateMachine = stateMachine;
        this.ctx = ctx;
        this.anim = anim;
        this.stateRegistry = stateRegistry;
        this.motor = motor;
        this.fxSpeaker = fxSpeaker;
        this.inputGate = inputGate;
    }
}