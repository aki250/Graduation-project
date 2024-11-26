using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    #region States
    //骷髅敌人状态
    public SkeletonIdleState idleState { get; private set; }    //待机
    public SkeletonMoveState moveState { get; private set; }    //移动
    public SkeletonBattleState battleState { get; private set; }    //战斗
    public SkeletonAttackState attackState { get; private set; }    //攻击
    public SkeletonStunnedState stunnedState { get; private set; }  //眩晕
    public SkeletonDeathState deathState { get; private set; }  //死亡状态
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SkeletonIdleState(this, stateMachine, "Idle", this);
        moveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, stateMachine, "Move", this);
        attackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deathState = new SkeletonDeathState(this, stateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();

        //初始化敌人最后一次攻击的时间信息
        InitializeLastTimeInfo();
        //敌人初始状态为待机状态
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        //防止反击窗口在骨架敌人攻击被打断时总是显示
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }

    }

    // 重写基类方法，判断骨架敌人是否可以被反击打晕
    public override bool CanBeStunnedByCounterAttack()
    {
        // 如果基类可以反击打晕，则进入眩晕状态
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState);  // 切换到眩晕状态
            return true;  // 返回true，表示敌人被打晕
        }

        return false;  // 否则返回false
    }

    //重写死亡，敌人死亡逻辑
    public override void Die()
    {
        base.Die(); 

        stateMachine.ChangeState(deathState);
    }

    //进入战斗状态
    public override void GetIntoBattleState()
    {
        //如果当前已经在战斗状态或攻击状态，或已经死亡，就不再进入战斗状态
        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);  //切换到战斗状态
        }
    }

    //初始化敌人上次攻击的时间，防止逻辑错误
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;  //初始化攻击时间
    }
}
