using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirLaunchAttackState : PlayerState
{
    //标记是否触发了空中发起攻击
    public bool airLaunchJumpTrigger { get; private set; } = false;

    //记录玩家是否已经跳过
    private bool hasJumped = false;

    //初始化玩家和状态机
    public PlayerAirLaunchAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //重置空中发起攻击标志
        airLaunchJumpTrigger = false;
        //重置跳跃状态
        hasJumped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //如果当前状态不是空中发起攻击状态，则退出操作
        if (player.stateMachine.currentState != this)
        {
            return;
        }

        //如果触发了空中发起攻击且尚未执行跳跃
        if (airLaunchJumpTrigger && !hasJumped)
        {
            //设置玩家的跳跃速度，垂直方向上给予一个初始速度
            player.SetVelocity(0, 17); // 假设17是跳跃的垂直速度
            //标记为已跳跃
            hasJumped = true;
        }

        //改变状态机为空中状态
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    //用于触发空中发起攻击
    public void SetAirLaunchJumpTrigger()
    {
        airLaunchJumpTrigger = true; //设置空中发起攻击标志为真
    }
}
