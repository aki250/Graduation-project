using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherDeathState : ArcherState
{
    //标志位，控制弓箭手在死亡时触发飞起行为，仅允许触发一次
    private bool canBeFliedUP = true;

    public ArcherDeathState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //死亡动画
        enemy.anim.SetBool(enemy.lastAnimBoolName, true);

        //将动画速度设置为0
        enemy.anim.speed = 0;

        //禁用碰撞体，防止死亡后仍与玩家发生碰撞
        enemy.cd.enabled = false;

        //初始化状态计时器，用于控制飞起行为的触发时机
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //检查计时器和飞起
        if (stateTimer > 0 && canBeFliedUP)
        {
            //设置敌人的垂直速度为 10，模拟死亡时的飞起效果
            enemy.SetVelocity(0, 10);

            //禁用飞起标志，防止重复触发
            canBeFliedUP = false;
        }
    }
}
