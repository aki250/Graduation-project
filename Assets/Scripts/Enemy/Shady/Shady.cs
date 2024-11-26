using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shady : Enemy
{
    [Header("shady自爆效果")]
    [SerializeField] private GameObject explosionPrefab;  //爆炸效果预设体
    [SerializeField] private float explosionMaxSize;     //爆炸最大尺寸
    [SerializeField] private float explosionGrowSpeed;   //爆炸增长速度

    #region States
    //Shady类的不同状态
    public ShadyIdleState idleState { get; private set; } 
    public ShadyMoveState moveState { get; private set; } 
    public ShadyBattleState battleState { get; private set; } 
    public ShadyAttackState attackState { get; private set; } 
    public ShadyExplosionState explosionState { get; private set; } 

    // public ShadyStunnedState stunnedState { get; private set; }
    public ShadyDeathState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        // **初始化各个状态**：每个状态都通过状态机进行管理
        idleState = new ShadyIdleState(this, stateMachine, "Idle", this);  // 闲置状态
        moveState = new ShadyMoveState(this, stateMachine, "Move", this);  // 移动状态
        battleState = new ShadyBattleState(this, stateMachine, "BattleMove", this);  // 战斗状态
        attackState = new ShadyAttackState(this, stateMachine, "Attack", this);  // 攻击状态
        explosionState = new ShadyExplosionState(this, stateMachine, "Explosion", this);  // 爆炸状态
        // stunnedState = new ShadyStunnedState(this, stateMachine, "Stunned", this);  // 被击晕状态 (注释掉了)
        deathState = new ShadyDeathState(this, stateMachine, "Death", this);  // 死亡状态
    }

    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();  //初始化上次攻击信息，
        stateMachine.Initialize(idleState);  //初始为闲置状态
    }

    protected override void Update()
    {
        base.Update();

        //防止攻击被中断时，始终显示反击图像
        // if (stateMachine.currentState != attackState)
        // {
        //     CloseCounterAttackWindow();
        // }
    }

    // 反击是否能够击晕敌人
    // public override bool CanBeStunnedByCounterAttack()
    // {
    //     if (base.CanBeStunnedByCounterAttack())
    //     {
    //         stateMachine.ChangeState(stunnedState);
    //         return true;
    //     }
    //     return false;
    // }

    public override void Die()
    {
        base.Die(); 

        stateMachine.ChangeState(deathState); 
    }

    public override void GetIntoBattleState()
    {
        if (stateMachine.currentState != battleState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);  //切换到战斗状态
        }
    }

    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;  //初始化上次攻击时间
    }

    //自爆
    public override void SpecialAttackTrigger()
    {
        //实例化爆炸效果
        GameObject newExplosion = Instantiate(explosionPrefab, attackCheck.position, Quaternion.identity);

        //爆炸的成长速度和最大尺寸
        newExplosion.GetComponent<ShadyExplosion_Controller>()?.SetupExplosion(stats, explosionGrowSpeed, explosionMaxSize, attackCheckRadius);

        cd.enabled = false;  //关闭冷却时间
        rb.gravityScale = 0;  //停止重力作用

        //自爆，丢掉物品和货币
        EnemyStats myStats = stats as EnemyStats;
        myStats.ZeroHP(); 
        myStats.DropCurrencyAndItem();  //爆物品
    }

    public void SelfDestroy()
    {
        Destroy(gameObject); 
    }
}
