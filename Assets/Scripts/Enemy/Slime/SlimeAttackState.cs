using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                                                      //ʷ��ķ����״̬
public class SlimeAttackState : SlimeState
{
    public SlimeAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Slime _slime) : base(_enemyBase, _stateMachine, _animBoolName, _slime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //ʹ�����ڹ�����ʼʱ����ǰ�ƶ�һ��
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
            //����������ǰ�ƶ�
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;
                return;
            }
            enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {   
            enemy.SetVelocity(0, rb.velocity.y);    //����ֹͣ�ƶ�
        }

        //�������������������л�ս��״̬
        if (triggerCalled)
        {
            stateMachine.ChangeState(enemy.battleState);
        }

    }
}
