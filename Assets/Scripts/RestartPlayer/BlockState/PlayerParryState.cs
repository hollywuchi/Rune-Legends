using RestartPlayer.HFSM;
using UnityEngine;

/// <summary>
/// 弹反成功状态：短暂的反馈状态，播放弹反特效后快速退出
/// </summary>
public class PlayerParryState : PlayerState
{
    private float parryTimer;
    private const float ParryDuration = 0.3f; // 弹反状态持续时间

    public PlayerParryState(PlayerServices s) : base(s) { }

    public override void Enter()
    {
        base.Enter();
        s.ctx.IsParrying = true;

        // 播放弹反动画
        // s.anim.TriggerParrySuccess();

        // 触发弹反特效
        // s.fxSpeaker.CreateFX(s.motor.transform.position, s.ctx.FacingDirection, ParticalEffectType.ParrySpark);

        // 触发屏幕震动
        s.cameraShakeEvent?.RaiseEvent();

        // Hit Stop（时间冻结）
        s.motor.StartCoroutine(HitStop());

        // 减少自身架势值
        s.ctx.CurrentPosture = Mathf.Max(s.ctx.CurrentPosture - s.config.parryPostureRecovery, 0f);

        parryTimer = 0f;

        // 冻结输入
        s.inputGate.Freeze(ParryDuration);

        Debug.Log("完美格挡！");
    }

    public override Transition LogicUpdate()
    {
        parryTimer += Time.deltaTime;

        if (parryTimer >= ParryDuration)
        {
            // 松开格挡键 -> 回到Idle
            if (!s.ctx.BlockIsHeld)
                return new Transition(PlayerStateId.Idle);
            // 继续按住 -> 回到格挡状态
            return new Transition(PlayerStateId.Block);
        }

        return Transition.None;
    }

    public override void Exit()
    {
        base.Exit();
        s.ctx.IsParrying = false;
    }

    /// <summary>
    /// Hit Stop 效果：短暂冻结时间
    /// </summary>
    private System.Collections.IEnumerator HitStop()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(s.config.hitStopDuration);
        Time.timeScale = originalTimeScale;
    }
}
