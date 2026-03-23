using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Permissions;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    [HideInInspector]public PhysicsCheck physicsCheck;
    [HideInInspector]public Animator anim;

    [Header("基本参数")]
    public float NormalSpeed;
    // 追逐速度
    public float chaseSpeed;
    public float CurrentSpeed;
    // 敌人的面朝方向
    public Vector3 dir;
    public Transform attacker;
    public float Backforce;
    [Header("检测玩家")]
    public Vector2 CheckOffset;
    public Vector2 CheckSize;
    public float CheckDistance;
    public LayerMask AttackLayer;

    [Header("计时器")]
    public float WaitTime;
    public float WaitTimeCounter;
    public float LostTime;
    public float LostTimeCounter;   
    [Header("状态")]
    public bool IsWait;
    public bool Ishurt;
    public bool IsDeath;

    // 现在的状态
    BaseState currnetState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();    
        physicsCheck = GetComponent<PhysicsCheck>();
        CurrentSpeed = NormalSpeed;
        WaitTimeCounter = WaitTime;
    }   
    private void OnEnable() 
    {
        // 现在的状态转换为巡逻状态
        currnetState = patrolState;
        currnetState.OnEnter(this);    
    }
    private void Update() 
    {
        dir = new Vector3(-transform.localScale.x,0,0);
        
        currnetState.LogicUpdate();
        TimeCount();
    }
    private void FixedUpdate() 
    {
        if(!Ishurt && !IsDeath && !IsWait)
            Move();
        currnetState.PhysicsUpdate();
    }
    private void OnDisable() 
    {
        currnetState.OnExit();    
    }

    /// <summary>
    /// 在这个案例中，敌人的运动状态都是直接更改速度
    /// </summary>
    public virtual void Move()
    {
        rb.velocity = new Vector3(CurrentSpeed * dir.x * Time.deltaTime,rb.velocity.y,0);
    }

    /// <summary>
    /// 行走撞墙等待时间计数器
    /// </summary>
    void TimeCount()
    {
        if(IsWait)
        {
            WaitTimeCounter -= Time.deltaTime;
            if(WaitTimeCounter <= 0)
            {
                IsWait = false;
                WaitTimeCounter = WaitTime;
                transform.localScale = new Vector3(dir.x,1,1);
                physicsCheck.GroundOffset.x *= transform.localScale.x;
            }
        }
        if(!FindPlayer())
        {
            LostTimeCounter -=Time.deltaTime;
            if(LostTimeCounter <= 0)
                LostTimeCounter = 0;
        }
        else
        {
            LostTimeCounter = LostTime;

        }
    }
    /// <summary>
    /// 背刺回头方法&受伤击退
    /// </summary>
    /// <param name="Attacker"></param>
    public void OnTakeDamage(Transform Attacker)
    {
        attacker = Attacker;
        if(attacker.transform.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1,1,1);
        if(attacker.transform.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1,1,1);

        Ishurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attacker.transform.position.x,0).normalized;
        rb.velocity = new Vector2(0,rb.velocity.y);
        StartCoroutine(WaitAttack(dir));

    }

    IEnumerator WaitAttack(Vector2 dir)
    {
        rb.AddForce(dir * Backforce,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        Ishurt = false;
    }

    public void OnDie()
    {
        IsDeath = true;
        
        rb.velocity = new Vector2(0,rb.velocity.y);
        anim.SetBool("Death",IsDeath);
    }

    public void DestoryAnimation()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 发现玩家,然后返回bool值
    /// </summary>
    /// <returns></returns>
    public bool FindPlayer()
    {
        // 一个新的盒装检测器：参数有很多（原点，盒子的大小，盒子角度，向什么方向投射，形状的最大距离，检测的碰撞器）
        return Physics2D.BoxCast((Vector2)transform.position + CheckOffset,CheckSize,0,dir,CheckDistance,AttackLayer);
        // 作为一个bool值返回（本质上是返回的一个类型信息）
    }
    /// <summary>
    /// 状态的检测和转换
    /// </summary>
    /// <param name="state"></param>
    public void SwitchState(NPCstate state)
    {
        // 新版的"语法糖"
        // 也就是传入一个新的状态,在枚举内判断
        // 之后再改变状态
        var newState = state switch
        {
            NPCstate.Patrol => patrolState,
            NPCstate.Chase => chaseState,
            _=>null
        };
        currnetState.OnExit();
        currnetState = newState;
        currnetState.OnEnter(this);
    }


    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)CheckOffset + new Vector3( CheckDistance * -transform.localScale.x,0),1f);    
    }
}
