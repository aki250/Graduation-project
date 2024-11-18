using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                                //玩家在墙面跳跃状态
public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        //状态计时器为玩家的墙面跳跃持续时间
        stateTimer = player.wallJumpDuration;
        //玩家的速度为墙面跳跃的水平速度乘以面向方向的相反数（以实现向左或向右跳跃），垂直速度为跳跃力
        player.SetVelocity(player.wallJumpXSpeed * -player.facingDirection, player.jumpForce);
    }

    // 当退出墙面跳跃状态时调用
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //如果当前状态不是墙面跳跃状态，则不执行剩余的更新函数代码
        if (stateMachine.currentState != player.wallJumpState)
        {
            return;
        }

        //如果状态计时器小于0，即墙面跳跃时间结束
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.airState);
        }

        //检测地面，即玩家落地
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}