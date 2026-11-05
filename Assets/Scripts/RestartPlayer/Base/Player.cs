using UnityEngine;
using RestartPlayer.HFSM;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class Player : MonoBehaviour
{
    public InputManager inputActions;
    // private Transform telePoint => GetComponent<Transform>().Find("ClimbPoint");

    [Header("基本组件")] public PlayerConfig config;
    public PhysicsCheck physicsCheck;
    public PlayerMotor2D motor;

    public PlayerAnimatorDriver anim;

    // public PlayerFxSpeaker fxSpeaker;
    public Character character;

    [Header("事件引用")] private EventBinding<BlockCameraShakeEvent> cameraShakeEventBinding;
    private EventBinding<NewGameEvent> newGameEventBinding;
    private EventBinding<BackToMenuEvent> backToMenuEventBinding;
    private EventBinding<DeathEvent> deathEventBinding;

    private EventBinding<HurtEvent> hurtEventBinding;

    // public VoidSo cameraShakeEvent;
    // public VoidSo NewGameEvent;
    // public VoidSo BackToMenuEvent;
    // public CharacterEventSo HealthChangeEvent;
    public PlayerServices s;
    public PlayerContext context;
    public PlayerStateRegistry stateRegistry;
    public PlayerStateMachine stateMachine;
    public PlayerInputGate inputGate;

    private void Awake()
    {
        inputActions = new InputManager();
        inputActions.Enable();

        // ====== 服务容器初始化 ======
        character = GetComponent<Character>();
        if (context == null) context = new PlayerContext();
        stateRegistry = new PlayerStateRegistry();
        stateMachine = new PlayerStateMachine(stateRegistry);
        inputGate = new PlayerInputGate();
        s = new PlayerServices(config, stateMachine, context, anim, stateRegistry, motor, inputGate, character);

        // WORKFLOW：在这里注册状态
        // 普通动作状态
        stateRegistry.Register(PlayerStateId.Idle, new PlayerIdleState(s));
        stateRegistry.Register(PlayerStateId.Locomotion, new PlayerLocomotionState(s));
        // stateRegistry.Register(PlayerStateId.Turn, new PlayerTurnState(s));
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

        // 格挡状态
        stateRegistry.Register(PlayerStateId.Block, new PlayerBlockState(s));
        stateRegistry.Register(PlayerStateId.Parry, new PlayerParryState(s));
        stateRegistry.Register(PlayerStateId.PostureBroken, new PlayerPostureBrokenState(s));

        // 休息状态
        stateRegistry.Register(PlayerStateId.Rest, new PlayerRestState(s));
    }

    void OnEnable()
    {
        newGameEventBinding = new EventBinding<NewGameEvent>(NewGame);
        backToMenuEventBinding = new EventBinding<BackToMenuEvent>(BackToMenu);
        deathEventBinding = new EventBinding<DeathEvent>(PlayerDeath);
        hurtEventBinding = new EventBinding<HurtEvent>(PlayerHurt);

        EventBus<NewGameEvent>.Register(newGameEventBinding);
        EventBus<BackToMenuEvent>.Register(backToMenuEventBinding);
        EventBus<DeathEvent>.Register(deathEventBinding);
        EventBus<HurtEvent>.Register(hurtEventBinding);
    }

    private void Start()
    {
        // 初始化朝向（与旧字段FacingDirection一致的职责转移到ctx）
        context.SetFacingDirection(1);
        context.PlayerLayer = LayerMask.NameToLayer("Player");
        context.EnemyLayer = LayerMask.NameToLayer("Enemy");
        stateMachine.Initialize(PlayerStateId.Idle);
    }

    private void OnDisable()
    {
        inputActions.Disable();
        EventBus<NewGameEvent>.Deregister(newGameEventBinding);
        EventBus<BackToMenuEvent>.Deregister(backToMenuEventBinding);
        EventBus<DeathEvent>.Deregister(deathEventBinding);
        EventBus<HurtEvent>.Deregister(hurtEventBinding);
    }

    private void Update()
    {
        inputGate.Tick(Time.deltaTime); // 更新输入冻结计时器
        if (inputGate.IsFrozen || SceneManager.GetSceneByName("Menu").isLoaded)
        {
            inputActions.MoveSystem.Disable();
            inputActions.AttackSystem.Disable();
            inputActions.SkillSystem.Disable();
            inputActions.MoveSystem.Rest.Disable();
            inputActions.UI.Disable();
        }
        else
            inputActions.Enable(); // 非冻结时启用输入系统

        // ====== 采样输入 -> ctx ======
        var move = inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();
        if (inputActions.MoveSystem.Sprint.IsPressed())
        {
            move.x *= 2;
        }

        context.MoveInput = inputGate.FilterMove(move); // 通过输入门过滤移动输入

        context.SprintIsHeld = inputActions.MoveSystem.Sprint.IsPressed();
        // 部分输入检测已经被预输入替代
        // context.JumpPressedThisFrame = inputActions.MoveSystem.Jump.WasPressedThisFrame();
        // context.SprintPressedThisFrame = inputActions.MoveSystem.Sprint.WasPressedThisFrame();
        // context.AttackPressedThisFrame = inputActions.AttackSystem.Attack.WasPressedThisFrame();
        // context.UpAttackPressedThisFrame = inputActions.AttackSystem.UpAttack.WasPressedThisFrame();
        // context.DownAttackPressedThisFrame = inputActions.AttackSystem.DownAttack.WasPressedThisFrame();
        // context.BlockPressedThisFrame = inputActions.AttackSystem.Block.WasPressedThisFrame();

        // 治疗技能输入采样
        context.SkillPressedThisFrame = inputActions.SkillSystem.Heal.WasPressedThisFrame();
        context.IsHoldingSkill = inputActions.SkillSystem.Heal.IsPressed();
        context.SkillPerformedThisFrame = inputActions.SkillSystem.Heal.WasPerformedThisFrame();

        // 霹雳一闪技能输入采样
        context.LightCutPressedThisFrame = inputActions.SkillSystem.LightCut.WasPressedThisFrame();
        context.IsHoldingLightCut = inputActions.SkillSystem.LightCut.IsPressed();
        if (inputActions.SkillSystem.LightCut.WasPerformedThisFrame()) context.LightCutPerformedThisFrame = true;
        
        // 危险的操作，会导致内存泄露
        // inputActions.SkillSystem.LightCut.performed += _ctx => context.LightCutPerformedThisFrame = true;
        // context.LightCutPerformedThisFrame = inputActions.SkillSystem.LightCut.WasPerformedThisFrame();

        // 攻击力Buff技能输入采样
        context.LightCrownPressedThisFrame = inputActions.SkillSystem.LightCrown.WasPressedThisFrame();
        context.IsHoldingLightCrown = inputActions.SkillSystem.LightCrown.IsPressed();
        context.LightCrownPerformedThisFrame = inputActions.SkillSystem.LightCrown.WasPerformedThisFrame();

        // 格挡输入采样
        context.BlockIsHeld = inputActions.AttackSystem.Block.IsPressed();

        // 激活存档点按键采样
        context.ActivatePressedThisFrame = inputActions.SkillSystem.ActivePoint.WasPressedThisFrame();
        context.IsHoldingActivate = inputActions.SkillSystem.ActivePoint.IsPressed();
        context.ActivatePerformedThisFrame = inputActions.SkillSystem.ActivePoint.WasPerformedThisFrame();

        // 休息按键采样
        context.RestPressedThisFrame = inputActions.MoveSystem.Rest.WasPressedThisFrame();

        // ====== 预输入缓冲计时器 =======

        context.TickInputBuffers(Time.deltaTime);

        if (inputActions.MoveSystem.Jump.WasPressedThisFrame())
            context.JumpBufferTimer = config.jumpBufferTime;
        if (inputActions.MoveSystem.Sprint.WasPressedThisFrame())
            context.SprintBufferTimer = config.sprintBufferTime;
        if (inputActions.AttackSystem.Attack.WasPressedThisFrame())
            context.AttackBufferTimer = config.attackBufferTime;
        if (inputActions.AttackSystem.UpAttack.WasPressedThisFrame())
            context.UpAttackBufferTimer = config.attackBufferTime;
        if (inputActions.AttackSystem.DownAttack.WasPressedThisFrame())
            context.DownAttackBufferTimer = config.attackBufferTime;
        if (inputActions.AttackSystem.Block.WasPressedThisFrame())
            context.BlockBufferTimer = config.blockBufferTime;


        // ====== 传感器 -> ctx ======
        context.CurrentFocus = character.currentFocus;
        context.IsGrounded = physicsCheck.IsGround;
        context.IsTouchingLeftWall = physicsCheck.touchLeftWall;
        context.IsTouchingRightWall = physicsCheck.touchRightWall;
        context.IsTouchingTopLeftWall = physicsCheck.touchLeftTopWall;
        context.IsTouchingTopRightWall = physicsCheck.touchRightTopWall;

        // 架势系统同步
        if (character.postureSystem != null)
        {
            context.CurrentPosture = character.postureSystem.currentPosture;
            context.MaxPosture = character.postureSystem.maxPosture;
            context.IsPostureBroken = character.postureSystem.isBroken;
        }

        // ====== 动画参数统一在 Driver 写入 ======
        // if (context.IsAttacking || context.IsBlocking)
        //     anim.SetInputX(0);
        // else
        anim.SetInputX(Mathf.Abs(context.MoveInput.x));
        anim.SetIsGround(context.IsGrounded);
        anim.SetIsWall(context.IsTouchingWall);
        anim.SetVelocityY(Mathf.Clamp(motor.Velocity.y, -1f, 1f));

        // ====== 状态机 Tick（统一提交切换）======
        stateMachine.Tick();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedTick();
    }

    private void NewGame()
    {
        // 新游戏注册方法，用于重置玩家的各项数值
        context.Reset();
        inputActions.Enable();
        stateMachine.currentState.Exit();

        // FIXME:当前事件调用需要测试
        EventBus<HealthChangeEvent>.Raise(new HealthChangeEvent { character = character });
        stateMachine.Initialize(PlayerStateId.Idle);
    }

    /// <summary>
    /// 返回主菜单时，务必确保玩家面朝向右
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void BackToMenu()
    {
        if (gameObject.transform.rotation.y == -1)
        {
            context.SetFacingDirection(1);
            motor.FlipFacing();
        }
    }

    /// <summary>
    /// 复活按钮事件
    /// </summary>
    public void Resurrect()
    {
        context.CanResurrect = true;
    }


    public void CreateSprintDust()
    {
        // fxSpeaker.CreateFX(motor.transform.position, ctx.FacingDirection, ParticalEffectType.UnderDust);
        EventBus<FxEvent>.Raise(new FxEvent
        {
            playerTran = motor.transform.position,
            dir = context.FacingDirection,
            type = ParticalEffectType.UnderDust
        });
    }

    public void CreateChargingFX()
    {
        // fxSpeaker.CreateFX(motor.transform.position, ctx.FacingDirection, ParticalEffectType.Charging);
        EventBus<FxEvent>.Raise(new FxEvent
        {
            playerTran = motor.transform.position,
            dir = context.FacingDirection,
            type = ParticalEffectType.Charging
        });
    }

    public IEnumerator Teleport_AfterClimb()
    {
        motor.Teleport(transform.position +
                       new Vector3(context.FacingDirection * 1.372f, 2.67f, 0)); // 这里直接上移1单位，避免攀爬点设置不准引发的bug
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
            case PlayerBlockState blockState:
                blockState.OnBlockAnimFinished();
                break;
            case PlayerSprintState:
            case PlayerAirSprintState:
                context.IsSprintFinished = true;
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

    public void PlayerHurt(HurtEvent e)
    {
        if (e.character != this.character) return;
        if (stateMachine.currentState is PlayerHurtState) return; // 已经在受伤状态了就别重复请求了
        context.IsHurt = true;
    }

    public void PlayerDeath(DeathEvent e)
    {
        if (e.character != this.character) return;
        if (stateMachine.currentState is PlayerDeathState) return;
        EventBus<GameOverEvent>.Raise(new GameOverEvent { });
        context.IsDead = true;
    }

    public void Animation_ParryWindowOff()
    {
        var blockState = stateMachine.currentState as PlayerBlockState;
        blockState?.SetParryWindow(false);
    }

    public void Animation_ParryWindowOn()
    {
        var blockState = stateMachine.currentState as PlayerBlockState;
        blockState?.SetParryWindow(true);
    }

    public void Animation_Move(float distance)
    {
        var attackState = stateMachine.currentState as PlayerAttackState;
        attackState?.ApplyAttackMove(distance);
    }

    /// <summary>
    /// 为流光闪击提供的动画事件回调：执行冲刺移动
    /// </summary>
    /// <param name="cutForce"></param>
    public void Animation_LightCutMove(float cutForce)
    {
        var lightCutState = stateMachine.currentState as PlayerLightCutState;
        lightCutState?.LightCutMove(cutForce);
    }
}