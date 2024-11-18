using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{

    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    // 当进入跳跃状态时调用
    public override void Enter()
    {
        base.Enter(); 

        // 设置玩家的水平速度为当前速度，垂直速度为跳跃力
        player.SetVelocity(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.jumpState)
        {
            return;
        }

        // 如果玩家的垂直速度小于0，即玩家开始下落
        if (rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}