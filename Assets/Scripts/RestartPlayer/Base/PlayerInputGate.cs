using UnityEngine;

/// <summary>
/// 将原始输入加工为“最终输入”。支持短时间冻结（比如 WallJump 出招锁定）
/// </summary>
public sealed class PlayerInputGate
{
    private float _freezeTimer;

    public bool IsFrozen => _freezeTimer > 0f;

    /// <summary>
    /// 冻结输入 duration 秒。可重复调用，取更长的时间
    /// </summary>
    public void Freeze(float duration)
    {
        if (duration <= 0f) return;
        _freezeTimer = Mathf.Max(_freezeTimer, duration);
    }

    public void Tick(float dt)
    {
        if (_freezeTimer <= 0f) return;
        _freezeTimer -= dt;
        if (_freezeTimer < 0f) _freezeTimer = 0f;
    }

    /// <summary>
    /// 处理移动输入：冻结时输出 0
    /// </summary>
    public Vector2 FilterMove(Vector2 rawMove)
    {
        return IsFrozen ? Vector2.zero : rawMove;
    }

    /// <summary>
    /// 处理按钮类输入：冻结时直接吞掉
    /// </summary>
    public bool FilterButton(bool rawPressed)
    {
        return IsFrozen ? false : rawPressed;
    }
}