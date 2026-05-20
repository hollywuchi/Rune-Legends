using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 第三段攻击状态（终结技）
/// </summary>
public class PlayerAttackCombo3State : PlayerAttackState
{
    public PlayerAttackCombo3State(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.AttackComboIndex = 3;
        s.anim.SetAttackCombo(3);
        s.anim.SetIsAttacking(true);
        s.anim.TriggerAttack();
        
    }

    protected override Transition GetNextComboTransition()
    {
        // 第三段后不能继续连招，等待动画结束
        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        // 重置连招
        s.ctx.ResetAttackState();
    }
}