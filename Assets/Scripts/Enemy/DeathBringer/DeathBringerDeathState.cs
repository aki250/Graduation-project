using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerDeathState : DeathBringerState
{
    private bool canBeFliedUP = true;
    public DeathBringerDeathState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.SetBool(enemy.lastAnimBoolName, true);
        enemy.anim.speed = 0;
        enemy.cd.enabled = false;   //禁用碰撞器，防止死亡时的交互

        stateTimer = 0.1f;  //初始化状态计时器

        enemy.CloseBossHPAndName(); //关闭Boss生命值和名称显示
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //状态计时器大于0且死亡使者可以飞起
        if (stateTimer > 0 && canBeFliedUP)
        {
            enemy.SetVelocity(0, 10);
            canBeFliedUP = false;
        }
    }
}
