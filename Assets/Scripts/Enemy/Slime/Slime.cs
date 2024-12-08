using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//三种史莱姆类型枚举
public enum SlimeType
{
    big,    
    medium, 
    small   
}

//史莱姆类继承自敌人 类
public class Slime : Enemy
{
    [Header("史莱姆")]
    [SerializeField] private SlimeType slimeType; 
    [SerializeField] private int amoutOfSlimeToSpawnAfterDeath; //死亡后生成的史莱姆数量
    [SerializeField] private GameObject slimePrefab;    //史莱姆预制体
    [SerializeField] private Vector2 minSlimeSpawnSpeed;    //生成史莱姆时最小速度
    [SerializeField] private Vector2 maxSlimeSpawnSpeed;    //生成史莱姆时最大速度

    #region States
    //史莱姆各个状态
    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeathState deathState { get; private set; }
    #endregion

    //初始化各个状态
    protected override void Awake()
    {
        base.Awake();

        //实例化状态对象
        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);
        deathState = new SlimeDeathState(this, stateMachine, "Idle", this);

        SetupDefaultFacingDirection(-1); //默认面朝左
    }

    //设置初始状态
    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();   //记录最后一次攻击时间，防止出现多重攻击或者一些别的bug
        stateMachine.Initialize(idleState); //初始状态为站立
    }

    protected override void Update()
    {
        base.Update();

        //如果当前状态不是攻击状态，关闭反击窗口
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }
    }

    //判断是否能被反击并进入眩晕状态
    public override bool CanBeStunnedByCounterAttack()
    {
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState); //切换到眩晕状态
            return true;
        }

        return false;
    }

    //史莱姆死亡时调用
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState); //切换到死亡状态

        //如果是小史莱姆，则不生成新史莱姆
        if (slimeType == SlimeType.small)
        {
            return;
        }

        //否则，根据设置的数量生成新的史莱姆
        SpawnSlime(amoutOfSlimeToSpawnAfterDeath, slimePrefab);
    }

    //进入战斗状态
    public override void GetIntoBattleState()
    {
        //如果当前是大史莱姆，且当前状态是战斗或攻击状态，则不允许进入战斗状态
        if (slimeType == SlimeType.big && (stateMachine.currentState == battleState || stateMachine.currentState == attackState))
        {
            return;
        }

        //如果当前不是战斗状态、眩晕状态或死亡状态，切换到战斗状态
        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState); //进入战斗状态
        }
    }

    //生成指定数量的史莱姆
    private void SpawnSlime(int _amountOfSlimeToSpawn, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlimeToSpawn; i++)
        {
            //实例化新的史莱姆
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);

            //设置新生成的史莱姆的方向
            newSlime.GetComponent<Slime>()?.SetupSpawnedSlime(facingDirection);
        }
    }

    // 设置生成的史莱姆的移动速度和方向
    public void SetupSpawnedSlime(int _facingDirection)
    {
        // 保证生成的史莱姆与父对象的方向一致
        if (facingDirection != _facingDirection)
        {
            Flip(); // 翻转史莱姆
        }

        // 随机生成史莱姆的速度
        float xVelocity = Random.Range(minSlimeSpawnSpeed.x, maxSlimeSpawnSpeed.x);
        float yVelocity = Random.Range(minSlimeSpawnSpeed.y, maxSlimeSpawnSpeed.y);

        // 使史莱姆不能在生成时被击飞
        isKnockbacked = true;

        // 设置史莱姆的速度，使其朝指定方向移动
        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDirection, yVelocity);

        // 防止史莱姆生成时的速度被打断
        Invoke("CancelKnockback", 1.5f);
    }

    // 取消史莱姆的击飞状态
    private void CancelKnockback()
    {
        isKnockbacked = false;
    }

    //初始化记录最后一次攻击的时间
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;
    }
}
