using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;  //状态机，用于切换不同的状态
    protected Player player;    //获取父物体Player组件
    protected Rigidbody2D rb;   //玩家物理刚体

    //动画相关的字段
    private string animBoolName;    //动画参数名称，用于控制动画状态
    protected float xInput;     //水平输入
    protected float yInput;    //垂直输入

    // 状态计时器，控制状态持续的时间
    protected float stateTimer;  //状态持续时间的计时器
    protected bool triggerCalled;   //动画触发器的状态，用于检测动画是否已经完成

    //使用构造函数，初始化状态对象
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        player = _player;   //将传入的玩家对象赋值给player
        stateMachine = _stateMachine;   //将传入的状态机对象赋值给stateMachine
        animBoolName = _animBoolName;   //将传入的动画参数名赋值给animBoolName
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);    //播放对应的动画

        rb = player.rb;
        triggerCalled = false;  //初始化触发器状态为false，表示动画尚未完成
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;    //每帧减少状态计时器

        xInput = Input.GetAxisRaw("Horizontal");  //获取水平
        yInput = Input.GetAxisRaw("Vertical");    //获取垂直

        //更新控制动画中的y轴速度
        player.anim.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);   //停止动画
    }

    //动画完成触发器
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true; //将触发器标记为已调用，动画结束后切换状态
    }
}
