using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerCastState : DeathBringerState
{
    private int castAmount; //施法次数
    private float castTimer;    //施法间隔计数器

    public DeathBringerCastState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        castAmount = enemy.castAmount;  //设置为，敌人的施法次数
        castTimer = 0.5f;   //初始化计时器
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeEnterSpellCastState = Time.time;  //最后一次施法时间
    }

    public override void Update()
    {
        base.Update();

        castTimer -= Time.deltaTime;

        //只有当死亡使者施放了所有法术后，才能退出施法状态
        if (CanCast())
        {
            enemy.CastSpell();
        }
        
        if (castAmount <= 0)
        {
            stateMachine.ChangeState(enemy.teleportState);
        }
    }

    private bool CanCast()
    {
        //次数，时间都满足才能施法
        if (castAmount >= 0 && castTimer < 0)
        {
            castAmount--;
            castTimer = enemy.castCooldown; //重置施法计时器，为敌人的施法冷却时间
            return true;
        }

        return false;
    }
}
