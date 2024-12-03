using System.Collections;
using UnityEngine;

public class Player : Entity
{

    public SkillManager skill { get; private set; }  //技能管理器
    public GameObject sword { get; private set; }  //玩家的剑

    [Header("移动")]
    public float moveSpeed;  //玩家移动速度
    public float jumpForce;  //玩家跳跃力度
    public float wallJumpXSpeed;  //墙跳时的X方向速度
    public float wallJumpDuration;  //墙跳持续时间
    private float defaultMoveSpeed;  //默认速度
    private float defaultJumpForce;  //默认跳跃高度

    [Header("攻击")]
    public Vector2[] attackMovement;  //攻击时的移动方式
    public float counterAttackDuration = 0.2f;  //反击持续时间

    [Header("冲刺")]
    public float dashSpeed;  //冲刺速度
    public float dashDuration;  //冲刺持续时间
    public float dashDirection { get; private set; }  //冲刺方向
    private float defaultDashSpeed;  //默认冲刺速度

    [Header("洞坑检查")]
    [SerializeField] private BoxCollider2D pitCheck;  //检测坑洞
    [SerializeField] private BoxCollider2D downablePlatformCheck;  //检测可下降的平台

    public bool isNearPit { get; set; }  //是否接近坑洞
    public DownablePlatform lastPlatform { get; set; }  //上一个可下降平台
    public bool isOnPlatform { get; set; } = false;  //玩家是否站在平台上

    public bool isBusy { get; private set; }  //忙碌状态
    public PlayerFX fx { get; private set; }  //特效组件

    #region States and Statemachine
    //玩家状态机和各种状态
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerAirLaunchAttackState airLaunchAttackState { get; private set; }
    public PlayerDownStrikeState downStrikeState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerThrowSwordState throwSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerReleaseBlackholeSkillState blackholeSkillState { get; private set; }
    public PlayerDeathState deathState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();
        fx = GetComponent<PlayerFX>();

        //初始化各个状态
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        airLaunchAttackState = new PlayerAirLaunchAttackState(this, stateMachine, "AirLaunchAttack");
        downStrikeState = new PlayerDownStrikeState(this, stateMachine, "DownStrike");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSwordState = new PlayerAimSwordState(this, stateMachine, "AimSword");
        throwSwordState = new PlayerThrowSwordState(this, stateMachine, "ThrowSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackholeSkillState = new PlayerReleaseBlackholeSkillState(this, stateMachine, "Jump");
        deathState = new PlayerDeathState(this, stateMachine, "Death");
    }

    //初始化时设定默认值
    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        base.Update();

        stateMachine.currentState.Update();

        if (stats.isDead)
        {
            return;
        }

        CheckForDashInput();  //检查冲刺输入

        //按下特定键时使用技能
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Crystal"]) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.UseSkillIfAvailable();
        }

        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Flask"]))
        {
            Inventory.instance.UseFlask_ConsiderCooldown(null);
        }
    }

    //检查是否按下冲刺键
    private void CheckForDashInput()
    {
        if (skill.dash.dashUnlocked == false) return;  //冲刺技能未解锁

        if (IsWallDetected()) return;  //检测到墙壁，不进行冲刺

        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Dash"]) && SkillManager.instance.dash.UseSkillIfAvailable())
        {
            //如果玩家在瞄准剑或投掷剑状态，先隐藏剑的瞄准点
            if (stateMachine.currentState == aimSwordState || stateMachine.currentState == throwSwordState)
            {
                skill.sword.ShowDots(false);
            }

            dashDirection = Input.GetAxisRaw("Horizontal");

            if (dashDirection == 0)
            {
                dashDirection = facingDirection;
            }

            stateMachine.ChangeState(dashState);  //切换到冲刺状态
        }
    }

    //动画完成后调用
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    //触发空中发射攻击
    public void AirLaunchJumpTrigger()
    {
        airLaunchAttackState.SetAirLaunchJumpTrigger();
    }

    //触发下击攻击
    public void DownStrikeTrigger()
    {
        downStrikeState.SetFallingStrikeTrigger();
    }

    //停止下击动画
    public void DownStrikeAnimStopTrigger()
    {
        downStrikeState.SetAnimStopTrigger();
    }

    //玩家忙碌时，无法进行其他操作
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    //给玩家分配新剑
    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    //捡起剑并切换到相应状态
    public void CatchSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    //检查玩家是否没有剑
    public bool HasNoSword()
    {
        if (!sword) return true;

        sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }

    //玩家死亡时，切换到死亡状态
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);
    }

    //让玩家在一定时间内减速
    public override void SlowSpeedBy(float _percentage, float _duration)
    {
        moveSpeed *= (1 - _percentage);
        jumpForce *= (1 - _percentage);
        dashSpeed *= (1 - _percentage);
        anim.speed *= (1 - _percentage);

        Invoke("ReturnDefaultSpeed", _duration);  //恢复默认速度
    }

    //恢复默认的速度
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    //受伤时的闪烁效果
    public override void DamageFlashEffect()
    {
        fx.StartCoroutine("FlashFX");
    }

    //从平台跳下
    public void JumpOffPlatform()
    {
        if (isOnPlatform)
        {
            lastPlatform.TurnOffPlatformColliderForTime(0.5f);
        }
    }
}
