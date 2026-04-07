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
    public float walkBufferTimer;

    [Header("基本组件")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("所有状态机和状态实例")]
    // WORKFLOW:创建一个状态实例
    public PlayerStateMachine stateMachine;
    public PlayerIdleState idleState;
    public PlayerRunState runState;
    public PlayerTurnState turnState;
    public PlayerWalkState walkState;

    [Header("状态参数")]
    // 角色朝向：1代表右边，-1代表左边
    public int FacingDirection = 1;

    public Vector2 moveInput;
    void Awake()
    {
        inputActions = new InputManager();

        // stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        turnState = new PlayerTurnState(this, stateMachine);
        walkState = new PlayerWalkState(this, stateMachine);

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
        stateMachine.CurrentState.LogicUpdate();
        moveInput = inputActions.MoveSystem.WalkOrRun.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }


    public bool WalkBuffer()
    {
        walkBufferTimer += Time.deltaTime;
        if(walkBufferTimer >= 0.05f)
            return true;
        else
            return false;
    }
    public void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0, 180f, 0);
    }
    
    public void Animation_TurnFinished()
    {
        stateMachine.CurrentState.OnAnimationFinished();
    }

    /// <summary>
    /// 判断玩家行走或者跑步状态
    /// </summary>
    public void ChackMoveState()
    {
        // 根据玩家输入判断是否在行走，轻推摇杆为走路，满摇杆为跑步
        if (Mathf.Abs(moveInput.x) < 0.4f)
        {
            if (Mathf.Abs(moveInput.x) < 0.01f && Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                stateMachine.ChangeState(idleState);
                // 当玩家不动的时候，return回去，防止其他操作
                return;
            }
            else
                // 否则就切换到行走状态
                stateMachine.ChangeState(walkState);
        }
    }

}
