using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    //组合计数器，连击次数
    public int comboCounter { get; private set; }

    private float lastTimeAttacked; //记录最后一次攻击时间，用于与连击窗口时间配合

    private float comboWindow = 0.4f;   //连击窗口时间，即玩家释放下一个连击攻击的输入时间窗口

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        AudioManager.instance.PlaySFX(2, player.transform);

        if (stateMachine.currentState != player.primaryAttackState)
        {
            return;
        }

        //如果组合计数器大于2或者当前时间大于最后一次攻击时间加上连击窗口时间，则重置组合计数器（就连击时间嘛，就不上连击就重置
        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        //设置动画中的组合计数器参数
        player.anim.SetInteger("ComboCounter", comboCounter);

        //攻击方向与玩家面向方向一致
        float attackDirection = player.facingDirection;

        //重新赋值xInput以防止xInput的值没有及时更新导致的错误
        xInput = Input.GetAxisRaw("Horizontal");

        //xInput不为0，则攻击方向与xInput一致
        if (xInput != 0)
        {
            attackDirection = xInput;
        }

        //设置玩家的速度为攻击移动数组中对应组合计数器的值
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDirection, player.attackMovement[comboCounter].y);

        //设置状态计时器为0.1f，以在移动状态中保持一定的惯性
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        //用协程使玩家忙碌0.05秒
        player.StartCoroutine(player.BusyFor(0.05f));

        //组合计数器增加
        comboCounter++;
        //更新最后一次攻击时间为当前时间
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //如果状态计时器小于0，则将玩家速度设置为零
        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        //如果触发器被调用
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}