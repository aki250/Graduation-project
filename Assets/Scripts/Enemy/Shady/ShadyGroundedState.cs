using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                                        //检测墙面
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

    // 当状态退出时执行
    public override void Exit()
    {
        base.Exit(); 
    }

    public override void Update()
    {
        base.Update();

        //敌人看到玩家，或玩家离敌人太近，且玩家没有死亡
        if ((enemy.IsPlayerDetected() || Vector2.Distance(player.position, enemy.transform.position) < enemy.playerHearDistance) && !player.GetComponent<PlayerStats>().isDead)
        {
            //切换战斗状态
            stateMachine.ChangeState(enemy.battleState);
            return;  
        }
    }
}

