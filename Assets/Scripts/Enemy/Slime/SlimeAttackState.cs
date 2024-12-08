using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                                                      //史莱姆攻击状态
public class SlimeAttackState : SlimeState
{
    public SlimeAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _slime) : base(_enemyBase, _stateMachine, _animBoolName, _slime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //使敌人在攻击开始时，向前移动一点
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            //被击退则不向前移动
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;
                return;
            }
            enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {   
            enemy.SetVelocity(0, rb.velocity.y);    //否则停止移动
        }

        //触发攻击动画结束，切换战斗状态
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }

    }
}
