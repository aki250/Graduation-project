using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    [Header("弓箭手")]
    [SerializeField] private GameObject arrowPrefab;  //弓箭手射出的箭矢预设体
    public Vector2 jumpVelocity;  //弓箭手跳跃的初始速度
    public float jumpCooldown;  //跳跃的冷却时间
    public float jumpJudgeDistance; //玩家离弓箭手的距离，当玩家距离足够近时，弓箭手会选择跳跃远离
    public float lastTimeJumped { get; set; }  //上次跳跃的时间
    [SerializeField] private float arrowFlySpeed;  //箭矢飞行的速度

    [Header("坑洞检查")]
    [SerializeField] private Transform groundBehindCheck;  //用于检查弓箭手身后是否有地面
    [SerializeField] private Vector2 groundBehindCheckSize;  // 检查区域的大小

    #region 状态
    public ArcherIdleState idleState { get; private set; }  //空闲状态
    public ArcherMoveState moveState { get; private set; }  //移动状态
    public ArcherBattleState battleState { get; private set; }  //战斗状态
    public ArcherAttackState attackState { get; private set; }  //攻击状态
    public ArcherJumpState jumpState { get; private set; }  //跳跃状态
    public ArcherStunnedState stunnedState { get; private set; }  //眩晕状态
    public ArcherDeathState deathState { get; private set; }  //死亡状态
    #endregion

    //初始化状态和设置
    protected override void Awake()
    {
        base.Awake();

        //创建状态机中的各个状态
        idleState = new ArcherIdleState(this, stateMachine, "Idle", this);
        moveState = new ArcherMoveState(this, stateMachine, "Move", this);
        battleState = new ArcherBattleState(this, stateMachine, "Idle", this);
        attackState = new ArcherAttackState(this, stateMachine, "Attack", this);
        jumpState = new ArcherJumpState(this, stateMachine, "Jump", this);
        stunnedState = new ArcherStunnedState(this, stateMachine, "Stunned", this);
        deathState = new ArcherDeathState(this, stateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();
        stateMachine.Initialize(idleState);  //初始化为空闲状态
    }

    protected override void Update()
    {
        base.Update();

        //如果弓箭手的攻击状态没有被打断，关闭反击窗口
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }
    }

    //判断是否可以通过反击将敌人眩晕
    public override bool CanBeStunnedByCounterAttack()
    {
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState);  //切换到眩晕状态
            return true;
        }

        return false;
    }

    //弓箭手死亡时的处理
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);  //切换到死亡状态
    }

    //进入战斗状态，除非当前状态已经是战斗状态或其他特殊状态
    public override void GetIntoBattleState()
    {
        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);  //切换到战斗状态
        }
    }

    //弓箭手的特殊攻击是射箭
    public override void SpecialAttackTrigger()
    {
        //实例化一个新的箭矢
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, Quaternion.identity);

        //计算箭矢的飞行方向，指向玩家
        Vector2 flyDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        Vector2 finalFlySpeed = new Vector2(flyDirection.normalized.x * arrowFlySpeed, flyDirection.normalized.y * arrowFlySpeed);

        //设置箭矢的飞行速度和属性
        newArrow.GetComponent<Arrow_Controller>()?.SetupArrow(finalFlySpeed, stats);
    }

    //初始化跳跃和攻击的时间信息
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;
        lastTimeJumped = 0;  //初始化跳跃时间
    }

    //检查弓箭手身后是否有地面
    public bool GroundBehindCheck()
    {
        return Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    //检查弓箭手身后是否有墙壁
    public bool WallBehindCheck()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDirection, wallCheckDistance + 2, whatIsGround);
    }

    //Gizmos绘制
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);  //绘制检测区域
    }

    //检测玩家是否在范围内
    public override RaycastHit2D IsPlayerDetected()
    {
        return Physics2D.CircleCast(wallCheck.position, playerScanDistance, Vector2.right * facingDirection, 0, whatIsPlayer);
    }
}
