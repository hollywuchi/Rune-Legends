using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Red red;
    Vector2 inputValue;
    PhysicsCheck physicsCheck;
    [HideInInspector] public Rigidbody2D rb;
    PlayerAnimation playerAnimation;
    [HideInInspector] public CapsuleCollider2D Coll;
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidSo SceneAfter;
    public VoidSo LoadDataEvent;
    public VoidSo BackToMenuEvent;
    [Header("参数")]
    public float Speed;
    public float AttackForce;
    public float DashDistance;
    // 在挥剑过程中进行的位移补偿
    public float Attack_Move;
    [Header("Dash参数")]
    public float dashTime;  //dash冲刺时长
    float dashTimeLeft;  //冲刺剩余时间
    float lastDash = -10;//上一次dash时间点
    public float dashCoolDown;
    public float dashSpeed;

    [Header("跳跃部分")]
    public float JumpForce;

    [Header("状态")]
    public bool Ishurt;
    public bool IsDeath;
    public bool IsAttack;
    public bool IsDash;
    private void Awake()
    {
        red = new Red();
        //跳跃的事件部分
        red.GameSystem.Jump.started += Jump;
        // 攻击事件
        red.GameSystem.Attack.started += Attack_Evnent;
        // 冲刺事件
        red.GameSystem.Dash.started += Dash;
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();
        Coll = GetComponent<CapsuleCollider2D>();

        red.Enable();

    }




    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += DisMove;
        SceneAfter.OnEventRaised += CacMove;
        LoadDataEvent.OnEventRaised += loadData;
        BackToMenuEvent.OnEventRaised += loadData;
    }

    private void OnDisable()
    {
        red.Disable();
        loadEventSO.LoadRequestEvent -= DisMove;
        SceneAfter.OnEventRaised -= CacMove;
        LoadDataEvent.OnEventRaised -= loadData;
        BackToMenuEvent.OnEventRaised -= loadData;
    }



    void Update()
    {
        // 新的人物控制器，在勾选“可以变为一个类”之后，其会变为一个脚本，我们可以构造函数来实现应用
        inputValue = red.GameSystem.Move.ReadValue<Vector2>();
        CheckMaterial();
        // print(IsDeath);
    }

    void FixedUpdate()
    {
        if (!Ishurt && !IsAttack && !IsDash)
            Move();
        if (!IsAttack && !Ishurt)
            _Dash();
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputValue.x * Speed * Time.deltaTime, rb.velocity.y);

        int direction = (int)transform.localScale.x;

        if (inputValue.x > 0)
            direction = 1;
        if (inputValue.x < 0)
            direction = -1;

        transform.localScale = new Vector3(direction, 1, 1);
    }

    #region 跳跃事件
    private void Jump(InputAction.CallbackContext context)
    {
        // throw new NotImplementedException();
        // 这里使用刚体添加力，让玩家可以跳跃，
        // 力的模式改为瞬时力：forceMode2D.Impulse
        if (physicsCheck.IsGround && !IsDash)
            rb.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
    }
    #endregion

    #region 攻击事件
    private void Attack_Evnent(InputAction.CallbackContext context)
    {
        if (!IsDash)
        {
            playerAnimation.Attack();
            IsAttack = true;
            ShackXBOX(1, 0.75f, 0.75f);
            // Gamepad.current.SetMotorSpeeds(1f,1f);
        }
    }
      

    private void ShackXBOX(float time, float low, float high) => StartCoroutine(ShackingXBOX(time, low, high));
    private IEnumerator ShackingXBOX(float time, float low, float high)
    {

        if (Gamepad.current == null)
        {
            // 
            // print("you has not connect");
            yield break;
        }

        Gamepad.current.ResetHaptics();

        var EndTime = Time.time + time;

        while (Time.time < EndTime)
        {
            // TODO：继续完善手柄震动 但暂时关闭
            // Gamepad.current.SetMotorSpeeds(low, high);
            print("it's shaking");
            yield return null;
        }
        // Gamepad.current.ResumeHaptics();
        if (Gamepad.current == null)
            yield break;

        Gamepad.current.PauseHaptics();
        print("shaking has stop");
    }
    #endregion

    #region 冲刺事件
    private void Dash(InputAction.CallbackContext context)
    {
        if (Time.time >= (lastDash + dashCoolDown))
        {
            IsDash = true;

            dashTimeLeft = dashTime;

            lastDash = Time.time;

            Coll.contactCaptureLayers = LayerMask.GetMask("Nothing");
        }

    }
    void _Dash()
    {
        if (IsDash)
        {
            if (dashTimeLeft > 0)
            {
                if (!physicsCheck.IsGround)
                    physicsCheck.IsGround = true;
                rb.velocity = new Vector2(dashSpeed * transform.localScale.x, 0);
                rb.gravityScale = 0;
                // 进入冷却
                dashTimeLeft -= Time.fixedDeltaTime;

                ShadowPool.instance.GetFromPool();

            }
            else
            {
                Coll.contactCaptureLayers = LayerMask.GetMask("Enemy", "Default");
                IsDash = false;
            }
        }
        else
        {
            rb.gravityScale = 4;
        }
    }
    #endregion

    #region 物理材质事件
    void CheckMaterial()
    {
        Coll.sharedMaterial = IsAttack ? physicsCheck.Walk : physicsCheck.Wall;
    }
    #endregion

    #region UnityEnvent
    /// <summary>
    /// 玩家受伤反弹
    /// </summary>
    /// <param name="attack"></param>
    public void Gethurt(Transform attack)
    {
        rb.velocity = Vector2.zero;
        Ishurt = true;
        Vector2 dir = new Vector2(transform.position.x - attack.position.x, 0).normalized;
        // 在2D游戏中添加力是要用2D的力模式
        rb.AddForce(dir * AttackForce, ForceMode2D.Impulse);
    }
    /// <summary>
    /// 玩家死亡
    /// </summary>
    public void OnDeath()
    {
        IsDeath = true;
        red.GameSystem.Disable();
    }

    /// <summary>
    /// 加载场景开始不让玩家乱跑
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void DisMove(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        red.Disable();
    }

    /// <summary>
    /// 加载结束再乱跑
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void CacMove()
    {
        red.Enable();
    }

    private void loadData()
    {
        IsDeath = false;
    }
    #endregion
}
