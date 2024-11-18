using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                            //玩家的反击状态
public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;    //标记玩家是否可以在当前反击中创建克隆体

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        stateTimer = player.counterAttackDuration;  //设置状态计时器为玩家的反击持续时间
        player.anim.SetBool("SuccessfulCounterAttack", false);  //初始化动画为false

        canCreateClone = true;  //确保每次进入反击状态时，玩家都能创建克隆体
    }

 
    public override void Exit()
    {
        base.Exit(); 
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != this)
        {
            return;     //如果当前状态不是反击状态，则直接返回
        }

        player.SetZeroVelocity();   //设置玩家速度为零

        //检测玩家攻击范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            //如果检测到的是箭矢控制器，则翻转箭矢
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
                SuccessfulCounterAttack(); //调用反击方法
            }

            //如果检测到的是敌人
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();

                //如果敌人可以被反击击晕
                if (enemy.CanBeStunnedByCounterAttack())
                {
                    SuccessfulCounterAttack(); //调用反击方法

                    //反击恢复生命值/法力值
                    player.skill.parry.RecoverHPFPInSuccessfulParry();

                    //如果可以创建克隆体
                    if (canCreateClone)
                    {
                        //在敌人后方创建克隆体并攻击敌人
                        player.skill.parry.MakeMirageInSuccessfulParry(new Vector3(enemy.transform.position.x - 1.5f * enemy.facingDirection, enemy.transform.position.y));
                        canCreateClone = false;  //每次反击只能创建一个克隆体
                    }
                }
            }
        }

        //如果状态计时器小于0或触发了触发器，则切换到空闲状态
        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    //成功反击时的方法
    private void SuccessfulCounterAttack()
    {
        //设置状态计时器为一个大值，因为如果成功反击，状态将通过 triggerCalled 退出
        stateTimer = 10;

        player.anim.SetBool("SuccessfulCounterAttack", true); //设置动画为 true
        player.fx.ScreenShake(player.fx.shakeDirection_medium); //屏幕震动效果
    }
}
