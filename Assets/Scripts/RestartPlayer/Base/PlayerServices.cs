using RestartPlayer.HFSM;
public sealed class PlayerServices
{
    // WORKFLOW：在这里添加模块
    // public Player player;
    public PlayerConfig config;
    public PlayerStateMachine stateMachine;
    public PlayerContext context;
    public PlayerAnimatorDriver anim;
    public PlayerStateRegistry stateRegistry;
    public PlayerMotor2D motor;
    // public PlayerFxSpeaker fxSpeaker;
    public PlayerInputGate inputGate;
    public Character character;
    // REVIEW：格挡系统 - 屏幕震动事件引用（用于弹反反馈）
    // public VoidSo cameraShakeEvent;

    
    public PlayerServices(
        PlayerConfig config,
        PlayerStateMachine stateMachine,
        PlayerContext context,
        PlayerAnimatorDriver anim,
        PlayerStateRegistry stateRegistry,
        PlayerMotor2D motor,
        // PlayerFxSpeaker fxSpeaker,
        PlayerInputGate inputGate,
        Character character)
    {
        // this.player = player;
        this.config = config;
        this.stateMachine = stateMachine;
        this.context = context;
        this.anim = anim;
        this.stateRegistry = stateRegistry;
        this.motor = motor;
        // this.fxSpeaker = fxSpeaker;
        this.inputGate = inputGate;
        this.character = character;
    }
}