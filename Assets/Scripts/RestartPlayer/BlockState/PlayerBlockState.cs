using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 格挡状态：按住格挡键进入，包含弹反窗口判定
/// </summary>
public class PlayerBlockState : PlayerGroundState
{
    private bool isInParryWindow;
    private bool blockHitAnimPlayed;
    private bool isAnimFinished;

    public PlayerBlockState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.context.IsBlocking = true;
        s.anim.SetIsBlocking(true);
        s.anim.TriggerBlock();
        
        s.motor.SetVelocityX(0f);
        s.context.isParry = false;
        isAnimFinished = false;

        s.context.AttackBufferTimer = 0f;
        
        blockHitAnimPlayed = false;

        if (s.character != null)
            s.character.OnTryBlockDamage = HandleBlockDamage;
    }

    public override Transition LogicUpdate()
    {
        var t = base.LogicUpdate();
        if (t.HasTarget) return t;

        if (s.context.isParry && isInParryWindow)
        {
            return new Transition(PlayerStateId.Parry);
        }
        // 架势破碎判定
        if (s.context.IsPostureBroken)
            return new Transition(PlayerStateId.PostureBroken);

        // 松开格挡键 -> 退出格挡
        if (!s.context.BlockIsHeld && isAnimFinished)
            return new Transition(PlayerStateId.Idle);

        // 离地 -> 退出格挡
        if (!s.context.IsGrounded)
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
        s.context.IsBlocking = false;

        if (s.character != null && s.character.OnTryBlockDamage == HandleBlockDamage)
            s.character.OnTryBlockDamage = null;
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
            s.context.isParry = isParry;
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

    private bool HandleBlockDamage(DamageData data)
    {
        Character attackerCharacter = data.attacker != null ? data.attacker.GetComponent<Character>() : null;
        PostureSystem attackerPosture = data.attacker != null ? data.attacker.GetComponent<PostureSystem>() : null;

        Vector3 blockFxPoint = s.character.transform.position + new Vector3(s.context.FacingDirection, 2f, 0f);
        if (IsInParryWindow())
        {
            Debug.Log("完美格挡！");

            EventBus<FxEvent>.Raise(new FxEvent
            {
                dir = s.context.FacingDirection,
                playerTran = blockFxPoint,
                type = ParticalEffectType.PerfBlock
            });
            EventBus<BlockCameraShakeEvent>.Raise(new BlockCameraShakeEvent{Force = 0.5f});
            
            // 弹反成功！
            // 对攻击者造成架势伤害
            attackerPosture?.AddPosture(s.config.parryPostureDamageToEnemy);

            // 减少目标自身架势值
            s.postureSystem?.ReducePosture(s.config.parryPostureRecovery);

            // 通知格挡状态弹反成功
            OnBlocked(true);

            // 通知攻击者被弹反
            attackerCharacter?.NotifyAttackBlocked(true, s.character.transform);
        }
        else
        {
            // 普通格挡
            // 对目标造成架势伤害

            EventBus<FxEvent>.Raise(new FxEvent
            {
                dir = s.context.FacingDirection,
                playerTran = blockFxPoint,
                type = ParticalEffectType.BlockFx
            });
            EventBus<BlockCameraShakeEvent>.Raise(new BlockCameraShakeEvent{Force = 0.2f});
            
            s.postureSystem?.AddPosture(s.config.blockPostureDamage);

            // 通知格挡状态普通格挡
            OnBlocked(false);

            // 通知攻击者被格挡
            attackerCharacter?.NotifyAttackBlocked(false, s.character.transform);
        }

        return true;
    }
}
