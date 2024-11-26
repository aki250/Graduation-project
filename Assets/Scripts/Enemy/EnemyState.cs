using UnityEngine;
                                                    //敌人各类状态
public class EnemyState
{
    protected EnemyStateMachine stateMachine;          //状态机管理各种状态
    protected Enemy enemyBase;                          //敌人基础类实例，包含敌人共有属性，方法

    protected Rigidbody2D rb; //刚体组件
    protected Animator anim; //动画器组件

    private string animBoolName; //动画播放

    protected float stateTimer; //状态计时器，用于控制状态的持续时间
    protected bool triggerCalled; //触发器标志，用于标记动画触发器是否已被调用


    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        enemyBase = _enemyBase; //基础类实例
        stateMachine = _stateMachine; //状态机
        animBoolName = _animBoolName; //动画布尔参数
    }

    public virtual void Enter()
    {
        triggerCalled = false; //初始化触发器标志为false

        rb = enemyBase.rb; //获取敌人刚体组件
        anim = enemyBase.anim; //获取敌人的动画器组件

        anim.SetBool(animBoolName, true); //true，播放对应动画
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false); //停止动画

        enemyBase.AssignLastAnimBoolName(animBoolName); //将当前动画布尔参数名称保存为最后一次使用的名称
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime; //减少状态计时器
    }

    // 动画触发器调用的方法
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true; //标记触发器已被调用
    }
}
