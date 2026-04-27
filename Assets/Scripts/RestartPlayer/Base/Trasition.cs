using System.Collections.Generic;

namespace RestartPlayer.HFSM
{
    // 不写filepath：你可以放在 RestartPlayer/HFSM 下
    public enum PlayerStateId
    {
        // WORKFLOW:在这里添加状态ID
        None = 0,
        Idle, Locomotion, Turn, Sprint,
        Jump, Jump2, WallJump, Fall, AirSprint, WallSlide
    }
    /// <summary>
    /// 状态逻辑只“提出转移请求”，由状态机统一执行。
    /// </summary>
    public readonly struct Transition
    {
        public PlayerStateId Target { get; }
        public bool HasTarget => Target != PlayerStateId.None;
        public Transition(PlayerStateId target) => Target = target;
        public static Transition None => new Transition(PlayerStateId.None);
    }

    public sealed class PlayerStateRegistry
    {
        private readonly Dictionary<PlayerStateId, PlayerState> _map = new();

        public void Register(PlayerStateId id, PlayerState state) => _map[id] = state;
        public PlayerState Get(PlayerStateId id)
        {
            _map.TryGetValue(id, out var state);
            return state;
        }
    }
}