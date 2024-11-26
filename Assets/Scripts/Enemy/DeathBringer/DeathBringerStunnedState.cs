using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                             //眩晕状态
public class DeathBringerStunnedState : DeathBringerState
{
    //控制眩晕状态下，移动时间计时器
    private float moveTimer;

    public DeathBringerStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //开始眩晕视觉特效 红闪，以 0.1 秒间隔
        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        //状态计时器，眩晕持续时间
        stateTimer = enemy.stunDuration;

        moveTimer = 0.1f;

        // 设置眩晕期间的初始移动速度（眩晕位移方向与面向方向相反）
        rb.velocity = new Vector2(enemy.stunMovement.x * -enemy.facingDirection, enemy.stunMovement.y);
    }

    public override void Exit()
    {
        base.Exit();

        //停止闪烁特效
        enemy.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();

        //减少移动计时器
        moveTimer -= Time.deltaTime;

        //如果移动计时器到期，停止水平移动，但保留垂直速度
        if (moveTimer < 0)
        {
            enemy.SetVelocity(0, rb.velocity.y);
        }

        //如果眩晕时间结束，切换回空闲状态
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
