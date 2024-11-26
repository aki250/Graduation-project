using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                        //����
public class DeathBringerTeleportState : DeathBringerState
{
    public DeathBringerTeleportState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //���ô��͸���ΪĬ��ֵ
        enemy.chanceToTeleport = enemy.defaultChanceToTeleport;

        //���˽����޵�״̬���޵�===================
        enemy.stats.BecomeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        //ȡ�������޵�״̬
        enemy.stats.BecomeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        //�����ǰ״̬�ѱ��滻���򲻼���ִ��
        if (stateMachine.currentState != this)
        {
            return;
        }

        //������Ͷ����������ѱ�����
        if (triggerCalled)
        {
            //�жϵ����Ƿ�����ͷŷ���
            if (enemy.CanCastSpell())
            {
                                                     //�����ͷŷ��������л�ʩ��״̬
                stateMachine.ChangeState(enemy.castState);
            }
            else
            {
                                                        //����ս��״̬
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}

