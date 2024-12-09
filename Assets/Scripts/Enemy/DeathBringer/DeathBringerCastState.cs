using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerCastState : DeathBringerState
{
    private int castAmount; //ʩ������
    private float castTimer;    //ʩ�����������

    public DeathBringerCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        castAmount = enemy.castAmount;  //����Ϊ�����˵�ʩ������
        castTimer = 0.5f;   //��ʼ����ʱ��
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeEnterSpellCastState = Time.time;  //���һ��ʩ��ʱ��
    }

    public override void Update()
    {
        base.Update();

        castTimer -= Time.deltaTime;

        //ֻ�е�����ʹ��ʩ�������з����󣬲����˳�ʩ��״̬
        if (CanCast())
        {
            enemy.CastSpell();
        }
        
        if (castAmount <= 0)
        {
            stateMachine.ChangeState(enemy.teleportState);
        }
    }

    private bool CanCast()
    {
        //������ʱ�䶼�������ʩ��
        if (castAmount >= 0 && castTimer < 0)
        {
            castAmount--;
            castTimer = enemy.castCooldown; //����ʩ����ʱ����Ϊ���˵�ʩ����ȴʱ��
            return true;
        }

        return false;
    }
}
