using UnityEngine;

public class DeathBringerAttackState : DeathBringerState
{
    public DeathBringerAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //�ڹ�����ʼʱ����һ��ʱ���ӳ٣����˻�����ǰλ��
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time; //�������һ�ι���ʱ��
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
        {
            //������˱����ˣ���ֹͣ������ǰ�ƶ�
            if (enemy.isKnockbacked)
            {
                stateTimer = 0;
                return;
            }

            // ���û�б����ˣ����˻��ڹ�����ʼʱ��ǰ�ƶ�
            // enemy.SetVelocity(enemy.battleMoveSpeed * enemy.facingDirection, rb.velocity.y);
        }
        else
        {
            // ����������ֹͣ���˵�ˮƽ�ƶ���ֻ������ֱ�ٶȣ���������Ӱ�죩
            enemy.SetVelocity(0, rb.velocity.y);
        }

        //����Ƿ���Ҫ����ת̬���
        if (triggerCalled)
        {
            //���˿��Դ��ͣ����л�������״̬
            if (enemy.CanTeleport())
            {
                stateMachine.ChangeState(enemy.teleportState);
            }
            else
            {
                //�������Ӵ��͸��ʲ��л���ս��״̬
                enemy.chanceToTeleport += 10;
                stateMachine.ChangeState(enemy.battleState);
            }
        }
    }
}
