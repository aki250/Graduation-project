using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                        //宝箱死亡
public class TextEnemyDeathState : TextEnemyState
{
    private bool canBeFliedUP = true; //标记敌人是否可以飞起

    public TextEnemyDeathState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, TextEnemy _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        enemy.cd.enabled = false; //禁用敌人的碰撞器，防止死亡时的交互

        stateTimer = 0.1f; //初始化状态计时器
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update(); 

        if (stateTimer > 0 && canBeFliedUP)
        {
            //模拟死亡动画
            enemy.rb.velocity = new Vector2(0, 10);

            //禁用敌人文本碰撞器，防止死亡动画期间有交互
            enemy.textCollider.enabled = false;

            canBeFliedUP = false;   //标记敌人飞起，不再重复
        }
    }
}
