using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    private Character character;
    private float DefaultSign => facingRight ? 1f : -1f;
    private float FacingDir => Mathf.Sign(transform.localScale.x) * DefaultSign;
    public Transform Attacker { get; private set; }
    public bool IsBlocked { get; private set; }
    public bool attackFinished;
    private float blockTimer;

    [Header("素材是否默认面向右")] public bool facingRight;
    [Header("敌人是否有霸体")] public bool hasSuperArmor;
    [Header("敌人视野参数")] public Vector2 offset;
    public Vector2 checkSizeFinding;
    public float checkSizeFound;
    [Tooltip("垂直方向上最大距离")] public float maxVerticalDistance;

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
        // 当前是霸体状态，只播放一个受击特效即可
        if (hasSuperArmor)
        {
            EventBus<FxEvent>.Raise(new FxEvent { dir = 1, playerTran = data.hitPoint, type = ParticalEffectType.Hit });
            return;
        }
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
        if (Mathf.Abs(dirX) < 0.2f) return;

        var desiredDir = dirX > 0 ? 1f : -1f;
        transform.localScale = new Vector3(desiredDir * DefaultSign, 1, 1);
    }

    /// <summary>
    /// 发现敌人方法（未发现玩家时）
    /// </summary>
    public Transform FindPlayer()
    {
        var hit = Physics2D.OverlapBox(GetBoxOri(), checkSizeFinding, 0f, LayerMask.GetMask("Player"));
        return hit != null ? hit.transform : null;
    }

    /// <summary>
    /// 警戒范围，如果玩家在这个范围，就会尝试追击
    /// </summary>
    /// <returns></returns>
    public Transform FoundPlayer()
    {
        var hit = Physics2D.OverlapCircle(transform.position, checkSizeFound, LayerMask.GetMask("Player"));
        if (hit == null) return null;

        // 因为警戒范围是
        return Mathf.Abs(hit.transform.position.y - transform.position.y) > maxVerticalDistance ? null : hit.transform;
    }

    /// <summary>
    /// 检测玩家是否在近战攻击范围内
    /// </summary>
    public bool CheckAttackRange(Vector2 attackOffset, Vector2 attackSize)
    {
        var hit = Physics2D.OverlapBox(GetAttackBoxOri(attackOffset), attackSize, 0f, LayerMask.GetMask("Player"));
        return hit != null;
    }

    private Vector2 GetAttackBoxOri(Vector2 attackOffset) =>
        (Vector2)transform.position + new Vector2(attackOffset.x * FacingDir, attackOffset.y);

    private Vector2 GetBoxOri() => (Vector2)transform.position + new Vector2(offset.x * FacingDir, offset.y);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetBoxOri(), checkSizeFinding);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkSizeFound);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x, transform.position.y + maxVerticalDistance));
    }

    /// <summary>
    /// 被格挡之后的反应
    /// </summary>
    /// <param name="isParry"></param>
    /// <param name="playerTrans"></param>
    private void HandleAttackBlocked(bool isParry, Transform playerTrans)
    {
        IsBlocked = true;
        var backForce = isParry ? 8f : 4f;
        blockTimer = isParry ? 0.8f : 0.5f;

        rb.velocity = Vector2.zero;
        var backDir = (transform.position - playerTrans.position).normalized;
        rb.AddForce(new Vector2(backDir.x * backForce, 0), ForceMode2D.Impulse);
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
    
    /// <summary>
    /// 敌人冲刺方法
    /// </summary>
    public void Dash(float speed,float forwardSign = 1)
    {
        rb.velocity = new Vector2(speed * FacingDir * forwardSign, rb.velocity.y);
    }
}