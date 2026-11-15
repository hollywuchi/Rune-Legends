using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyMotor : MonoBehaviour
{
    private Rigidbody2D rb;
    private Character character;
    private float FacingDir => -transform.localScale.x;
    public Transform attacker { get; private set; }

    [Header("敌人视野参数")] public Vector2 offset;
    public Vector2 checkSize;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        if (character != null) character.OnHurt += GetHurt;
    }

    private void OnDisable()
    {
        if (character != null) character.OnHurt -= GetHurt;
    }

    public void Move(float velocity) =>
        rb.velocity = new Vector2(velocity * FacingDir, rb.velocity.y);

    private void GetHurt(DamageData data)
    {
        attacker = data.attacker != null ? data.attacker.transform : null;

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
    /// 受伤硬直协程
    /// </summary>
    /// <param name="stunTime">硬直时间</param>
    /// <returns></returns>
    public IEnumerator HurtStun(float stunTime)
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
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