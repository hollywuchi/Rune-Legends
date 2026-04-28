using UnityEngine;

/// <summary>
/// PlayerContext是一个纯数据类，存储玩家的输入、传感器状态、能力状态等信息。
/// PlayerState通过访问PlayerContext来决定何时转移状态。
/// </summary>
public sealed class PlayerContext
{
    // ====== 输入（每帧由Player.Update采样写入）======
    public Vector2 MoveInput { get; set; }
    public bool JumpPressedThisFrame { get; set; }
    public bool SprintPressedThisFrame { get; set; }
    public bool SprintIsHeld { get; set; }

    // ====== 传感器 ======
    public bool IsGrounded { get; set; }
    public bool IsTouchingLeftWall { get; set; }
    public bool IsTouchingRightWall { get; set; }
    public bool IsTouchingWall => IsTouchingLeftWall || IsTouchingRightWall;
    public bool IsTouchingTopLeftWall { get; set; }
    public bool IsTouchingTopRightWall { get; set; }
    public bool IsTouchingTopWall => IsTouchingTopLeftWall || IsTouchingTopRightWall;
    // ====== 朝向 ======
    public int FacingDirection { get; private set; } = 1;

    // ====== 能力与计数 ======
    public int JumpCount { get; set; }             // 0=未跳，1=一段，2=二段
    public int MaxJumpCount { get; set; } = 2;

    public bool CanSprint { get; set; } = true;
    public bool IsSprintFinished { get; set; }

    // ====== 土狼时间（从状态里搬出来）======
    public float CoyoteTime { get; set; } = 0.2f;
    public float CoyoteTimer { get; set; }         // >0 时允许“离地后仍可跳”

    public void SetFacingDirection(int dir)
    {
        FacingDirection = dir >= 0 ? 1 : -1;
    }
    /// <summary>
    /// 翻转朝向，只是数值方面
    /// </summary>
    public void FlipFacing()
    {
        FacingDirection *= -1;
    }
}