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

        //设置敌人默认攻击时间，                           避免玩家从背后接近时，敌人反复切换状态
        stateTimer = enemy.aggressiveTime;

        player = PlayerManager.instance.player.transform;

        //角色绕后，敌人则会面向玩家
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

        //如果当前状态不是战斗状态，直接返回，避免状态冲突
        if (enemy.stateMachine.currentState != this)
        {
            return;
        }

        //防止卡地形边缘
        FacePlayer();

        if (enemy.IsPlayerDetected())
        {
            //如果敌人检测到玩家，则进入攻击模式并重置计时器
            stateTimer = enemy.aggressiveTime;

            // 玩家距离过近时，敌人跳跃以拉开距离
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

            // 玩家在攻击范围内时，敌人切换到攻击状态
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
            // 如果玩家不在扫描范围内，敌人切换到巡逻状态
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > enemy.playerScanDistance)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        //如果玩家距离较远或在敌人身后，敌人向玩家移动，当敌人接近玩家时，停止移动并切换为待机动画
        if (enemy.IsPlayerDetected() && Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.attackDistance)
        {
            ChangeToIdleAnimation();
            return;
        }

    }

    /// 如果满足条件，会随机生成新的攻击冷却时间。
    private bool CanAttack()
    {

        //攻击需要满足冷却时间已结束,且敌人未被击退的条件。
        if (Time.time - enemy.lastTimeAttacked >= enemy.attackCooldown && !enemy.isKnockbacked)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            return true;
        }

        return false;
    }

    //检查敌人是否可以跳跃。需要满足后方没有坑或墙壁，冷却时间已结束且  敌人未被击退的条件。
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

    //玩家在敌人的左右侧，翻转敌人的朝向以面对玩家。
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
