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
        s.ctx.CanCombo = false;
        s.anim.SetAttackCombo(3);
        s.anim.SetIsAttacking(true);
        s.anim.TriggerAttack();
        
    }

    protected override Transition GetNextComboTransition()
    {
        return new Transition(PlayerStateId.AttackCombo1);
    }

    public override void Exit()
    {
        base.Exit();
        // 重置连招
        s.ctx.ResetAttackState();
    }
}