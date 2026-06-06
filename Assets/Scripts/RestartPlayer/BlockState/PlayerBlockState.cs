using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 格挡状态：按住格挡键进入，包含弹反窗口判定
/// </summary>
public class PlayerBlockState : PlayerState
{
    private float parryWindowTimer;
    private bool isInParryWindow;
    private bool blockHitAnimPlayed;

    public PlayerBlockState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.IsBlocking = true;
        s.anim.SetIsBlocking(true);
        s.motor.SetVelocityX(0f);

        parryWindowTimer = 0f;
        isInParryWindow = true;
        blockHitAnimPlayed = false;
        Debug.Log("进入格挡状态，弹反窗口开启");
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        // 架势破碎判定
        if (s.ctx.IsPostureBroken)
            return new Transition(PlayerStateId.PostureBroken);

        // 松开格挡键 -> 退出格挡
        if (!s.ctx.BlockIsHeld)
            return new Transition(PlayerStateId.Idle);

        // 离地 -> 退出格挡
        if (!s.ctx.IsGrounded)
            return new Transition(PlayerStateId.Fall);

        // 弹反窗口计时
        if (isInParryWindow)
        {
            parryWindowTimer += Time.deltaTime;
            if (parryWindowTimer >= s.config.parryWindowDuration)
            {
                isInParryWindow = false;
            }
        }

        return Transition.None;
    }

    public override void PhysicsUpdate()
    {
        s.motor.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
        s.ctx.IsBlocking = false;
        s.anim.SetIsBlocking(false);
    }

    /// <summary>
    /// 是否在弹反窗口内
    /// </summary>
    public bool IsInParryWindow()
    {
        return isInParryWindow;
    }

    /// <summary>
    /// 被攻击命中时调用（由 Attack.cs 触发）
    /// </summary>
    public void OnBlocked(bool isParry)
    {
        if (isParry)
        {
            // 弹反成功 -> 切换到弹反状态
            s.stateMachine.RequestChangeState(PlayerStateId.Parry);
        }
        else
        {
            // 普通格挡 -> 播放格挡受击动画
            if (!blockHitAnimPlayed)
            {
                s.anim.TriggerBlockHit();
                blockHitAnimPlayed = true;
            }
        }
    }
}
