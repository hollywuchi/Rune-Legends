using RestartPlayer.HFSM;

public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    private readonly PlayerStateRegistry _registry;

    private PlayerStateId _pendingId = PlayerStateId.None;
    private bool _isInitialized;

    public PlayerStateMachine(PlayerStateRegistry registry)
    {
        _registry = registry;
    }

    public void Initialize(PlayerStateId startingStateId)
    {
        var startingState = _registry.Get(startingStateId);

        currentState = startingState;
        _isInitialized = true;

        currentState.Enter();
    }

    /// <summary>
    /// 状态内部调用：提出切换请求（本帧只接受第一次请求）
    /// </summary>
    public void RequestChangeState(PlayerStateId newStateId)
    {
        if (newStateId == PlayerStateId.None) return;
        if (_pendingId != PlayerStateId.None) return; // 本帧已有人抢到切换权
        _pendingId = newStateId;
    }

    /// <summary>
    /// Player.Update里调用：先跑逻辑，再统一提交切换
    /// </summary>
    public void Tick()
    {
        if (!_isInitialized || currentState == null) return;

        _pendingId = PlayerStateId.None;

        // 1) 当前状态提出“意图”
        var t = currentState.LogicUpdate();

        // 2) 状态内部也可能直接 RequestChangeState（比如动画事件回调）
        // 统一用 pending 优先
        if (_pendingId == PlayerStateId.None && t.HasTarget)
            _pendingId = t.Target;

        if (_pendingId != PlayerStateId.None)
            ChangeStateInternal(_pendingId);
    }

    public void FixedTick()
    {
        if (!_isInitialized || currentState == null) return;
        currentState.PhysicsUpdate();
    }

    private void ChangeStateInternal(PlayerStateId newStateId)
    {
        var newState = _registry.Get(newStateId);
        if (newState == null) return;
        if (newState == currentState) return;

        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}