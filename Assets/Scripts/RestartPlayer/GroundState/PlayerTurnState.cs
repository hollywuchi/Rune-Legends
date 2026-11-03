// using RestartPlayer.HFSM;
// using UnityEngine;

// public class PlayerTurnState : PlayerGroundState
// {
//     public PlayerTurnState(PlayerServices s) : base(s) { }

//     public override void Enter()
//     {
//         base.Enter();
//         s.anim.PlayTrickTurn();
//     }

//     public override Transition LogicUpdate()
//     {
//         var t = base.LogicUpdate();
//         if (t.HasTarget) return t;

//         // if (s.ctx.MoveInput.x != 0 && Mathf.Sign(s.ctx.MoveInput.x) == s.ctx.FacingDirection)
//         //     return new Transition(PlayerStateId.Locomotion);
//         if (Mathf.Abs(s.ctx.MoveInput.x) < 0.01f)
//             s.stateMachine.RequestChangeState(PlayerStateId.Idle);
//         else
//             s.stateMachine.RequestChangeState(PlayerStateId.Locomotion);

//         return Transition.None;
//     }

//     public override void Exit()
//     {
//         base.Exit();
//         s.ctx.FlipFacing();
//         s.character.transform.Rotate(0, 180f, 0);
//     }
// }