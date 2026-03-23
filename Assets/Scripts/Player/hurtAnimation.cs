 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hurtAnimation : StateMachineBehaviour
{
    // 动画开始函数
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // 动画重复播放的函数
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // 动画退出函数
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.GetComponent<PlayerController>().Ishurt = false;
    }

    // 动画退出函数
    // OnStateMove is called right after Animator.OnAnimatorMove()
    // OnStateMove在Animator之后立即调用。动画移动（）
    // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //    // Implement code that processes and affects root motion
       
    // }

    // 动画退出的时候也会调用这个函数
    // OnStateIK在Animator之后立即调用。OnAnimatorIK（）
    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
