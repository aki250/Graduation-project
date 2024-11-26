using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeathBringerMoveState : DeathBringerGroundedState
{
    public DeathBringerMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter(); 
    }

    public override void Exit()
    {
        base.Exit(); 
        //AudioManager.instance.StopSFX(24);
    }

    public override void Update()
    {
        base.Update(); 

        //如果当前状态已经不是移动状态，则不执行以下代码
        if (enemy.stateMachine.currentState != this)
        {
            return;
        }

        //播放移动时的音效
        //AudioManager.instance.PlaySFX(24, enemy.transform);
        //AudioManager.instance.PlaySFX(14, enemy.transform);

        //如果检测到墙壁或者没有检测到地面
        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            //切换到空闲状态
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        enemy.SetVelocity(enemy.patrolMoveSpeed * enemy.facingDirection, rb.velocity.y);
    }
}