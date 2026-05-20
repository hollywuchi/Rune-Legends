using UnityEngine;
using UnityEngine.Events;


// TODO：将攻击伤害计算跑通
// TODO：添加打击反馈
/// <summary>
/// 攻击判定框组件，挂载到武器或攻击判定区域
/// 通过动画事件启用/禁用，检测敌人并造成伤害
/// </summary>
public class AttackHitbox : MonoBehaviour
{
    [Header("攻击参数")]
    public int baseDamage = 1;
    public float knockbackForce = 5f;
    public LayerMask hitLayers;

    [Header("判定区域")]
    public Vector2 hitboxOffset = new Vector2(1f, 0f);
    public Vector2 hitboxSize = new Vector2(1.5f, 1f);

    [Header("事件")]
    public UnityEvent<Transform> OnHitTarget;

    private PlayerContext ctx;
    private PlayerConfig config;
    private bool hasHitThisAttack;  // 防止同一攻击多次命中

    private void Awake()
    {
        // 获取父对象的Player组件
        var player = GetComponentInParent<Player>();
        if (player != null)
        {
            ctx = player.ctx;
            config = player.config;
        }
    }

    /// <summary>
    /// 启用攻击判定（动画事件调用）
    /// </summary>
    public void EnableHitbox()
    {
        hasHitThisAttack = false;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 禁用攻击判定（动画事件调用）
    /// </summary>
    public void DisableHitbox()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (hasHitThisAttack) return;

        // 检测敌人
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + hitboxOffset * GetFacingDirection(),
            hitboxSize,
            0f,
            hitLayers
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hasHitThisAttack = true;
                ApplyDamage(hit);
                OnHitTarget?.Invoke(hit.transform);
                break;
            }
        }
    }

    /// <summary>
    /// 对目标造成伤害
    /// </summary>
    private void ApplyDamage(Collider2D target)
    {
        var character = target.GetComponent<Character>();
        if (character == null) return;

        // 计算伤害（基础伤害 * 连招倍率）
        int finalDamage = CalculateDamage();
        
        // 创建临时Attack对象传递伤害信息
        var attackInfo = new AttackInfo
        {
            Damage = finalDamage,
            knockbackForce = knockbackForce,
            attackerTransform = transform.root
        };

        // 调用敌人的受伤方法
        var enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnTakeDamage(transform.root);
        }

        // 通过Character组件扣血
        character.TakeDamage(attackInfo);
    }

    /// <summary>
    /// 根据连招段数计算伤害
    /// </summary>
    private int CalculateDamage()
    {
        if (ctx == null || config == null) return baseDamage;

        float multiplier = ctx.AttackComboIndex switch
        {
            1 => config.combo1DamageMultiplier,
            2 => config.combo2DamageMultiplier,
            3 => config.combo3DamageMultiplier,
            _ => 1f
        };

        return Mathf.RoundToInt(baseDamage * multiplier);
    }

    private int GetFacingDirection()
    {
        return ctx != null ? ctx.FacingDirection : 1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        int dir = ctx != null ? ctx.FacingDirection : 1;
        Gizmos.DrawWireCube(
            (Vector2)transform.position + hitboxOffset * dir,
            hitboxSize
        );
    }
}

/// <summary>
/// 攻击信息类，用于传递伤害数据
/// </summary>
public class AttackInfo : Attack
{
    public float knockbackForce;
    public Transform attackerTransform;

    private void Awake()
    {
        // 确保Damage被正确初始化
        Damage = 1;
    }
}