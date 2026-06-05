using UnityEngine;
using RestartPlayer.HFSM;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
public class Player : MonoBehaviour
{
    public InputManager inputActions;
    // private Transform telePoint => GetComponent<Transform>().Find("ClimbPoint");

    [Header("基本组件")]
    public PlayerConfig config;
    public PhysicsCheck physicsCheck;
    public PlayerMotor2D motor;
    public PlayerAnimatorDriver anim;
    public PlayerFxSpeaker fxSpeaker;
    public Character character;

    public PlayerServices s;
    public PlayerContext ctx;
    public PlayerStateRegistry stateRegistry;
    public PlayerStateMachine stateMachine;
    public PlayerInputGate inputGate;

    private void Awake()
    {
        inputActions = new InputManager();
        inputActions.Enable();

        // ====== 服务容器初始化 ======
        character = GetComponent<Character>();
        if (ctx == null) ctx = new PlayerContext();
        stateRegistry = new PlayerStateRegistry();
        stateMachine = new PlayerStateMachine(stateRegistry);
        inputGate = new PlayerInputGate();
        s = new PlayerServices(config, stateMachine, ctx, anim, stateRegistry, motor, fxSpeaker, inputGate, character);

        // WORKFLOW：在这里注册状态
        // 普通动作状态
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

        // 攻击动作状态
        stateRegistry.Register(PlayerStateId.AttackCombo1, new PlayerAttackCombo1State(s));
        stateRegistry.Register(PlayerStateId.AttackCombo2, new PlayerAttackCombo2State(s));
        stateRegistry.Register(PlayerStateId.AttackCombo3, new PlayerAttackCombo3State(s));
        stateRegistry.Register(PlayerStateId.AirAttack, new PlayerAirAttackState(s));
        stateRegistry.Register(PlayerStateId.AirDownAttack, new PlayerAirDownAttackState(s));
        stateRegistry.Register(PlayerStateId.AirUpAttack, new PlayerAirUpAttackState(s));

        stateRegistry.Register(PlayerStateId.Hurt, new PlayerHurtState(s));
        stateRegistry.Register(PlayerStateId.Death, new PlayerDeathState(s));

        // 技能状态
        stateRegistry.Register(PlayerStateId.Heal, new PlayerHealState(s));
        stateRegistry.Register(PlayerStateId.LightCut, new PlayerLightCutState(s));
        stateRegistry.Register(PlayerStateId.LightCrown, new PlayerLightCrownState(s));

        // 休息状态
        stateRegistry.Register(PlayerStateId.Rest, new PlayerRestState(s));
    }

    private void Start()
    {
        // 初始化朝向（与旧字段FacingDirection一致的职责转移到ctx）
        ctx.SetFacingDirection(1);
        ctx.playerLayer = LayerMask.NameToLayer("Player");
        ctx.enemyLayer = LayerMask.NameToLayer("Enemy");
        stateMachine.Initialize(PlayerStateId.Idle);
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {

        inputGate.Tick(Time.deltaTime); // 更新输入冻结计时器
        if (inputGate.IsFrozen)
            inputActions.Disable(); // 冻结时禁用输入系统
        else
            inputActions.Enable(); // 非冻结时启用输入系统

        // ====== 采样输入 -> ctx ======
        var move = inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();
        if (inputActions.MoveSystem.Sprint.IsPressed())
        {
            move.x *= 2;
        }

        // ctx.MoveInput = move;
        ctx.MoveInput = inputGate.FilterMove(move); // 通过输入门过滤移动输入
        // if (ctx.IsHurt) inputActions.Disable(); // 受伤时禁用输入系统
        // else inputActions.Enable(); // 恢复输入系统

        ctx.JumpPressedThisFrame = inputActions.MoveSystem.Jump.WasPressedThisFrame();
        ctx.SprintPressedThisFrame = inputActions.MoveSystem.Sprint.WasPressedThisFrame();
        ctx.SprintIsHeld = inputActions.MoveSystem.Sprint.IsPressed();

        // 攻击输入采样
        ctx.AttackPressedThisFrame = inputActions.AttackSystem.Attack.WasPressedThisFrame();
        ctx.UpAttackPressedThisFrame = inputActions.AttackSystem.UpAttack.WasPressedThisFrame();
        ctx.DownAttackPressedThisFrame = inputActions.AttackSystem.DownAttack.WasPressedThisFrame();

        // 治疗技能输入采样
        ctx.SkillPressedThisFrame = inputActions.SkillSystem.Heal.WasPressedThisFrame();
        ctx.IsHoldingSkill = inputActions.SkillSystem.Heal.IsPressed();
        ctx.SkillPerformedThisFrame = inputActions.SkillSystem.Heal.WasPerformedThisFrame();

        // 霹雳一闪技能输入采样
        ctx.LightCutPressedThisFrame = inputActions.SkillSystem.LightCut.WasPressedThisFrame();
        ctx.IsHoldingLightCut = inputActions.SkillSystem.LightCut.IsPressed();
        inputActions.SkillSystem.LightCut.performed += _ctx => ctx.LightCutPerformedThisFrame = true;

        // 攻击力Buff技能输入采样
        ctx.LightCrownPressedThisFrame = inputActions.SkillSystem.LightCrown.WasPressedThisFrame();
        ctx.IsHoldingLightCrown = inputActions.SkillSystem.LightCrown.IsPressed();
        ctx.LightCrownPerformedThisFrame = inputActions.SkillSystem.LightCrown.WasPerformedThisFrame();

        // 激活存档点按键采样
        ctx.ActivatePressedThisFrame = inputActions.SkillSystem.ActivePoint.WasPressedThisFrame();
        ctx.IsHoldingActivate = inputActions.SkillSystem.ActivePoint.IsPressed();
        ctx.ActivatePerformedThisFrame = inputActions.SkillSystem.ActivePoint.WasPerformedThisFrame();

        // 休息按键采样
        ctx.RestPressedThisFrame = inputActions.MoveSystem.Rest.WasPressedThisFrame();

        // 复活按键采样
        ctx.ResurrectPressedThisFrame = inputActions.MoveSystem.Resurrect.WasPressedThisFrame();

        // ====== 传感器 -> ctx ======
        ctx.CurrentFocus = character.currentFocus;
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


    public void CreateSprintDust()
    {
        fxSpeaker.CreateFX(motor.transform.position, ctx.FacingDirection, ParticalEffectType.UnderDust);
    }
    public void CreateChargingFX()
    {
        fxSpeaker.CreateFX(motor.transform.position, ctx.FacingDirection, ParticalEffectType.Charging);
    }

    public IEnumerator Teleport_AfterClimb()
    {
        motor.Teleport(transform.position + new Vector3(ctx.FacingDirection * 1.372f, 2.67f, 0)); // 这里直接上移1单位，避免攀爬点设置不准引发的bug
        yield return new WaitForSeconds(0.01f); // 等待0.1秒，确保位置更新后再播放落地动画
        anim.PlayLand();
    }

    /// <summary>
    /// 统一方法，动画完成后调用
    /// </summary>
    public void Animation_Callback()
    {
        var state = stateMachine.currentState;

        if (state == null) return;

        switch (state)
        {
            case PlayerAttackState attackState:
                attackState.OnAttackAnimFinished();
                break;
            case PlayerAirAttackState airAttackState:
                airAttackState.OnAirAttackAnimFinished();
                break;
            case PlayerAirDownAttackState downAttackState:
                downAttackState.OnAirDownAttackAnimFinished();
                break;
            case PlayerAirUpAttackState upAttackState:
                upAttackState.OnAirUpAttackAnimFinished();
                break;
            case PlayerHealState healState:
                healState.OnHealAnimFinished();
                break;
            case PlayerLightCutState lightCutState:
                lightCutState.OnLightCutAnimFinished();
                break;
            case PlayerLightCrownState buffState:
                buffState.OnBuffAnimFinished();
                break;
            case PlayerSprintState:
            case PlayerAirSprintState:
                ctx.IsSprintFinished = true;
                break;
            default:
                break;
        }
    }

    public void Animation_ComboWindowOpen()
    {
        var attackState = stateMachine.currentState as PlayerAttackState;
        attackState?.OnComboWindowOpen();
    }

    public void Animation_ComboWindowClose()
    {
        var attackState = stateMachine.currentState as PlayerAttackState;
        attackState?.OnComboWindowClose();
    }

    public void PlayerHurt()
    {
        if (stateMachine.currentState is PlayerHurtState) return; // 已经在受伤状态了就别重复请求了
        ctx.IsHurt = true;
    }

    public void PlayerDeath()
    {
        if (stateMachine.currentState is PlayerDeathState) return;
        ctx.IsDead = true;
    }

    public void Animation_Move(float distance)
    {
        var attackState = stateMachine.currentState as PlayerAttackState;
        attackState?.ApplyAttackMove(distance);
    }
    /// <summary>
    /// 为霹雳一闪提供的动画事件回调：执行冲刺移动
    /// </summary>
    /// <param name="cutForce"></param>
    public void Animation_LightCutMove(float cutForce)
    {
        var lightCutState = stateMachine.currentState as PlayerLightCutState;
        lightCutState?.LightCutMove(cutForce);
    }
}