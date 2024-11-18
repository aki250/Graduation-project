using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 
    }

    public override void Exit()
    {
        base.Exit(); 

        AudioManager.instance.StopSFX(14); //停止音效，编号14
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.moveState)
        {
            return;
        }

        //播放音效，编号14
        AudioManager.instance.PlaySFX(14, player.transform);

        //设置玩家的水平速度，垂直速度保持不变
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        //如果玩家的水平输入为0或者玩家检测到墙，则切换至站立
        if (xInput == 0 || player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}