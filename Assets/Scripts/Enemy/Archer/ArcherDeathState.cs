using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherDeathState : ArcherState
{
    //��־λ�����ƹ�����������ʱ����������Ϊ����������һ��
    private bool canBeFliedUP = true;

    public ArcherDeathState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //��������
        enemy.anim.SetBool(enemy.lastAnimBoolName, true);

        //�������ٶ�����Ϊ0
        enemy.anim.speed = 0;

        //������ײ�壬��ֹ������������ҷ�����ײ
        enemy.cd.enabled = false;

        //��ʼ��״̬��ʱ�������ڿ��Ʒ�����Ϊ�Ĵ���ʱ��
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //����ʱ���ͷ���
        if (stateTimer > 0 && canBeFliedUP)
        {
            //���õ��˵Ĵ�ֱ�ٶ�Ϊ 10��ģ������ʱ�ķ���Ч��
            enemy.SetVelocity(0, 10);

            //���÷����־����ֹ�ظ�����
            canBeFliedUP = false;
        }
    }
}
