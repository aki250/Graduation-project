using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        AudioManager.instance.StopSFX(14);
    }

    public override void Exit()
    {
        base.Exit(); 
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.idleState)
        {
            return;
        }

        player.SetVelocity(0, rb.velocity.y); //设置玩家的水平速度为0，保持垂直速度不变

        //如果玩家输入的水平移动值不0且玩家不在忙碌状态（例如，攻击后的硬直）
        if (xInput != 0 && !player.isBusy)
        {
            //如果玩家不是在检测到墙的情况下向墙移动（即玩家不是面向墙移动）
            if (!(player.IsWallDetected() && xInput == player.facingDirection))
            {
                stateMachine.ChangeState(player.moveState); // 切换到移动状态
            }
        }
    }
}