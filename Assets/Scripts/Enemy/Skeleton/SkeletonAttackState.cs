using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
                                                                    //骷髅攻击状态
public class SkeletonAttackState : EnemyState
{
    Enemy_Skeleton enemy;

    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;                              //保存敌人对象引用，方便在后续操作中访问
    }

    public override void Enter()
    {
        base.Enter(); 

        stateTimer = 0.1f;  //给攻击状态设置一个短暂持续时间（前摇）
    }

    public override void Exit()
    {
        base.Exit(); 

        enemy.lastTimeAttacked = Time.time; //记录上次攻击的时间，防止敌人立即再次攻击
    }

    public override void Update()
    {
        base.Update(); 

        if (stateTimer > 0)
        {
            if (enemy.isKnockbacked)
            {
                //敌人处于击退状态，则停止移动并退出攻击状态
                stateTimer = 0;
                return; 
            }

            // 否则，敌人在攻击的初始阶段会稍微向前移动一小段距离
            // 这里可以通过修改敌人的速度来实现，设置水平速度为 `enemy.battleMoveSpeed * enemy.facingDirection`
            // enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            // 一旦攻击初期时间结束，敌人停止向前移动
            enemy.SetVelocity(0, rb.velocity.y);
        }

        //攻击触发标志已经被设置，切换到战斗状态
        if (triggerCalled)
        {
            //切换到战斗状态，通常是等待敌人移动或攻击结束
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
