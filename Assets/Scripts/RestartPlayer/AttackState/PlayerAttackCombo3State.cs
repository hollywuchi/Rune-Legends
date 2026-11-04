using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 第三段攻击状态（终结技）
/// 支持循环连招：第三段攻击后可以衔接第一段攻击
/// </summary>
public class PlayerAttackCombo3State : PlayerAttackState
{

    public PlayerAttackCombo3State(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.context.AttackComboIndex = 3;
        s.anim.SetAttackCombo(3);
        s.anim.SetIsAttacking(true);
        s.anim.TriggerAttack();
    }

    protected override Transition GetNextComboTransition()
    {
        // 第三段后循环到第一段，实现无限连招
        return new Transition(PlayerStateId.AttackCombo1);
    }

    public override void Exit()
    {
        base.Exit();
        // 重置连招状态，为下一轮连招做准备
        s.context.ResetAttackState();
    }
}