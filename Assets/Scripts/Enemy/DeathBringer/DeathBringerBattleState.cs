using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeathBringerBattleState : DeathBringerState
{
    private Transform player;  //玩家位置的引用
    private int moveDirection; //敌人移动方向

    public DeathBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, DeathBringer _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //进入战斗状态时设置默认的攻击时间
        stateTimer = enemy.aggressiveTime;

        //获取玩家对象引用
        player = PlayerManager.instance.player.transform;

        //如果玩家从背后攻击，敌人会立即转向玩家
        FacePlayer();

        //显示Boss的HP和名字UI
        enemy.ShowBossHPAndName();

        //如果玩家已经死亡，切换到移动状态
        if (player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    //退出战斗状态时停止播放音效
    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(24);  //停止退出状态时的音效
    }

    public override void Update()
    {
        base.Update();

        //如果当前状态已经发生变化，提前返回
        if (enemy.stateMachine.currentState != this)
        {
            return;
        }

        //始终让敌人在战斗状态下面向玩家
        FacePlayer();

        //如果敌人检测到玩家
        if (enemy.IsPlayerDetected())
        {
            // 重置状态计时器为激进时间
            stateTimer = enemy.aggressiveTime;

            //玩家在攻击范围内，尝试攻击
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
        else  //敌人没有看到玩家
        {
            //玩家在一段时间内一直不在敌人视野内，切换到待机状态（脱战）
            if (stateTimer < 0)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }
        }

        //如果玩家在敌人的攻击范围之外或者玩家在敌人背后，敌人朝着玩家移动
        if (enemy.IsPlayerDetected() && Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.attackDistance)
        {
            ChangeToIdleAnimation(); //如果太近，切换到待机动画
            enemy.SetVelocity(0, rb.velocity.y); // 停止水平方向的移动
            return;
        }

        // 根据玩家的位置判断敌人移动的方向
        if (player.position.x > enemy.transform.position.x)
        {
            moveDirection = 1; //向右移动
        }
        else if (player.position.x < enemy.transform.position.x)
        {
            moveDirection = -1; //向左移动
        }

        //如果没有检测到地面，停止移动并切换为待机动画
        if (!enemy.IsGroundDetected())
        {
            enemy.SetVelocity(0, rb.velocity.y);
            ChangeToIdleAnimation();
            return;
        }

        // 根据设定的战斗移动速度朝玩家移动
        enemy.SetVelocity(enemy.battleMoveSpeed * moveDirection, rb.velocity.y);
        ChangeToMoveAnimation();
    }

    //检查敌人是否可以攻击
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

    //切换待机动画
    private void ChangeToIdleAnimation()
    {
        anim.SetBool("Move", false);
        anim.SetBool("Idle", true);
    }

    //切换移动动画
    private void ChangeToMoveAnimation()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Move", true);
    }

    //确保敌人始终面向玩家
    private void FacePlayer()
    {
        //如果玩家在敌人的左侧，敌人转向左侧
        if (player.transform.position.x < enemy.transform.position.x)
        {
            if (enemy.facingDirection != -1)
            {
                enemy.Flip();
            }
        }

        //如果玩家在敌人的右侧，敌人转向右侧
        if (player.transform.position.x > enemy.transform.position.x)
        {
            if (enemy.facingDirection != 1)
            {
                enemy.Flip();
            }
        }
    }
}
