using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        CurrentEnemy = enemy;
        CurrentEnemy.CurrentSpeed = CurrentEnemy.chaseSpeed;
        CurrentEnemy.anim.SetBool("run",true);
    }
    
    public override void LogicUpdate()
    {
        if(CurrentEnemy.LostTimeCounter <= 0)
        {
            CurrentEnemy.SwitchState(NPCstate.Patrol);
            CurrentEnemy.CurrentSpeed = CurrentEnemy.NormalSpeed;
        }

        if(!CurrentEnemy.physicsCheck.IsGround || (CurrentEnemy.physicsCheck.touchLeftWall && CurrentEnemy.dir.x<0) || (CurrentEnemy.physicsCheck.touchRightWall && CurrentEnemy.dir.x > 0))
        {
            CurrentEnemy.transform.localScale = new Vector3(CurrentEnemy.dir.x,1,1);
        }

    }

    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        CurrentEnemy.anim.SetBool("run",false);
    }
}
