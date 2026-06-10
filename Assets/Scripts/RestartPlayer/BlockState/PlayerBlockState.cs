using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 格挡状态：按住格挡键进入，包含弹反窗口判定
/// </summary>
public class PlayerBlockState : PlayerState
{
    private bool isInParryWindow;
    private bool blockHitAnimPlayed;
    private bool isAnimFinished;

    public PlayerBlockState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.IsBlocking = true;
        s.anim.SetIsBlocking(true);
        s.anim.TriggerBlock();
        s.motor.SetVelocityX(0f);
        s.ctx.isParry = false;

        blockHitAnimPlayed = false;
        // Debug.Log("进入格挡状态，弹反窗口开启");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (s.ctx.isParry && isInParryWindow)
        {
            return new Transition(PlayerStateId.Parry);
        }
        // 架势破碎判定
        if (s.ctx.IsPostureBroken)
            return new Transition(PlayerStateId.PostureBroken);

        // 松开格挡键 -> 退出格挡
        if (!s.ctx.BlockIsHeld && isAnimFinished)
            return new Transition(PlayerStateId.Idle);

        // 离地 -> 退出格挡
        if (!s.ctx.IsGrounded)
            return new Transition(PlayerStateId.Fall);



        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        s.motor.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
        s.anim.SetIsBlocking(false);
        s.ctx.IsBlocking = false;
        // Debug.Log("退出格挡状态，弹反窗口关闭");
    }

    /// <summary>
    /// 是否在弹反窗口内
    /// </summary>
    public bool IsInParryWindow()
    {
        return isInParryWindow;
    }

    public void SetParryWindow(bool isActive)
    {
        isInParryWindow = isActive;
    }

    public void OnBlockAnimFinished()
    {
        isAnimFinished = true;
    }

    /// <summary>
    /// 被攻击命中时调用（由 Attack.cs 触发）
    /// </summary>
    public void OnBlocked(bool isParry)
    {
        if (isParry)
        {
            s.ctx.isParry = isParry;
            // TODO：播放弹反粒子
        }
        else
        {
            // TODO：播放普通粒子
            if (!blockHitAnimPlayed)
            {
                // s.anim.TriggerBlockHit();
                // blockHitAnimPlayed = true;
            }
        }
    }

    // public void HandleBlockInteraction(Character attackerCharacter, Enemy enemy)
    // {
    //     if (IsInParryWindow())
    //     {
    //         Debug.Log("完美格挡！");
    //         // 弹反成功！
    //         // 对攻击者造成架势伤害
    //         if (attackerCharacter != null && attackerCharacter.postureSystem != null)
    //         {
    //             attackerCharacter.postureSystem.AddPosture(s.character.config.parryPostureDamageToEnemy);
    //         }

    //         // 减少目标自身架势值
    //         if (s.character.postureSystem != null)
    //         {
    //             s.character.postureSystem.ReducePosture(s.character.config.parryPostureRecovery);
    //         }

    //         // 通知格挡状态弹反成功
    //         OnBlocked(true);

    //         // 通知敌人被弹反
    //         enemy?.OnBlockedByPlayer(true, s.character.transform);
    //     }
    //     else
    //     {
    //         // 普通格挡
    //         // TODO:伤害判定对目标造成减伤
    //         // targetCharacter.TakeBlockedDamage(this, targetCharacter.config.blockDamageReduction);

    //         // 对目标造成架势伤害
    //         if (s.character.postureSystem != null)
    //         {
    //             s.character.postureSystem.AddPosture(s.character.config.blockPostureDamage);
    //         }

    //         // 通知格挡状态普通格挡
    //         OnBlocked(false);

    //         // 通知敌人被格挡
    //         enemy?.OnBlockedByPlayer(false, s.character.transform);
    //     }
    // }

}
