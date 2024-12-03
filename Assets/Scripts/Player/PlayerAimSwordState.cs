using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        // 玩家如果在进入此状态时是奔跑状态，会稍微滑动一段距离
        stateTimer = 0.1f; //定时器0.1秒

        //显示瞄准点
        player.skill.sword.ShowDots(true);
    }

    public override void Exit()
    {
        base.Exit();

        //玩家在投掷剑后不能立即移动，开始协程使玩家在0.1秒内处于忙碌状态
        player.StartCoroutine(player.BusyFor(0.1f));
    }

    public override void Update()
    {
        base.Update(); 

        if (stateMachine.currentState != player.aimSwordState)
        {
            return;
        }

        if (stateTimer < 0)
        {
            player.SetZeroVelocity(); //停止玩家的移动速度
        }

        //获取鼠标位置并将其转换为世界坐标
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //根据鼠标位置来翻转玩家的朝向，确保玩家朝向正确
        if (mousePosition.x < player.transform.position.x && player.facingDirection == 1)
        {
            player.Flip();
        }
        else if (mousePosition.x > player.transform.position.x && player.facingDirection == -1)
        {
            player.Flip();
        }

        //如果玩家松开右键（瞄准键），则退出AimSword状态
        if (Input.GetKeyUp(KeyBindManager.instance.keybindsDictionary["Aim"]))
        {
            //退出AimSword状态时，不再显示瞄准点
            player.skill.sword.ShowDots(false);

            stateMachine.ChangeState(player.idleState);
        }

        //如果玩家按下左键（攻击键），切换到投掷剑的状态
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            //在ThrowSword动画中会调用ThrowSword()方法，进而调用CreateSword()方法，最终会调用ShowDots(false)来隐藏瞄准点
            stateMachine.ChangeState(player.throwSwordState);
        }
    }
}
