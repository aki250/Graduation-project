using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeathBringerMoveState : DeathBringerGroundedState
{
    public DeathBringerMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter(); 
    }

    public override void Exit()
    {
        base.Exit(); 
        //AudioManager.instance.StopSFX(24);
    }

    public override void Update()
    {
        base.Update(); 

        //�����ǰ״̬�Ѿ������ƶ�״̬����ִ�����´���
        if (enemy.stateMachine.currentState != this)
        {
            return;
        }

        //�����ƶ�ʱ����Ч
        //AudioManager.instance.PlaySFX(24, enemy.transform);
        //AudioManager.instance.PlaySFX(14, enemy.transform);

        //�����⵽ǽ�ڻ���û�м�⵽����
        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            //�л�������״̬
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        enemy.SetVelocity(enemy.patrolMoveSpeed * enemy.facingDirection, rb.velocity.y);
    }
}