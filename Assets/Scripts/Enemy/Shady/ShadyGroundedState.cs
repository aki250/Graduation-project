using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                        //���ǽ��
public class ShadyGroundedState : ShadyState
{
    protected Transform player;
    public ShadyGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        player = PlayerManager.instance.player.transform;
    }

    // ��״̬�˳�ʱִ��
    public override void Exit()
    {
        base.Exit(); 
    }

    public override void Update()
    {
        base.Update();

        //���˿�����ң�����������̫���������û������
        if ((enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position) < enemy.playerHearDistance) && !player.GetComponent<PlayerStats>().isDead)
        {
            //�л�ս��״̬
            stateMachine.ChangeState(enemy.battleState);
            return;  
        }
    }
}

