using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        UI.instance.SwitchToEndScreen();    //切换到游戏结束屏幕
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.deathState)
        {
            return;
        }

        player.SetZeroVelocity(); // 设置玩家速度为零，防止玩家在死亡状态下移动
    }
}