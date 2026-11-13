using System;
using System.Collections;
using UnityEngine;

public class EnemyMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    private Character character;
    public bool isHurt;
    
    [Header("攻击距离参数")]
    public Vector2 checkOffset;
    public Vector2 checkSize;
    public float checkDistance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float velocity) =>
        rb.velocity = new Vector2(velocity * transform.localScale.x, rb.velocity.y);

    public void GetHurt(DamageData data)
    {
        isHurt = true;
        // 挨打掉头
        var dirX = data.attacker.transform.position.x - transform.position.x;
        transform.localScale = new Vector3(dirX > 0 ? -1 : 1, 1, 1);

        // 击退效果
        rb.velocity = Vector2.zero;
        rb.AddForce(data.knockBackDir * data.knockBackForce, ForceMode2D.Impulse);
        // 生成特效
        EventBus<FxEvent>.Raise(new FxEvent { dir = 1, playerTran = data.hitPoint, type = ParticalEffectType.Hit });
    }

    /// <summary>
    /// 受伤硬直协程
    /// </summary>
    /// <param name="stunTime">硬直时间</param>
    /// <returns></returns>
    public IEnumerator HurtStun(float stunTime)
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        isHurt = false;
    }

    /// <summary>
    /// 发现玩家的方法
    /// </summary>
    /// <param name="checkOffset">原点</param>
    /// <param name="checkSize">大小</param>
    /// <param name="dir">方向</param>
    /// <param name="checkDistance">距离</param>
    /// <param name="attackLayer">检测的LayerMask</param>
    /// <returns></returns>
    public Transform FindPlayer()
    {
        return Physics2D.BoxCast((Vector2)transform.position + checkOffset, checkSize, 0, transform.localScale, checkDistance,
            LayerMask.GetMask("Player")).transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + (Vector3)checkOffset + new Vector3(checkDistance * -transform.localScale.x, 0), 1f);
    }

    /// <summary>
    /// 被玩家格挡之后的反应
    /// </summary>
    /// <param name="playerTransform">玩家的位置</param>
    /// <param name="recoilForce">被击退的力</param>
    /// <param name="recoverTime">恢复时间</param>
    public void OnBlockedByPlayer(Transform playerTransform, float recoilForce, float recoverTime)
    {
        // 立即停止移动
        rb.velocity = new Vector2(0, rb.velocity.y);

        // 计算后退方向（远离玩家）
        Vector2 recoilDir = (transform.position - playerTransform.position).normalized;

        rb.AddForce(recoilDir * recoilForce, ForceMode2D.Impulse);

        StartCoroutine(BlockedRecovery(recoverTime));
    }

    private IEnumerator BlockedRecovery(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    /// <summary>
    /// 转身方法
    /// </summary>
    public void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

    public void Stop() => rb.velocity = Vector2.zero;
}