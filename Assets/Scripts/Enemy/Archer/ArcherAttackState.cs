using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                        //��ʾ�����ֵĹ���״̬
public class ArcherAttackState : ArcherState
{
    public ArcherAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //���ü�ʱ����ʹ�����ڹ���������ʼʱ��ǰ�ƶ�һС��ʱ��
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        //���µ�����󹥻�ʱ��
        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //�����ʱ������0
        if (stateTimer > 0)
        {
            //������˱����ˣ�������ǰ�ƶ�
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;  //���ü�ʱ��
                return;      
            }

            //�����ڹ���������ʼʱ��ǰ�ƶ�
            //enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            //������ֹͣ�ƶ������ִ�ֱ������ٶȲ���
            enemy.SetVelocity(0, rb.velocity.y);
        }

        if (triggerCalled)
        {
            //�л���ս��״̬
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
