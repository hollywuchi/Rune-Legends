using UnityEngine;

/// <summary>
/// PlayerMotor2D封装了对Rigidbody2D的操作，提供了更语义化的方法来控制玩家的运动。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlayerMotor2D : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    private float _originalGravity;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public Vector2 Velocity => rb.velocity;

    public float GravityScale
    {
        get => rb.gravityScale;
        set => rb.gravityScale = value;
    }

    public void CaptureOriginalGravity()
    {
        _originalGravity = rb.gravityScale;
    }

    public void RestoreOriginalGravity()
    {
        rb.gravityScale = _originalGravity;
    }

    public void SetVelocity(Vector2 v)
    {
        rb.velocity = v;
    }

    public void SetVelocityX(float x)
    {
        rb.velocity = new Vector2(x, rb.velocity.y);
    }

    public void SetVelocityY(float y)
    {
        rb.velocity = new Vector2(rb.velocity.x, y);
    }

    public void Jump(float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // public void WallJump(float jumpForce, int facingDir, float horizontalBoost)
    // {
    //     rb.velocity = new Vector2(facingDir * horizontalBoost, jumpForce);
    // }

    public void WallJump(float jumpForce, int facingDir, float horizontalBoost)
    {
        rb.AddForce(new Vector2(facingDir * horizontalBoost, jumpForce), ForceMode2D.Impulse);
    }

    public void DashHorizontal(int facingDir, float dashSpeed)
    {
        rb.velocity = new Vector2(facingDir * dashSpeed, 0f);
    }
    /// <summary>
    /// 翻转玩家朝向,物理层面
    /// </summary>
    public void FlipFacing()
    {
        rb.transform.Rotate(0, 180f, 0);
    }
}