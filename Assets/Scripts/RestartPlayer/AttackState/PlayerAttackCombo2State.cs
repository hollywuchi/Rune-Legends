using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 第二段攻击状态
/// </summary>
public class PlayerAttackCombo2State : PlayerAttackState
{
    public PlayerAttackCombo2State(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.AttackComboIndex = 2;
        s.ctx.CanCombo = false;
        s.anim.SetAttackCombo(2);
        s.anim.SetIsAttacking(true);
        s.anim.TriggerAttack();
        
    }

    protected override Transition GetNextComboTransition()
    {
        // 可以进入第三段
        return new Transition(PlayerStateId.AttackCombo3);
    }
}