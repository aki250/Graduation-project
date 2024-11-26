using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerGroundedState : DeathBringerState
{
    protected Transform player; //获取玩家位置

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
        //测试boss传送状态
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    stateMachine.ChangeState(enemy.teleportState);
        //}

        //敌人在其扫描范围内检测到玩家，或者玩家在敌人后方但距离过近，敌人会听到玩家的脚步声并进入战斗状态
        if ((enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position)
            < enemy.playerHearDistance) && !player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.battleState);
            return;
        }
    }
}
