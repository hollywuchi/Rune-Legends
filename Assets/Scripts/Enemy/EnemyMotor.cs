using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    private Character character;
    private float FacingDir => -transform.localScale.x;
    public Transform Attacker { get; private set; } 
    public bool IsBlocked { get; private set; }
    public bool attackFinished;
    private float blockTimer;

    [Header("敌人视野参数")] 
    public Vector2 offset;
    public Vector2 checkSize;

    private float hurtTime;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        if (character != null)
        {
            character.OnHurt += GetHurt;
            character.OnAttackBlocked += HandleAttackBlocked;
        }
    }

    private void OnDisable()
    {
        if (character != null)
        {
            character.OnHurt -= GetHurt;
            character.OnAttackBlocked -= HandleAttackBlocked;
        }
    }

    private void Update()
    {
        if (hurtTime > 0)
        {
            hurtTime -= Time.deltaTime;
            if (hurtTime <= 0 && character != null)
            {
                character.isHurt = false;
            }
        }

        if (blockTimer > 0)
        {
            blockTimer -= Time.deltaTime;
            if (blockTimer < 0)
                IsBlocked = false;
        }
    }

    public void Move(float velocity) =>
        rb.velocity = new Vector2(velocity * FacingDir, rb.velocity.y);

    private void GetHurt(DamageData data)
    {
        Attacker = data.attacker != null ? data.attacker.transform : null;
        character.isHurt = true;
        hurtTime = 0.5f;
        // 击退效果
        rb.velocity = Vector2.zero;
        rb.AddForce(data.knockBackDir * data.knockBackForce, ForceMode2D.Impulse);
        // 生成特效
        EventBus<FxEvent>.Raise(new FxEvent { dir = 1, playerTran = data.hitPoint, type = ParticalEffectType.Hit });
    }

    public void FaceTarget(Transform target)
    {
        if (target == null) return;
        var dirX = target.position.x - transform.position.x;
        if (Mathf.Approximately(dirX, 0)) return;
        transform.localScale = new Vector3(dirX > 0 ? -1 : 1, 1, 1);
    }

    /// <summary>
    /// 发现玩家的方法
    /// </summary>
    public Transform FindPlayer()
    {
        var hit = Physics2D.OverlapBox(GetBoxOri(), checkSize, 0f, LayerMask.GetMask("Player"));
        return hit != null ? hit.transform : null;
    }

    private Vector2 GetBoxOri() => (Vector2)transform.position + new Vector2(offset.x * FacingDir, offset.y);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetBoxOri(), checkSize);
    }

    /// <summary>
    /// 被格挡之后的反应
    /// </summary>
    /// <param name="isParry"></param>
    /// <param name="playerTrans"></param>
   private void HandleAttackBlocked(bool isParry,Transform playerTrans)
    {
        IsBlocked = true;
        var backForce = isParry ? 8f : 4f;
        blockTimer = isParry ? 0.8f : 0.5f;

        rb.velocity = Vector2.zero;
        var backDir = (transform.position - playerTrans.position).normalized;
        rb.AddForce(new Vector2( backDir.x * backForce,0), ForceMode2D.Impulse);
    }

    /// <summary>
    /// 转身方法
    /// </summary>
    public void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }
    
    public void Stop() => rb.velocity = Vector2.zero;

    public void AnimCallBack() => attackFinished = true;
}