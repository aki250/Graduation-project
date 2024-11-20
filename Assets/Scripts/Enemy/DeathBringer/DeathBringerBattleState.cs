using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeathBringerBattleState : DeathBringerState
{
    private Transform player;  //���λ�õ�����
    private int moveDirection; //�����ƶ�����

    public DeathBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //����ս��״̬ʱ����Ĭ�ϵĹ���ʱ��
        stateTimer = enemy.aggressiveTime;

        //��ȡ��Ҷ�������
        player = PlayerManager.instance.player.transform;

        //�����Ҵӱ��󹥻������˻�����ת�����
        FacePlayer();

        //��ʾBoss��HP������UI
        enemy.ShowBossHPAndName();

        //�������Ѿ��������л����ƶ�״̬
        if (player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    //�˳�ս��״̬ʱֹͣ������Ч
    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(24);  //ֹͣ�˳�״̬ʱ����Ч
    }

    public override void Update()
    {
        base.Update();

        //�����ǰ״̬�Ѿ������仯����ǰ����
        if (enemy.stateMachine.currentState != this)
        {
            return;
        }

        //ʼ���õ�����ս��״̬���������
        FacePlayer();

        //������˼�⵽���
        if (enemy.IsPlayerDetected())
        {
            // ����״̬��ʱ��Ϊ����ʱ��
            stateTimer = enemy.aggressiveTime;

            //����ڹ�����Χ�ڣ����Թ���
            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                {
                    anim.SetBool("Idle", false);
                    anim.SetBool("Move", false);
                    stateMachine.ChangeState(enemy.attackState);
                    return;
                }
            }
        }
        else  //����û�п������
        {
            //�����һ��ʱ����һֱ���ڵ�����Ұ�ڣ��л�������״̬����ս��
            if (stateTimer < 0)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        //�������ڵ��˵Ĺ�����Χ֮���������ڵ��˱��󣬵��˳�������ƶ�
        if (enemy.IsPlayerDetected() && Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.attackDistance)
        {
            ChangeToIdleAnimation(); //���̫�����л�����������
            enemy.SetVelocity(0, rb.velocity.y); // ֹͣˮƽ������ƶ�
            return;
        }

        // ������ҵ�λ���жϵ����ƶ��ķ���
        if (player.position.x > enemy.transform.position.x)
        {
            moveDirection = 1; //�����ƶ�
        }
        else if (player.position.x < enemy.transform.position.x)
        {
            moveDirection = -1; //�����ƶ�
        }

        //���û�м�⵽���棬ֹͣ�ƶ����л�Ϊ��������
        if (!enemy.IsGroundDetected())
        {
            enemy.SetVelocity(0, rb.velocity.y);
            ChangeToIdleAnimation();
            return;
        }

        // �����趨��ս���ƶ��ٶȳ�����ƶ�
        enemy.SetVelocity(enemy.battleMoveSpeed * moveDirection, rb.velocity.y);
        ChangeToMoveAnimation();
    }

    //�������Ƿ���Թ���
    private bool CanAttack()
    {
        //ȷ�����˴��ڵ��桢û�б����˲��ҹ�����ȴʱ���ѹ�
        if (Time.time - enemy.lastTimeAttacked >= enemy.attackCooldown && !enemy.isKnockbacked && rb.velocity.y <= 0.1f && rb.velocity.y >= -0.1f)
        {
            //��������Ĺ�����ȴʱ�䣬�ù���Ƶ�ʸ��ӱ仯
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            return true;
        }

        return false;
    }

    //�л���������
    private void ChangeToIdleAnimation()
    {
        anim.SetBool("Move", false);
        anim.SetBool("Idle", true);
    }

    //�л��ƶ�����
    private void ChangeToMoveAnimation()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Move", true);
    }

    //ȷ������ʼ���������
    private void FacePlayer()
    {
        //�������ڵ��˵���࣬����ת�����
        if (player.transform.position.x < enemy.transform.position.x)
        {
            if (enemy.facingDirection != -1)
            {
                enemy.Flip();
            }
        }

        //�������ڵ��˵��Ҳ࣬����ת���Ҳ�
        if (player.transform.position.x > enemy.transform.position.x)
        {
            if (enemy.facingDirection != 1)
            {
                enemy.Flip();
            }
        }
    }
}
