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

    // ====== 图层信息 ======
    public int playerLayer { get; set; }
    public int enemyLayer { get; set; }

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

    // ====== 攻击系统 ======
    public bool AttackPressedThisFrame { get; set; }
    public bool UpAttackPressedThisFrame { get; set; }
    public bool DownAttackPressedThisFrame { get; set; }
    public int AttackComboIndex { get; set; }          // 当前连招段数: 0=无, 1=第一段, 2=第二段, 3=第三段
    public int MaxComboCount { get; set; } = 3;        // 最大连招段数
    public bool IsAttacking { get; set; }              // 是否正在攻击
    public bool AttackAnimFinished { get; set; }       // 攻击动画是否完成
    public bool CanCombo { get; set; }                 // 是否可以连招
    public float ComboWindowTimer { get; set; }        // 连招窗口计时器

    // ====== 空中攻击方向 ======
    public bool IsDownAttacking { get; set; }          // 是否正在下劈
    public bool IsUpAttacking { get; set; }            // 是否正在上劈
    public bool DownAttackBounced { get; set; }        // 下劈是否已弹跳

    // ====== 土狼时间（从状态里搬出来）======
    public float CoyoteTime { get; set; } = 0.2f;
    public float CoyoteTimer { get; set; }         // >0 时允许"离地后仍可跳"

    // ====== 技能输入 ======
    public bool SkillPressedThisFrame { get; set; }     // 刚开始按下
    public bool IsHoldingSkill { get; set; }            // 持续按着
    public bool SkillPerformedThisFrame { get; set; }   // 技能动作被执行

    // ====== 专注系统 ======
    public float CurrentFocus { get; set; }
    public float MaxFocus { get; set; } = 50;

    // ====== 治愈状态 ======
    public bool IsHealing { get; set; }

    // ====== 霹雳一闪状态 ======
    public bool IsLightCutCharging { get; set; }       // 是否正在蓄力霹雳一闪
    public bool IsLightCutting { get; set; }       // 是否正在执行霹雳一闪冲刺
    public bool LightCutPressedThisFrame { get; set; }      // 刚开始按下 => 开始播放第一段动画
    public bool IsHoldingLightCut { get; set; }             // 持续按着 => 维持蓄力状态，播放第二段动画
    public bool LightCutPerformedThisFrame { get; set; }    // 蓄力蓄满 => 进行提示，可以松开按键

    // ====== 攻击力Buff技能 ======
    public bool LightCrownPressedThisFrame { get; set; }    // 刚开始按下
    public bool IsHoldingLightCrown { get; set; }           // 持续按着
    public bool LightCrownPerformedThisFrame { get; set; }  // Buff动作被执行
    public bool IsBuffing { get; set; }                     // 是否正在施加Buff

    // ====== 受伤状态 ======
    public bool IsHurt { get; set; }  // 是否正在受伤

    // ===== 激活状态 =====
    public bool ActivatePressedThisFrame { get; set; }  // 刚开始按下激活键
    public bool IsHoldingActivate { get; set; }  // 持续按着激活键
    public bool ActivatePerformedThisFrame { get; set; }  // 激活动作被执行

    // ===== 休息状态 =====
    public bool CanRest { get; set; }  // 是否可以休息
    public bool RestPressedThisFrame { get; set; }  // 按下休息键

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

    /// <summary>
    /// 重置攻击状态
    /// </summary>
    public void ResetAttackState()
    {
        AttackComboIndex = 0;
        IsAttacking = false;
        AttackAnimFinished = false;
        CanCombo = false;
        ComboWindowTimer = 0f;
        IsDownAttacking = false;
        IsUpAttacking = false;
        DownAttackBounced = false;
    }

    /// <summary>
    /// 进入下一段连招
    /// </summary>
    public void AdvanceCombo()
    {
        AttackComboIndex++;
        if (AttackComboIndex > MaxComboCount)
            AttackComboIndex = 1;
        AttackAnimFinished = false;
        CanCombo = false;
        ComboWindowTimer = 0f;
    }
}