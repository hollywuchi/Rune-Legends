using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Permissions;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    [HideInInspector] public PhysicsCheck physicsCheck;
    [HideInInspector] public Animator anim;

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

    [Header("架势系统")]
    public PostureSystem postureSystem;  // 架势系统组件引用

    [Header("被格挡反应参数")]
    public bool isBlocked;                       // 是否被格挡/弹反
    public float parryRecoilForce = 8f;          // 弹反后退力
    public float blockRecoilForce = 5f;          // 格挡后退力
    public float parryStaggerDuration = 0.8f;    // 弹反硬直时间
    public float blockStaggerDuration = 0.5f;    // 格挡硬直时间

    // 现在的状态
    BaseState currnetState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        postureSystem = GetComponent<PostureSystem>();
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
        dir = new Vector3(-transform.localScale.x, 0, 0);

        currnetState.LogicUpdate();
        TimeCount();
    }
    private void FixedUpdate()
    {
        if (!Ishurt && !IsDeath && !IsWait && !isBlocked)
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
        rb.velocity = new Vector3(CurrentSpeed * dir.x * Time.deltaTime, rb.velocity.y, 0);
    }

    /// <summary>
    /// 行走撞墙等待时间计数器
    /// </summary>
    void TimeCount()
    {
        if (IsWait)
        {
            WaitTimeCounter -= Time.deltaTime;
            if (WaitTimeCounter <= 0)
            {
                IsWait = false;
                WaitTimeCounter = WaitTime;
                transform.localScale = new Vector3(dir.x, 1, 1);
                physicsCheck.GroundOffset.x *= transform.localScale.x;
            }
        }
        if (!FindPlayer())
        {
            LostTimeCounter -= Time.deltaTime;
            if (LostTimeCounter <= 0)
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
        if (postureSystem.isBroken) return;
        attacker = Attacker;
        if (attacker.transform.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        if (attacker.transform.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);

        Ishurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attacker.transform.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(WaitAttack(dir));

    }

    IEnumerator WaitAttack(Vector2 dir)
    {
        rb.AddForce(dir * Backforce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        Ishurt = false;
    }

    public void OnDie()
    {
        IsDeath = true;

        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("Death", IsDeath);
    }

    /// <summary>
    /// 架势破碎处理（被弹反多次后触发）
    /// </summary>
    public void OnPostureBroken()
    {
        Ishurt = true;
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetTrigger("hurt");
        anim.SetBool("isBroken", true);
        StartCoroutine(PostureBrokenRecovery());
    }

    private IEnumerator PostureBrokenRecovery()
    {
        if (postureSystem == null) yield break;
        Debug.Log("进入破防硬直");
        yield return new WaitUntil(() => postureSystem.isBroken == false);
        anim.SetBool("isBroken", false);
        Ishurt = false;
    }

    /// <summary>
    /// 被玩家格挡/弹反时的反应：停止移动 + 后退 + 硬直
    /// </summary>
    public void OnBlockedByPlayer(bool isParry, Transform playerTransform)
    {
        isBlocked = true;
        // 立即停止移动
        rb.velocity = new Vector2(0, rb.velocity.y);

        // 计算后退方向（远离玩家）
        Vector2 recoilDir = (transform.position - playerTransform.position).normalized;

        if (isParry)
        {
            // 弹反：大后退 + 长硬直
            rb.AddForce(recoilDir * parryRecoilForce, ForceMode2D.Impulse);
            anim.SetTrigger("hurt");
            StartCoroutine(BlockedRecovery(parryStaggerDuration));
        }
        else
        {
            // 普通格挡：小后退 + 短硬直
            rb.AddForce(recoilDir * blockRecoilForce, ForceMode2D.Impulse);
            anim.SetTrigger("hurt");
            StartCoroutine(BlockedRecovery(blockStaggerDuration));
        }
    }

    private IEnumerator BlockedRecovery(float duration)
    {
        yield return new WaitForSeconds(duration);
        isBlocked = false;
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
        return Physics2D.BoxCast((Vector2)transform.position + CheckOffset, CheckSize, 0, dir, CheckDistance, AttackLayer);
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
            _ => null
        };
        currnetState.OnExit();
        currnetState = newState;
        currnetState.OnEnter(this);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)CheckOffset + new Vector3(CheckDistance * -transform.localScale.x, 0), 1f);
    }
}
