using DG.Tweening;
using UnityEngine;

/// <summary>
/// PlayerMotor2D封装了对Rigidbody2D的操作，提供了更语义化的方法来控制玩家的运动。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerMotor2D : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private float _originalGravity;
    private Tween _forceMoveTween;

    public bool IsForceMoving { get; private set; }

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        StopForceMove();
    }

    public Vector2 Velocity => rb.velocity;

    public float GravityScale
    {
        get => rb.gravityScale;
        set => rb.gravityScale = value;
    }

    public void CaptureOriginalGravity()  => _originalGravity = rb.gravityScale;

    public void RestoreOriginalGravity()  => rb.gravityScale = _originalGravity;

    public void SetVelocity(Vector2 v) => rb.velocity = v;

    public void SetVelocityX(float x)  =>rb.velocity = new Vector2(x, rb.velocity.y);

    public void MoveVelocityX(float targetX, float maxDelta) =>
        rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, targetX, maxDelta), rb.velocity.y);

    public void SetVelocityY(float y)  => rb.velocity = new Vector2(rb.velocity.x, y);

    public void Jump(float jumpForce)  => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    public void WallJump(float jumpForce, int facingDir, float horizontalBoost) =>
        rb.AddForce(new Vector2(facingDir * horizontalBoost, jumpForce), ForceMode2D.Impulse);

    public void DashHorizontal(int facingDir, float dashSpeed) => rb.velocity = new Vector2(facingDir * dashSpeed, 0f);

    public void StopForceMove()
    {
        if (_forceMoveTween != null && _forceMoveTween.IsActive())
            _forceMoveTween.Kill();

        _forceMoveTween = null;
        IsForceMoving = false;
    }

    public void ForceMoveDistance(int facingDir, float distance, float duration = 0.2f, Ease ease = Ease.OutQuad)
    {
        if (rb == null || Mathf.Approximately(distance, 0f)) return;

        duration = Mathf.Max(0.05f, duration);
        float startSpeed = (3f * distance / duration) * facingDir;
        ForceMoveVelocity(startSpeed, duration, ease);
    }

    public void ForceMoveVelocity(float startSpeed, float duration, Ease ease = Ease.OutQuad)
    {
        if (rb == null) return;

        StopForceMove();
        duration = Mathf.Max(0.05f, duration);
        IsForceMoving = true;

        float currentSpeed = startSpeed;
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);

        _forceMoveTween = DOTween.To(() => currentSpeed, x =>
            {
                currentSpeed = x;
                if (rb != null)
                    rb.velocity = new Vector2(x, rb.velocity.y);
            }, 0f, duration)
            .SetEase(ease)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() =>
            {
                IsForceMoving = false;
                _forceMoveTween = null;
            })
            .OnKill(() =>
            {
                IsForceMoving = false;
                _forceMoveTween = null;
            });
    }

    public void LightCutDash(int facingDir, float cutForce) =>
        rb.AddForce(new Vector3(facingDir * cutForce, 0f), ForceMode2D.Impulse);

    /// <summary>
    /// 忽略玩家和目标层的碰撞
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="ignore"></param>
    public void IgnorePlayerCollision(int layer, bool ignore) =>
        Physics2D.IgnoreLayerCollision(gameObject.layer, layer, ignore);
    /// <summary>
    /// 翻转玩家朝向,物理层面
    /// </summary>
    public void FlipFacing() => rb.transform.Rotate(0, 180f, 0);

    public void Teleport(Vector2 position) => rb.position = position;

    public void SwtichInterpolate(bool enable)
    {
        rb.interpolation = enable switch
        {
            true => RigidbodyInterpolation2D.Interpolate,
            false => RigidbodyInterpolation2D.None,
        };
    }
}