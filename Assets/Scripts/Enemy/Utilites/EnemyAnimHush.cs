using UnityEngine;

public class EnemyAnimHush
{
    public static readonly int Run = Animator.StringToHash("run");
    public static readonly int Walk = Animator.StringToHash("walk");
    public static readonly int Hurt = Animator.StringToHash("hurt");
    public static readonly int Death = Animator.StringToHash("Death");
    public static readonly int IsBroken = Animator.StringToHash("isBroken");
    public static readonly int Attack = Animator.StringToHash("Attack");

    // Boss 专用动画 Hash
    public static readonly int Intro = Animator.StringToHash("Intro");
    public static readonly int Attack1 = Animator.StringToHash("Attack1");
    public static readonly int Attack2 = Animator.StringToHash("Attack2");
    public static readonly int Attack3 = Animator.StringToHash("Attack3");
    public static readonly int AttackCombo = Animator.StringToHash("AttackCombo");
    public static readonly int Dodge = Animator.StringToHash("Dodge");
    public static readonly int DodgeCounter = Animator.StringToHash("DodgeCounter");
    public static readonly int DodgeCounterPierce = Animator.StringToHash("DodgeCounterPierce");
}