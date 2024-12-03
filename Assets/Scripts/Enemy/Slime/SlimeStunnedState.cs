using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStunnedState : SlimeState
{
    private float moveTimer;

    public SlimeStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _slime) : base(_enemyBase, _stateMachine, _animBoolName, _slime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        //僵直时间
        stateTimer = enemy.stunDuration;
        moveTimer = 0.1f;   //初始化僵直移动计时器

        //使敌人以一定速度在水平方向上向反方向移动
        rb.velocity = new Vector2(enemy.stunMovement.x * -enemy.facingDirection, enemy.stunMovement.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        moveTimer -= Time.deltaTime;

        if (moveTimer < 0)
        {
            enemy.SetVelocity(0, rb.velocity.y);
        }

        if (rb.velocity.y < 0.1f && enemy.IsGroundDetected())
        {
            //敌人开始下落并且已接触地面,触发动画
            enemy.anim.SetTrigger("StunTrigger");
            enemy.fx.Invoke("CancelColorChange", 0);    //停止红色闪烁
        }

        //僵直状态持续时间结束，切换Idle
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
