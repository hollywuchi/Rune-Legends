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
    public bool IsTouchingWall { get; set; }

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

    // ====== 跳跃缓冲 ======
    public float JumpBufferTime { get; set; } = 0.12f;
    public float JumpBufferTimer { get; set; }     // >0 时允许“预按跳跃”

    // public void JumpBufferTick()
    // {
    //     if (JumpPressedThisFrame)
    //         JumpBufferTimer = JumpBufferTime;
    //     else
    //         JumpBufferTimer -= Time.deltaTime;

    //     if (JumpBufferTimer < 0f)
    //         JumpBufferTimer = 0f;
    // }

    // public bool HasJumpBuffered => JumpBufferTimer > 0;

    // public bool TryConsumeJumpBuffered()
    // {
    //     if (JumpBufferTimer > 0)
    //     {
    //         JumpBufferTimer = 0;
    //         return true;
    //     }

    //     return false;
    // }

    // public void ClearJumpBuffer() => JumpBufferTimer = 0;

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