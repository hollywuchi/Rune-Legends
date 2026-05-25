using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 第一段攻击状态
/// </summary>
public class PlayerAttackCombo1State : PlayerAttackState
{
    public PlayerAttackCombo1State(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.AttackComboIndex = 1;
        s.anim.SetAttackCombo(1);
        s.anim.SetIsAttacking(true);
        s.anim.TriggerAttack();
    }

    protected override Transition GetNextComboTransition()
    {
        // 可以进入第二段
        return new Transition(PlayerStateId.AttackCombo2);
    }
}