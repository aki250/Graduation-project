using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                        //玩家贴墙滑行状态
public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.wallSlideState)
        {
            return;
        }

        //如果玩家不再检测到墙壁
        if (!player.IsWallDetected())
        {
            //切换到空中状态
            stateMachine.ChangeState(player.airState);
        }

        //如果玩家按下下方向键，加速滑行速度
        if (yInput < 0)
        {
            player.SetVelocity(0, rb.velocity.y); //保持当前垂直速度
        }
        else
        {
            player.SetVelocity(0, rb.velocity.y * 0.2f); //减少垂直速度以模拟滑行
        }

        //如果玩家按下跳跃键（Space）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //切换到墙面跳跃状态
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        //如果玩家的水平输入不为零且与面向方向不一致
        if (xInput != 0 && player.facingDirection != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }

        //如果玩家检测到地面
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}