using UnityEngine;
using RestartPlayer.HFSM;
public class Player : MonoBehaviour
{
    public InputManager inputActions;
    public Vector2 moveinput;

    [Header("基本组件")]
    public PlayerConfig config;
    public PhysicsCheck physicsCheck;
    public PlayerMotor2D motor;
    public PlayerAnimatorDriver anim;
    public PlayerFxSpeaker fxSpeaker;

    public PlayerServices s;
    public PlayerContext ctx;
    public PlayerStateRegistry stateRegistry;
    public PlayerStateMachine stateMachine;
    public PlayerInputGate inputGate;

    private void Awake()
    {
        inputActions = new InputManager();
        inputActions.Enable();

        if (ctx == null) ctx = new PlayerContext();
        stateRegistry = new PlayerStateRegistry();
        stateMachine = new PlayerStateMachine(stateRegistry);
        inputGate = new PlayerInputGate();
        s = new PlayerServices(config, stateMachine, ctx, anim, stateRegistry, motor, fxSpeaker, inputGate);
        // WORKFLOW：在这里注册状态
        stateRegistry.Register(PlayerStateId.Idle, new PlayerIdleState(s));
        stateRegistry.Register(PlayerStateId.Locomotion, new PlayerLocomotionState(s));
        stateRegistry.Register(PlayerStateId.Turn, new PlayerTurnState(s));
        stateRegistry.Register(PlayerStateId.Sprint, new PlayerSprintState(s));
        stateRegistry.Register(PlayerStateId.Jump, new PlayerJumpState(s));
        stateRegistry.Register(PlayerStateId.Fall, new PlayerFallState(s));
        stateRegistry.Register(PlayerStateId.AirSprint, new PlayerAirSprintState(s));
        stateRegistry.Register(PlayerStateId.Jump2, new PlayerJump2State(s));
        stateRegistry.Register(PlayerStateId.WallSlide, new PlayerWallSlideState(s));
        stateRegistry.Register(PlayerStateId.WallJump, new PlayerWallJumpState(s));
        stateRegistry.Register(PlayerStateId.Climb, new PlayerClimbState(s));
    }

    private void Start()
    {
        // 初始化朝向（与旧字段FacingDirection一致的职责转移到ctx）
        ctx.SetFacingDirection(1);
        stateMachine.Initialize(PlayerStateId.Idle);
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {

        inputGate.Tick(Time.deltaTime); // 更新输入冻结计时器

        // ====== 采样输入 -> ctx ======
        var move = inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();
        if (inputActions.MoveSystem.Sprint.IsPressed())
        {
            move.x *= 2;
        }

        // ctx.MoveInput = move;
        ctx.MoveInput = inputGate.FilterMove(move); // 通过输入门过滤移动输入
        moveinput = ctx.MoveInput;
        // TODO：需要进一步测试与调参数
        ctx.JumpPressedThisFrame = inputActions.MoveSystem.Jump.WasPressedThisFrame();
        ctx.SprintPressedThisFrame = inputActions.MoveSystem.Sprint.WasPressedThisFrame();
        ctx.SprintIsHeld = inputActions.MoveSystem.Sprint.IsPressed();


        // ====== 传感器 -> ctx ======
        ctx.IsGrounded = physicsCheck.IsGround;
        ctx.IsTouchingLeftWall = physicsCheck.touchLeftWall;
        ctx.IsTouchingRightWall = physicsCheck.touchRightWall;
        ctx.IsTouchingTopLeftWall = physicsCheck.touchLeftTopWall;
        ctx.IsTouchingTopRightWall = physicsCheck.touchRightTopWall;

        // ====== 动画参数统一在 Driver 写入 ======
        anim.SetInputX(Mathf.Abs(ctx.MoveInput.x));
        anim.SetIsGround(ctx.IsGrounded);
        anim.SetIsWall(ctx.IsTouchingWall);
        anim.SetVelocityY(Mathf.Clamp(motor.Velocity.y, -1f, 1f));

        // ====== 状态机 Tick（统一提交切换）======
        stateMachine.Tick();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedTick();
    }

    // 动画事件：转身结束
    public void Animation_TurnFinished()
    {
        ctx.FlipFacing();
        transform.Rotate(0, 180f, 0);

        if (Mathf.Abs(ctx.MoveInput.x) < 0.01f)
            stateMachine.RequestChangeState(PlayerStateId.Idle);
        else
            stateMachine.RequestChangeState(PlayerStateId.Locomotion);
    }

    public void SprintFinished()
    {
        ctx.IsSprintFinished = true;
    }

    public void CreateSprintDust()
    {
        fxSpeaker.CreateFX(motor.transform, ctx.FacingDirection, ParticalEffectType.UnderDust);
    }
}