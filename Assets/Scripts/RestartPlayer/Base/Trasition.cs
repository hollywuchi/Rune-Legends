using System.Collections.Generic;

namespace RestartPlayer.HFSM
{
    public enum PlayerStateId
    {
        // WORKFLOW:在这里添加状态ID
        None = 0,
        Idle, Locomotion, Turn, Sprint, Rest,
        Jump, Jump2, WallJump, Fall, AirSprint, WallSlide, Climb,
        Attack, AttackCombo1, AttackCombo2, AttackCombo3, UpAttack,
        AirAttack, AirDownAttack, AirUpAttack,
        Heal, LightCut, LightCrown, Hurt,
        Death,
        Block, Parry, PostureBroken,
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