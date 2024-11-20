using UnityEngine;

public class DeathBringer : Enemy
{
    [Header("弓箭手")]
    [SerializeField] private BoxCollider2D teleportRegion;  //传送区域的碰撞框
    [SerializeField] private Vector2 surroundingCheckSize;  //检查周围环境的大小
    public float defaultChanceToTeleport;  //默认的传送概率
    public float chanceToTeleport { get; set; }  //传送概率

    [Header("法术设置")]
    [SerializeField] private GameObject spellPrefab;  //法术预设体
    [SerializeField] private float spellCastStateCooldown;  //法术施放状态的冷却时间
    public float lastTimeEnterSpellCastState { get; set; }  //上次进入施法状态的时间
    public int castAmount;  //法术的施放次数
    public float castCooldown;  //法术施放的冷却时间

    [Header("boss战斗状态")]
    [SerializeField] private GameObject bossNameAndHPUI;  //显示Boss名字和血量的UI
    public int stage { get; set; } = 1;  //Boss阶段，默认为阶段1

    #region States
    public DeathBringerIdleState idleState { get; private set; }  //空闲状态
    public DeathBringerMoveState moveState { get; private set; }  //移动状态
    public DeathBringerBattleState battleState { get; private set; }  //战斗状态
    public DeathBringerAttackState attackState { get; private set; }  //攻击状态
    public DeathBringerTeleportState teleportState { get; private set; }  //传送状态
    public DeathBringerCastState castState { get; private set; }  //施法状态
    public DeathBringerStunnedState stunnedState { get; private set; }  //眩晕状态
    public DeathBringerDeathState deathState { get; private set; }  //死亡状态
    #endregion

    //初始化状态和设定
    protected override void Awake()
    {
        base.Awake();

        //创建状态机中的各个状态
        idleState = new DeathBringerIdleState(this, stateMachine, "Idle", this);
        moveState = new DeathBringerMoveState(this, stateMachine, "Move", this);
        battleState = new DeathBringerBattleState(this, stateMachine, "Move", this);
        attackState = new DeathBringerAttackState(this, stateMachine, "Attack", this);
        teleportState = new DeathBringerTeleportState(this, stateMachine, "Teleport", this);
        castState = new DeathBringerCastState(this, stateMachine, "Cast", this);
        stunnedState = new DeathBringerStunnedState(this, stateMachine, "Idle", this);
        deathState = new DeathBringerDeathState(this, stateMachine, "Idle", this);

        SetupDefaultFacingDirection(-1);  //设置默认的朝向方向（-1 表示向左）
    }

    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();  //初始化上次的攻击时间
        chanceToTeleport = defaultChanceToTeleport;  //设置传送的概率
        stage = 1;  //初始化为阶段1

        stateMachine.Initialize(idleState);  //初始化为空闲状态
    }

    protected override void Update()
    {
        base.Update();

        //如果Boss的当前HP低于60%，进入阶段2
        if (stats.currentHP <= stats.getMaxHP() * 0.6f)
        {
            stage = 2;  //进入阶段2
        }

        //如果Boss的攻击被中断，关闭反击窗口
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }
    }

    //判断是否可以被反击并进入眩晕状态
    public override bool CanBeStunnedByCounterAttack()
    {
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState);  //切换到眩晕状态
            return true;
        }

        return false;
    }

    //Boss死亡时的处理
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);  //切换到死亡状态

        //根据语言设置显示不同的感谢页面
        if (LanguageManager.instance.localeID == 0)  //英文
        {
            UI.instance.SwitchToThankYouForPlaying("Achieved ending - Breaking the 4th wall");
        }
        else if (LanguageManager.instance.localeID == 1)  //中文
        {
            UI.instance.SwitchToThankYouForPlaying("达成结局 — 打破第四面墙");
        }
    }

    //进入战斗状态，防止中断攻击或施法
    public override void GetIntoBattleState()
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState || stateMachine.currentState == castState)
        {
            return;  //当前在战斗或施法状态时，不能进入战斗状态
        }

        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);  //切换到战斗状态
        }
    }

    //初始化上次的攻击时间
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //绘制下方的检测线
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - HasGroundBelow().distance));
        //绘制周围环境的检查区域
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    //检查是否有地面
    private RaycastHit2D HasGroundBelow()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);  //检测下方是否有地面
    }

    //检查周围是否有障碍物
    private RaycastHit2D HasSomethingSurrounded()
    {
        return Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);  //检查周围是否有地面
    }

    //随机选择一个传送位置
    public void FindTeleportPosition()
    {
        float x = Random.Range(teleportRegion.bounds.min.x + 3, teleportRegion.bounds.max.x - 3);  //随机选择X轴位置
        float y = Random.Range(teleportRegion.bounds.min.y + 3, teleportRegion.bounds.max.y - 3);  //随机选择Y轴位置

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - HasGroundBelow().distance + (cd.size.y / 2));  // 调整位置使其位于地面上

        // 如果该位置没有地面或者有障碍物，则需要重新选择位置
        if (!HasGroundBelow() || HasSomethingSurrounded())
        {
            Debug.Log("Need to find new teleport position");
            FindTeleportPosition();
        }
    }

    // 判断是否可以进行传送
    public bool CanTeleport()
    {
        if (stage == 1)
        {
            return false;  // 阶段1时不能传送
        }

        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            return true;  // 根据传送概率决定是否传送
        }

        return false;
    }

    // 判断是否可以施放法术
    public bool CanCastSpell()
    {
        if (stage == 1)
        {
            return false;  // 阶段1时不能施放法术
        }

        if (Time.time - lastTimeEnterSpellCastState >= spellCastStateCooldown)
        {
            return true;  // 如果冷却时间过了，则可以施放法术
        }

        return false;
    }

    // 施放法术
    public void CastSpell()
    {
        Vector3 spellSpawnPosition;

        // 如果玩家正在移动，法术会出现在玩家前方
        if (player.rb.velocity.x != 0)
        {
            spellSpawnPosition = new Vector3(player.transform.position.x + player.facingDirection * 3, player.transform.position.y + 1.65f);
        }
        // 如果玩家没有移动，法术会出现在玩家上方
        else
        {
            spellSpawnPosition = new Vector3(player.transform.position.x, player.transform.position.y + 1.65f);
        }

        // 实例化法术，并设置其属性
        GameObject newSpell = Instantiate(spellPrefab, spellSpawnPosition, Quaternion.identity);
        newSpell.GetComponent<SpellController>()?.SetupSpell(stats);
    }

    // 显示Boss的名字和血量
    public void ShowBossHPAndName()
    {
        bossNameAndHPUI.SetActive(true);
    }

    // 关闭Boss的名字和血量UI
    public void CloseBossHPAndName()
    {
        bossNameAndHPUI.SetActive(false);
    }
}
