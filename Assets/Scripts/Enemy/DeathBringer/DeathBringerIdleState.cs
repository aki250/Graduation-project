using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeathBringerIdleState : DeathBringerGroundedState
{
    public DeathBringerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //AudioManager.instance.StopSFX(14);
        stateTimer = enemy.patrolStayTime; // 初始化计时器为敌人的巡逻停留时间
    }

    public override void Exit()
    {
        base.Exit();

        //AudioManager.instance.PlaySFX(14, enemy.transform);
    }

    public override void Update()
    {
        base.Update(); 

        //当前状态已经不是空闲状态，则不执行以下代码
        if (stateMachine.currentState != enemy.idleState)
        {
            return;
        }

        enemy.SetVelocity(0, rb.velocity.y); //设置敌人的水平速度为0，保持垂直速度

        //如果计时器小于0，即超过了巡逻停留时间
        if (stateTimer < 0)
        {
            enemy.Flip(); //敌人翻转，改变面向
            stateMachine.ChangeState(enemy.moveState); //切换到移动状态
        }
    }
}
