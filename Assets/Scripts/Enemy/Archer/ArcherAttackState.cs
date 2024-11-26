using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                        //表示弓箭手的攻击状态
public class ArcherAttackState : ArcherState
{
    public ArcherAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //设置计时器，使敌人在攻击动作开始时向前移动一小段时间
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        //更新敌人最后攻击时间
        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //如果计时器大于0
        if (stateTimer > 0)
        {
            //如果敌人被击退，不再向前移动
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;  //重置计时器
                return;      
            }

            //敌人在攻击动作开始时向前移动
            //enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            //攻击后停止移动，保持垂直方向的速度不变
            enemy.SetVelocity(0, rb.velocity.y);
        }

        if (triggerCalled)
        {
            //切换到战斗状态
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
