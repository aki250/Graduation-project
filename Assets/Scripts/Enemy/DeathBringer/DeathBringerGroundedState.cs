using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerGroundedState : DeathBringerState
{
    protected Transform player; //��ȡ���λ��

    public DeathBringerGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.CloseBossHPAndName();
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        //����boss����״̬
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    stateMachine.ChangeState(enemy.teleportState);
        //}

        //��������ɨ�跶Χ�ڼ�⵽��ң���������ڵ��˺󷽵�������������˻�������ҵĽŲ���������ս��״̬
        if ((enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position)
            < enemy.playerHearDistance) && !player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.battleState);
            return;
        }
    }
}
