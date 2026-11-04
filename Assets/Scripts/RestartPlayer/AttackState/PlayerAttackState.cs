using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 攻击状态基类，管理连招逻辑
/// </summary>
public class PlayerAttackState : PlayerGroundState
{
    protected bool isAnimFinished;
    protected bool ComboBuffered;

    // 自动连招窗口配置
    protected virtual float ComboWindowDelay => 0.2f; // 连招窗口打开延迟（秒）
    protected virtual float ComboWindowDuration => 0.5f; // 连招窗口持续时间（秒）

    public PlayerAttackState(PlayerServices s) : base(s)
    {
    }

    public override void Enter()
    {
        base.Enter();
        s.context.IsAttacking = true;
        s.context.AttackAnimFinished = false;
        s.context.CanCombo = false;
        isAnimFinished = false;
        ComboBuffered = false;

        // 攻击时减速
        s.motor.SetVelocityX(0f);

        // 确保底层 Base Layer 回到 Idle，避免从跑步切入攻击时底层动画卡在跑步状态
        // s.anim.SetBreak();

        // 冻结移动输入
        // s.inputGate.Freeze(0.1f);
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 受伤状态检查（优先级最高）
        if (s.context.IsHurt)
        {
            // 受伤打断攻击，返回受伤状态
            return new Transition(PlayerStateId.Hurt);
        }

        if (s.context.ConsumeAttack())
            ComboBuffered = true;


        // if (attackTimer >= 0.1f && s.context.ConsumeAttack())
        // ComboBuffered = true;

        // 检测连招输入
        if (s.context.CanCombo && ComboBuffered)
        {
            ComboBuffered = false;
            return GetNextComboTransition();
        }

        // 动画结束，返回地面状态（若仍按住方向键则直接衔接移动）
        if (isAnimFinished)
        {
            if (Mathf.Abs(s.context.MoveInput.x) >= 0.1f)
                return new Transition(PlayerStateId.Locomotion);

            return new Transition(PlayerStateId.Idle);
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        // 攻击时保持静止或微小位移
        s.motor.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
        s.context.IsAttacking = false;
        s.anim.SetIsAttacking(false);
        ComboBuffered = false;
    }

    /// <summary>
    /// 获取下一段连招的转换（由子类实现）
    /// </summary>
    protected virtual Transition GetNextComboTransition()
    {
        return Transition.None;
    }

    /// <summary>
    /// 动画事件回调：攻击动画完成
    /// </summary>
    public void OnAttackAnimFinished()
    {
        isAnimFinished = true;
        s.context.AttackAnimFinished = true;
    }

    /// <summary>
    /// 动画事件回调：可以连招了
    /// </summary>
    public void OnComboWindowOpen()
    {
        s.context.CanCombo = true;
    }

    /// <summary>
    /// 动画事件回调：连招窗口关闭
    /// </summary>
    public void OnComboWindowClose()
    {
        s.context.CanCombo = false;
    }

    /// <summary>
    /// 应用攻击位移
    /// </summary>
    public void ApplyAttackMove(float distance)
    {
        float dir = s.context.FacingDirection;
        s.motor.Teleport(s.motor.transform.position + new Vector3(dir * distance, 0, 0));
    }
}