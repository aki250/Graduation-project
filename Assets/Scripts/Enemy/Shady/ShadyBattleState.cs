using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShadyBattleState : ShadyState
{
    private Transform player;
    private int moveDirection;

    public ShadyBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.aggressiveTime;

        player = PlayerManager.instance.player.transform;

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

        if (enemy.stateMachine.currentState != this)
        {
            return;
        }

        //AudioManager.instance.PlaySFX(24, enemy.transform);

        //AudioManager.instance.PlaySFX(14, enemy.transform);

        FacePlayer();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.aggressiveTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {

                if (CanAttack())
                {
                    anim.SetBool("Idle", false);
                    anim.SetBool("BattleMove", false);
                    stateMachine.ChangeState(enemy.explosionState);
                    return;
                }
            }
        }
        else  //敌人没有看到玩家 
        {
            //玩家在一段时间内一直不在敌人视野内，切换到待机状态（脱战）
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > enemy.playerScanDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        //如果玩家在敌人的攻击范围之外或者玩家在敌人背后，敌人朝着玩家移动
        if (enemy.IsPlayerDetected() && Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.attackDistance)
        {
            ChangeToIdleAnimation();
            return;
        }

        //根据玩家的位置判断敌人移动的方向
        if (player.position.x > enemy.transform.position.x)
        {
            moveDirection = 1; //向右
        }
        else if (player.position.x < enemy.transform.position.x)
        {
            moveDirection = -1; //向左
        }

        //如果没有检测到地面，停止移动并切换为待机动画
        if (!enemy.IsGroundDetected())
        {
            enemy.SetVelocity(0, rb.velocity.y);
            ChangeToIdleAnimation();
            return;
        }

        //根据设定的战斗移动速度朝玩家移动
        enemy.SetVelocity(enemy.battleMoveSpeed * moveDirection, rb.velocity.y);
        ChangeToMoveAnimation();

    }

    private bool CanAttack()
    {
        //确保敌人处于地面、没有被击退并且攻击冷却时间已过
        if (Time.time - enemy.lastTimeAttacked >= enemy.attackCooldown && !enemy.isKnockbacked && rb.velocity.y <= 0.1f && rb.velocity.y >= -0.1f)
        {
            //设置随机的攻击冷却时间，让攻击频率更加变化
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            return true;
        }

        return false;
    }

    private void ChangeToIdleAnimation()
    {
        anim.SetBool("BattleMove", false);
        anim.SetBool("Idle", true);
    }

    private void ChangeToMoveAnimation()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("BattleMove", true);
    }

    //始终面向玩家
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
