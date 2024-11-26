using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                             //ѣ��״̬
public class DeathBringerStunnedState : DeathBringerState
{
    //����ѣ��״̬�£��ƶ�ʱ���ʱ��
    private float moveTimer;

    public DeathBringerStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //��ʼѣ���Ӿ���Ч �������� 0.1 ����
        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        //״̬��ʱ����ѣ�γ���ʱ��
        stateTimer = enemy.stunDuration;

        moveTimer = 0.1f;

        // ����ѣ���ڼ�ĳ�ʼ�ƶ��ٶȣ�ѣ��λ�Ʒ������������෴��
        rb.velocity = new Vector2(enemy.stunMovement.x * -enemy.facingDirection, enemy.stunMovement.y);
    }

    public override void Exit()
    {
        base.Exit();

        //ֹͣ��˸��Ч
        enemy.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();

        //�����ƶ���ʱ��
        moveTimer -= Time.deltaTime;

        //����ƶ���ʱ�����ڣ�ֹͣˮƽ�ƶ�����������ֱ�ٶ�
        if (moveTimer < 0)
        {
            enemy.SetVelocity(0, rb.velocity.y);
        }

        //���ѣ��ʱ��������л��ؿ���״̬
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
