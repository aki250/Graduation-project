using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                        //传送
public class DeathBringerTeleportState : DeathBringerState
{
    public DeathBringerTeleportState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //重置传送概率为默认值
        enemy.chanceToTeleport = enemy.defaultChanceToTeleport;

        //敌人进入无敌状态（无敌===================
        enemy.stats.BecomeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        //取消敌人无敌状态
        enemy.stats.BecomeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        //如果当前状态已被替换，则不继续执行
        if (stateMachine.currentState != this)
        {
            return;
        }

        //如果传送动画触发器已被调用
        if (triggerCalled)
        {
            //判断敌人是否可以释放法术
            if (enemy.CanCastSpell())
            {
                                                     //可以释放法术，则切换施法状态
                stateMachine.ChangeState(enemy.castState);
            }
            else
            {
                                                        //否则战斗状态
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}

