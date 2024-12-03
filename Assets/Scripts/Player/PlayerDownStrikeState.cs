using UnityEngine;

//玩家下击状态类，负责处理玩家下击时的状态逻辑
public class PlayerDownStrikeState : PlayerState
{
    //判断触发下击下落阶段
    public bool fallingStrikeTrigger { get; private set; } = false;

    //判断触发动画阶段
    public bool animStopTrigger { get; private set; } = false;

    //判断设置停止动画
    private bool animStopTriggerHasBeenSet = false;

    //判断设置屏幕震动
    private bool screenShakeHasBeenSet = false;

    //初始化下击状态
    public PlayerDownStrikeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //设置玩家的初始速度，模拟下落过程
        player.SetVelocity(0, -10);

        //重置下击状态相关的触发器
        fallingStrikeTrigger = false;
        animStopTrigger = false;
        animStopTriggerHasBeenSet = false;
        screenShakeHasBeenSet = false;

        // 设置状态定时器为无限大，表示这个状态不会自动结束，直到手动触发
        stateTimer = Mathf.Infinity;
    }

    public override void Exit()
    {
        base.Exit(); 

        // 恢复动画播放速度
        player.anim.speed = 1;
    }

    public override void Update()
    {
        base.Update(); 

        //当前状态不是下击状态，直接返回
        if (stateMachine.currentState != this)
        {
            return;
        }

        //玩家先会稍微上升，作为下击的蓄力
        if (!fallingStrikeTrigger)
        {
            player.SetVelocity(0, 1.2f);  //玩家上升高度
        }
        else
        {
            //玩家开始进行下击攻击，速度向下
            player.SetVelocity(0, -17);

            //当剑完全伸出并准备攻击敌人时，停止动画
            if (animStopTrigger && !animStopTriggerHasBeenSet)
            {
                player.anim.speed = 0;
                animStopTriggerHasBeenSet = true;  //设置已停止动画标志
            }

            //当玩家检测到地面时，恢复动画播放
            if (player.IsGroundDetected())
            {
                player.anim.speed = 1;  // 恢复动画播放

                //如果屏幕震动还未设置，则执行屏幕震动和尘土特效
                if (!screenShakeHasBeenSet)
                {
                    player.fx.ScreenShake(player.fx.shakeDirection_medium);  // 播放屏幕震动特效
                    player.fx.PlayDownStrikeDustFX();  // 播放下击尘土特效
                    screenShakeHasBeenSet = true;  // 设置震动特效已执行
                }
            }

            //结束条件则切换到站立
            if (triggerCalled)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    //触发下落攻击阶段
    public void SetFallingStrikeTrigger()
    {
        fallingStrikeTrigger = true;
    }

    //触发动画停止阶段
    public void SetAnimStopTrigger()
    {
        animStopTrigger = true;
    }
}
