using UnityEngine;
using RestartPlayer.HFSM;
public class Player : MonoBehaviour
{
    public InputManager inputActions;

    [Header("参数调整")]
    public float Speed;
    public float SprintSpeed;
    public float jumpForce;
    public float SprintJumpSpeed;

    [Header("基本组件")]
    public PoolManager poolManager;
    public PhysicsCheck physicsCheck;
    public PlayerMotor2D motor;
    public PlayerAnimatorDriver anim;

    public PlayerContext ctx;
    public PlayerStateRegistry stateRegistry;
    public PlayerStateMachine stateMachine;

    private void Awake()
    {
        inputActions = new InputManager();
        inputActions.Enable();

        if (ctx == null) ctx = new PlayerContext();
        stateRegistry = new PlayerStateRegistry();
        stateMachine = new PlayerStateMachine(stateRegistry);

        stateRegistry.Register(PlayerStateId.Idle, new PlayerIdleState(this, stateMachine, ctx, anim, stateRegistry, motor));
        stateRegistry.Register(PlayerStateId.Locomotion, new PlayerLocomotionState(this, stateMachine, ctx, anim, stateRegistry, motor));
        stateRegistry.Register(PlayerStateId.Turn, new PlayerTurnState(this, stateMachine, ctx, anim, stateRegistry, motor));
        stateRegistry.Register(PlayerStateId.Sprint, new PlayerSprintState(this, stateMachine, ctx, anim, stateRegistry, motor));
        stateRegistry.Register(PlayerStateId.Jump, new PlayerJumpState(this, stateMachine, ctx, anim, stateRegistry, motor));
        stateRegistry.Register(PlayerStateId.Fall, new PlayerFallState(this, stateMachine, ctx, anim, stateRegistry, motor));
        stateRegistry.Register(PlayerStateId.AirSprint, new PlayerAirSprintState(this, stateMachine, ctx, anim, stateRegistry, motor));
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
        // ====== 采样输入 -> ctx ======
        var move = inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();
        if (inputActions.MoveSystem.Sprint.IsPressed())
        {
            // 保留你原来的“按住冲刺导致输入堆积”的处理（后续建议改成独立 sprint 修饰）
            move.x *= 2;
        }

        ctx.MoveInput = move;
        ctx.JumpPressedThisFrame = inputActions.MoveSystem.Jump.WasPressedThisFrame();
        ctx.SprintPressedThisFrame = inputActions.MoveSystem.Sprint.WasPressedThisFrame();
        ctx.SprintIsHeld = inputActions.MoveSystem.Sprint.IsPressed();

        // ====== 传感器 -> ctx ======
        ctx.IsGrounded = physicsCheck.IsGround;

        // ====== 动画参数统一在 Driver 写入 ======
        anim.SetInputX(Mathf.Abs(ctx.MoveInput.x));
        anim.SetIsGround(ctx.IsGrounded);
        anim.SetVelocityY(Mathf.Clamp(motor.Velocity.y, -1f, 1f));

        // ====== 状态机 Tick（统一提交切换）======
        stateMachine.Tick();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedTick();
    }

    public void Flip()
    {
        ctx.FlipFacing();
        transform.Rotate(0, 180f, 0);
    }

    // 动画事件：转身结束
    public void Animation_TurnFinished()
    {
        Flip();

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
        poolManager.CreateFX(transform, ctx.FacingDirection, ParticalEffectType.UnderDust);
    }
}