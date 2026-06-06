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
    public Character character;
    // REVIEW：格挡系统 - 屏幕震动事件引用（用于弹反反馈）
    public VoidSo cameraShakeEvent;

    
    public PlayerServices(
        PlayerConfig config,
        PlayerStateMachine stateMachine,
        PlayerContext ctx,
        PlayerAnimatorDriver anim,
        PlayerStateRegistry stateRegistry,
        PlayerMotor2D motor,
        PlayerFxSpeaker fxSpeaker,
        PlayerInputGate inputGate,
        Character character,
        VoidSo cameraShakeEvent = null)
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
        this.character = character;
        this.cameraShakeEvent = cameraShakeEvent;
    }
}