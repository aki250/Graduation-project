using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword; //存储剑的位置，用于判断位置和旋转

    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform; //获取玩家手中的剑的位置

        player.fx.PlayDustFX(); //播放尘土特效
        player.fx.ScreenShake(player.fx.shakeDirection_medium); //播放屏幕震动特效，给定震动方向

        //根据剑的位置来翻转玩家的朝向
        if (sword.position.x < player.transform.position.x && player.facingDirection == 1)
        {
            player.Flip(); //如果剑在玩家左边并且玩家面朝右，则翻转玩家朝向
        }
        else if (sword.position.x > player.transform.position.x && player.facingDirection == -1)
        {
            player.Flip(); //如果剑在玩家右边并且玩家面朝左，则翻转玩家朝向
        }

        stateTimer = 0.1f; //设置状态计时器，控制滑退的持续时间
        rb.velocity = new Vector2(player.moveSpeed * -player.facingDirection, rb.velocity.y); // 设置玩家的滑退速度
    }

    public override void Exit()
    {
        base.Exit(); 

        player.StartCoroutine(player.BusyFor(0.1f)); //让玩家稍微停顿一段时间，防止立即做其他动作
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.catchSwordState)
        {
            return; //如果当前状态不是catchSwordState，则跳过后续操作
        }

        //如果状态计时器小于0，表示滑退动作结束，停止滑动
        if (stateTimer < 0)
        {
            player.SetVelocity(0, rb.velocity.y); //设置玩家水平速度为0，垂直速度不变
        }

        //如果触发条件被满足（例如捕捉完剑后），切换回空闲状态
        if (triggerCalled)
        {
            player.stateMachine.ChangeState(player.idleState); //切换到空闲状态
        }
    }
}
