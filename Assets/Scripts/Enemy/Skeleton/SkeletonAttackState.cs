using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
                                                                    //���ù���״̬
public class SkeletonAttackState : EnemyState
{
    Enemy_Skeleton enemy;

    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;                              //������˶������ã������ں��������з���
    }

    public override void Enter()
    {
        base.Enter(); 

        stateTimer = 0.1f;  //������״̬����һ�����ݳ���ʱ�䣨ǰҡ��
    }

    public override void Exit()
    {
        base.Exit(); 

        enemy.lastTimeAttacked = Time.time; //��¼�ϴι�����ʱ�䣬��ֹ���������ٴι���
    }

    public override void Update()
    {
        base.Update(); 

        if (stateTimer > 0)
        {
            if (enemy.isKnockbacked)
            {
                //���˴��ڻ���״̬����ֹͣ�ƶ����˳�����״̬
                stateTimer = 0;
                return; 
            }

            // ���򣬵����ڹ����ĳ�ʼ�׶λ���΢��ǰ�ƶ�һС�ξ���
            // �������ͨ���޸ĵ��˵��ٶ���ʵ�֣�����ˮƽ�ٶ�Ϊ `enemy.battleMoveSpeed * enemy.facingDirection`
            // enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            // һ����������ʱ�����������ֹͣ��ǰ�ƶ�
            enemy.SetVelocity(0, rb.velocity.y);
        }

        //����������־�Ѿ������ã��л���ս��״̬
        if (triggerCalled)
        {
            //�л���ս��״̬��ͨ���ǵȴ������ƶ��򹥻�����
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
