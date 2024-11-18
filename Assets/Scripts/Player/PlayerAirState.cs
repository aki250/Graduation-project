using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    //初始化Player和StateMachine
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        //确保状态机当前状态是空中状态
        if (stateMachine.currentState != player.airState)
        {
            return; //如果不是空中状态，则直接返回
        }

        //玩家有横向输入时，更新玩家的水平速度
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
        }

        //检测玩家是否碰到墙壁并且不在平台上
        if (player.IsWallDetected() && !player.isOnPlatform)
        {
            //如果是墙壁滑行状态，切换到墙壁滑行状态
            stateMachine.ChangeState(player.wallSlideState);
        }

        //如果按下S键并且攻击按钮被按下，切换到下击状态
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            stateMachine.ChangeState(player.downStrikeState);
        }

        if (player.IsGroundDetected())
        {
            //修复玩家从空中落地时的卡顿问题，保持水平输入
            xInput = Input.GetAxisRaw("Horizontal");

            if (xInput != 0)
            {
                //如果有水平输入，切换到移动状态
                stateMachine.ChangeState(player.moveState);
            }
            else
            {
                //如果没有水平输入，切换到静止状态
                stateMachine.ChangeState(player.idleState);
            }
        }
    }
}
