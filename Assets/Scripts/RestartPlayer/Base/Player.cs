using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InputManager inputActions;
    [Header("基本组件")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("所有状态机和状态实例")]
    public PlayerStateMachine stateMachine;
    // public PlayerIdleState idleState;

    void Awake()
    {
        inputActions = new InputManager();

        stateMachine = new PlayerStateMachine();

        

    }

    void Start()
    {
        // stateMachine.Initialize()
    }

    void Update()
    {
        stateMachine.CurrentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }


}
