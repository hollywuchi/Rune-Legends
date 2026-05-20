using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 攻击状态基类，管理连招逻辑
/// </summary>
public class PlayerAttackState : PlayerState
{
    protected float attackTimer;
    protected float comboWindowTimer;
    protected bool isAnimFinished;

    public PlayerAttackState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.IsAttacking = true;
        s.ctx.AttackAnimFinished = false;
        isAnimFinished = false;
        attackTimer = 0f;
        comboWindowTimer = 0f;

        // 攻击时减速
        s.motor.SetVelocityX(0f);
        
        // 冻结移动输入
        s.inputGate.Freeze(0.1f);
    }

    public override Transition LogicUpdate()
    {
        attackTimer += Time.deltaTime;

        // 检测连招输入
        if (s.ctx.CanCombo && s.ctx.AttackPressedThisFrame)
        {
            return GetNextComboTransition();
        }

        // 动画结束，返回地面状态
        if (isAnimFinished)
        {
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
        s.ctx.IsAttacking = false;
        s.anim.SetIsAttacking(false);
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
        s.ctx.AttackAnimFinished = true;
    }

    /// <summary>
    /// 动画事件回调：可以连招了
    /// </summary>
    public void OnComboWindowOpen()
    {
        s.ctx.CanCombo = true;
    }

    /// <summary>
    /// 动画事件回调：连招窗口关闭
    /// </summary>
    public void OnComboWindowClose()
    {
        s.ctx.CanCombo = false;
    }

    /// <summary>
    /// 应用攻击位移
    /// </summary>
    public void ApplyAttackMove(float distance)
    {
        float dir = s.ctx.FacingDirection;
        s.motor.Teleport(s.motor.transform.position + new Vector3(dir * distance, 0, 0));
    }
}