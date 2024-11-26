using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                        //��������
public class TextEnemyDeathState : TextEnemyState
{
    private bool canBeFliedUP = true; //��ǵ����Ƿ���Է���

    public TextEnemyDeathState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, TextEnemy _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        enemy.cd.enabled = false; //���õ��˵���ײ������ֹ����ʱ�Ľ���

        stateTimer = 0.1f; //��ʼ��״̬��ʱ��
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update(); 

        if (stateTimer > 0 && canBeFliedUP)
        {
            //ģ����������
            enemy.rb.velocity = new Vector2(0, 10);

            //���õ����ı���ײ������ֹ���������ڼ��н���
            enemy.textCollider.enabled = false;

            canBeFliedUP = false;   //��ǵ��˷��𣬲����ظ�
        }
    }
}
