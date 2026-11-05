using RestartPlayer.HFSM;
public sealed class PlayerServices
{
    // WORKFLOW：在这里添加模块
    
    public PlayerConfig config;
    public PlayerStateMachine stateMachine;
    public PlayerContext context;
    public PlayerAnimatorDriver anim;
    public PlayerStateRegistry stateRegistry;
    public PlayerMotor2D motor;
    // public PlayerFxSpeaker fxSpeaker;
    public PlayerInputGate inputGate;
    public Character character;
    
    
    public PlayerServices(
        PlayerConfig config,
        PlayerStateMachine stateMachine,
        PlayerContext context,
        PlayerAnimatorDriver anim,
        PlayerStateRegistry stateRegistry,
        PlayerMotor2D motor,
        PlayerInputGate inputGate,
        Character character)
    {
        
        this.config = config;
        this.stateMachine = stateMachine;
        this.context = context;
        this.anim = anim;
        this.stateRegistry = stateRegistry;
        this.motor = motor;
        this.inputGate = inputGate;
        this.character = character;
    }
}