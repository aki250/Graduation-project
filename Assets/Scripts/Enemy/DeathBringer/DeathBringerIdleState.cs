using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DeathBringerIdleState : DeathBringerGroundedState
{
    public DeathBringerIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //AudioManager.instance.StopSFX(14);
        stateTimer = enemy.patrolStayTime; // ��ʼ����ʱ��Ϊ���˵�Ѳ��ͣ��ʱ��
    }

    public override void Exit()
    {
        base.Exit();

        //AudioManager.instance.PlaySFX(14, enemy.transform);
    }

    public override void Update()
    {
        base.Update(); 

        //��ǰ״̬�Ѿ����ǿ���״̬����ִ�����´���
        if (stateMachine.currentState != enemy.idleState)
        {
            return;
        }

        enemy.SetVelocity(0, rb.velocity.y); //���õ��˵�ˮƽ�ٶ�Ϊ0�����ִ�ֱ�ٶ�

        //�����ʱ��С��0����������Ѳ��ͣ��ʱ��
        if (stateTimer < 0)
        {
            enemy.Flip(); //���˷�ת���ı�����
            stateMachine.ChangeState(enemy.moveState); //�л����ƶ�״̬
        }
    }
}
