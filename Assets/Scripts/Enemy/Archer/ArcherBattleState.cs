using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArcherBattleState : ArcherState
{
    private Transform player;

    public ArcherBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //���õ���Ĭ�Ϲ���ʱ�䣬                           ������Ҵӱ���ӽ�ʱ�����˷����л�״̬
        stateTimer = enemy.aggressiveTime;

        player = PlayerManager.instance.player.transform;

        //��ɫ�ƺ󣬵�������������
        FacePlayer();

        if (player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        //AudioManager.instance.StopSFX(24);
    }

    public override void Update()
    {
        base.Update();

        //�����ǰ״̬����ս��״̬��ֱ�ӷ��أ�����״̬��ͻ
        if (enemy.stateMachine.currentState != this)
        {
            return;
        }

        //��ֹ�����α�Ե
        FacePlayer();

        if (enemy.IsPlayerDetected())
        {
            //������˼�⵽��ң�����빥��ģʽ�����ü�ʱ��
            stateTimer = enemy.aggressiveTime;

            // ��Ҿ������ʱ��������Ծ����������
            if (Vector2.Distance(player.transform.position, enemy.transform.position) < enemy.jumpJudgeDistance)
            {
                if (CanJump())
                {
                    anim.SetBool("Idle", false);
                    anim.SetBool("Move", false);
                    stateMachine.ChangeState(enemy.jumpState);
                    return;
                }
            }

            // ����ڹ�����Χ��ʱ�������л�������״̬
            if (Vector2.Distance(player.transform.position, enemy.transform.position) < enemy.attackDistance)
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
        else
        {
            // �����Ҳ���ɨ�跶Χ�ڣ������л���Ѳ��״̬
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > enemy.playerScanDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        //�����Ҿ����Զ���ڵ�����󣬵���������ƶ��������˽ӽ����ʱ��ֹͣ�ƶ����л�Ϊ��������
        if (enemy.IsPlayerDetected() && Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.attackDistance)
        {
            ChangeToIdleAnimation();
            return;
        }

    }

    /// �����������������������µĹ�����ȴʱ�䡣
    private bool CanAttack()
    {

        //������Ҫ������ȴʱ���ѽ���,�ҵ���δ�����˵�������
        if (Time.time - enemy.lastTimeAttacked >= enemy.attackCooldown && !enemy.isKnockbacked)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            return true;
        }

        return false;
    }

    //�������Ƿ������Ծ����Ҫ�����û�пӻ�ǽ�ڣ���ȴʱ���ѽ�����  ����δ�����˵�������
    private bool CanJump()
    {
        if (!enemy.GroundBehindCheck() || enemy.WallBehindCheck())
        {
            return false;
        }

        if (Time.time - enemy.lastTimeJumped >= enemy.jumpCooldown && !enemy.isKnockbacked)
        {
            return true;
        }

        return false;
    }

    private void ChangeToIdleAnimation()
    {
        anim.SetBool("Move", false);
        anim.SetBool("Idle", true);
    }

    //����ڵ��˵����Ҳ࣬��ת���˵ĳ����������ҡ�
    private void FacePlayer()
    {
        if (player.transform.position.x < enemy.transform.position.x)
        {
            if (enemy.facingDirection != -1)
            {
                enemy.Flip();
            }
        }

        if (player.transform.position.x > enemy.transform.position.x)
        {
            if (enemy.facingDirection != 1)
            {
                enemy.Flip();
            }
        }
    }

}
