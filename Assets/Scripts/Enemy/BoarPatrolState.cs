using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        CurrentEnemy = enemy;
    }

    public override void LogicUpdate()
    {
        // 如果发现了player就切换到追击状态
        if(CurrentEnemy.FindPlayer())
        {
            CurrentEnemy.SwitchState(NPCstate.Chase);
        }

        // 现在是巡逻状态
        if(!CurrentEnemy.physicsCheck.IsGround || (CurrentEnemy.physicsCheck.touchLeftWall && CurrentEnemy.dir.x<0) || (CurrentEnemy.physicsCheck.touchRightWall && CurrentEnemy.dir.x > 0))
        {
            CurrentEnemy.IsWait = true;
            CurrentEnemy.anim.SetBool("walk",false);
        }
        else
        {
            CurrentEnemy.anim.SetBool("walk",true);
        }
    }


    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        CurrentEnemy.anim.SetBool("walk",false);
        // Debug.Log("退出巡逻状态");
    }
}
