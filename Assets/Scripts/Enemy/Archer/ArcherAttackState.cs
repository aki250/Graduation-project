using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ArcherAttackState�̳���ArcherState����ʾ�����ֵĹ���״̬
public class ArcherAttackState : ArcherState
{
    // ���캯������ʼ��״̬
    public ArcherAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    // ״̬����ʱ���߼�
    public override void Enter()
    {
        base.Enter();

        // ����һ����ʱ����ʹ�����ڹ���������ʼʱ��ǰ�ƶ�һС��ʱ��
        stateTimer = 0.1f;
    }

    // ״̬�˳�ʱ���߼�
    public override void Exit()
    {
        base.Exit();

        // ���µ��˵����һ�ι���ʱ��
        enemy.lastTimeAttacked = Time.time;
    }

    // ״̬����ʱ���߼�
    public override void Update()
    {
        base.Update();

        // �����ʱ������0
        if (stateTimer > 0)
        {
            // ������˱����ˣ�������ǰ�ƶ�
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;  // ���ü�ʱ��
                return;          // ��ǰ����
            }

            // �����ڹ���������ʼʱ��ǰ�ƶ���ע�͵��Ĵ���������߼���δ���ã�
            // enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            // ������ֹͣ�ƶ������ִ�ֱ������ٶȲ���
            enemy.SetVelocity(0, rb.velocity.y);
        }

        // ���������������������������
        if (triggerCalled)
        {
            // �л���ս��״̬
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
