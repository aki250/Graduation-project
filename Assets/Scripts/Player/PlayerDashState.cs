using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    // 当玩家进入冲刺状态时调用
    public override void Enter()
    {
        base.Enter();

        //在冲刺开始时调用技能 冲刺效果
        player.skill.dash.CloneOnDashStart(player.transform.position);

        //冲刺持续时间
        stateTimer = player.dashDuration;

        //玩家在冲刺期间为无敌状态
        player.stats.BecomeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        //停止玩家的横向速度（保持垂直速度不变）
        player.SetVelocity(0, rb.velocity.y);

        //在冲刺结束时调用技能（如结束冲刺效果）
        player.skill.dash.CloneOnDashEnd(player.transform.position);

        //取消玩家无敌状态
        player.stats.BecomeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        //如果当前状态不再是冲刺状态，取消所有冲刺效果
        if (stateMachine.currentState != player.dashState)
        {
            return;
        }

        //玩家没有接触地面且与墙壁碰撞，转到墙壁滑行状态
        if (!player.IsGroundDetected() && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }

        //设置玩家的横向速度为冲刺速度，并保持垂直速度不变
        player.SetVelocity(player.dashSpeed * player.dashDirection, 0);

        //创建冲刺后的残影效果
        player.fx.CreateAfterimage();

        //冲刺时间结束，切换到空闲状态
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
