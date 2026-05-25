using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 第三段攻击状态（终结技）
/// 支持循环连招：第三段攻击后可以衔接第一段攻击
/// </summary>
public class PlayerAttackCombo3State : PlayerAttackState
{
    // 第三段攻击的连招窗口更晚打开，给玩家更明确的终结技反馈
    protected override float ComboWindowDelay => 0.35f;
    protected override float ComboWindowDuration => 0.4f;

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
        // 第三段后循环到第一段，实现无限连招
        return new Transition(PlayerStateId.AttackCombo1);
    }

    public override void Exit()
    {
        base.Exit();
        // 重置连招状态，为下一轮连招做准备
        s.ctx.ResetAttackState();
    }
}