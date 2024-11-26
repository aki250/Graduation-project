using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherGroundedState : ArcherState
{
    protected Transform player;

    public ArcherGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Archer _enemy) :
        base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // �����Ҵ��ڵ��˵�ɨ�跶Χ��,���߾���С��������ΧplayerHearDistance
        // �������δ���������˽��л���ս��״̬��battleState��
        if ((enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position) < enemy.playerHearDistance)
            && !player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.battleState);
            return;
        }
    }
}
