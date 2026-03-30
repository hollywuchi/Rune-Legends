using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InputManager inputActions;
    [Header("参数调整")]
    public float Speed;
    
    [Header("基本组件")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("所有状态机和状态实例")]
    public PlayerStateMachine stateMachine;
    public PlayerIdleState idleState;
    public PlayerRunState runState;

    public Vector2 moveInput;
    void Awake()
    {
        inputActions = new InputManager();

        // stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this,stateMachine);
        runState = new PlayerRunState(this,stateMachine);
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


}
