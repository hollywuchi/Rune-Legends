using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InputManager inputActions;
    [Header("参数调整")]
    public float Speed;
    public float SprintSpeed;

    [Header("基本组件")]
    public Rigidbody2D rb;
    public Animator animator;
    public PoolManager poolManager;

    [Header("所有状态机和状态实例")]
    // WORKFLOW:创建一个状态实例
    public PlayerStateMachine stateMachine;
    public PlayerIdleState idleState;
    public PlayerLocomotionState locomotionState;
    public PlayerTurnState turnState;
    public PlayerSprintState sprintState;
    [Header("状态参数")]
    // 角色朝向：1代表右边，-1代表左边
    public int FacingDirection = 1;

    public Vector2 moveInput;
    void Awake()
    {
        inputActions = new InputManager();

        idleState = new PlayerIdleState(this, stateMachine);
        turnState = new PlayerTurnState(this, stateMachine);
        sprintState = new PlayerSprintState(this, stateMachine);
        locomotionState = new PlayerLocomotionState(this, stateMachine);

        // 别忘了打开新的控制系统
        inputActions.Enable();
    }

    void Start()
    {
        stateMachine.Initialize(idleState);
    }

    void OnDisable()
    {
        inputActions.Disable();
    }


    void Update()
    {
        // 先确保获得最新的输入，再来交给状态机逻辑更新
        moveInput = inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();
        if (inputActions.MoveSystem.Sprint.IsPressed())
        {
            // 修复BUG：修复玩家按住冲刺按键之后的输入堆积问题
            moveInput.x *= 2;
        }
        animator.SetFloat("InputX", Mathf.Abs(moveInput.x));
        stateMachine.CurrentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }


    public void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0, 180f, 0);
    }

    /// <summary>
    /// 动画事件：转身动画结束时调用,解决转身动画结束之后，仍然停留在转身状态的bug
    /// </summary>
    public void Animation_TurnFinished()
    {
        Flip();

        if (moveInput.x == 0)
        {
            stateMachine.ChangeState(idleState);
        }
        else
        {
            stateMachine.ChangeState(locomotionState);
        }
    }

    public void SprintFinished()
    {
        sprintState.isSprintFinished = true;
    }

    public void CreateSprintDust()
    {
        poolManager.CreateSprintDust(transform, FacingDirection, ParticalEffectType.UnderDust);
    }
}
