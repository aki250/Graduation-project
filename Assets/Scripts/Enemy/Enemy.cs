using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(CapsuleCollider2D))] 
[RequireComponent(typeof(EnemyStats))] 
[RequireComponent(typeof(EntityFX))] 
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [Header("移动")] 
    public float patrolMoveSpeed;  //巡逻速度
    public float patrolStayTime;  //巡逻时间
    private float defaultPatrolMoveSpeed;  //默认巡逻速度

    [Header("侦察范围")] 
    public float playerScanDistance = 10;  //玩家扫描距离
    public float playerHearDistance = 3;  //玩家听觉范围

    [SerializeField] protected LayerMask whatIsPlayer;  //玩家层

    [Header("战斗速度")] 
    public float battleMoveSpeed;  //战斗移动速度
    public float aggressiveTime = 7;  //攻击时间

    private float defaultBattleMoveSpeed;  //默认战斗移动速度

    [Header("攻击")] 
    public float attackDistance = 2;  //攻击距离
    public float attackCooldown = 1.5f;  //攻击冷却时间
    public float minAttackCooldown = 1;  //最小攻击冷却时间
    public float maxAttackCooldown = 2;  //最大攻击冷却时间
    [HideInInspector] public float lastTimeAttacked;  //上次攻击时间

    [Header("眩晕设定")]
    protected bool canBeStunned;  //是否能被眩晕
    public float stunDuration = 1;  //眩晕持续时间
    public Vector2 stunMovement = new Vector2(3, 3);  //眩晕时的移动量
    [SerializeField] protected GameObject counterPromptImage;  //反击提示图片

    public EnemyStateMachine stateMachine { get; private set; } 
    protected Player player { get; private set; } 
    public EntityFX fx { get; private set; }

    public string lastAnimBoolName { get; private set; } 

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine(); 
        fx = GetComponent<EntityFX>();  //获取EntityFX组件

        defaultBattleMoveSpeed = battleMoveSpeed;  //默认战斗移动速度
        defaultPatrolMoveSpeed = patrolMoveSpeed;  //默认巡逻移动速度
    }

    protected override void Start()
    {
        base.Start();

        player = PlayerManager.instance.player; 
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;  //设置Gizmos
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDirection, transform.position.y));  // 绘制攻击范围线
    }

    //检测玩家是否被敌人侦测到
    public virtual RaycastHit2D IsPlayerDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, playerScanDistance, whatIsPlayer);
    }

    //动画触发器
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    //特殊攻击触发器
    public virtual void SpecialAttackTrigger()
    {

    }

    //冻结敌人
    public virtual void FreezeEnemy(bool _freeze)
    {
        if (_freeze)
        {
            battleMoveSpeed = 0;  //战斗移动速度为0
            patrolMoveSpeed = 0;  //巡逻移动速度为0
            anim.speed = 0;  //动画速度为0
        }
        else
        {
            battleMoveSpeed = defaultBattleMoveSpeed;  //恢复默认战斗移动速度
            patrolMoveSpeed = defaultPatrolMoveSpeed;  //恢复默认巡逻移动速度
            anim.speed = 1;  //恢复动画速度为1
        }
    }

    //冻结敌人一段时间
    protected virtual IEnumerator FreezeEnemyForTime_Coroutine(float _seconds)
    {
        FreezeEnemy(true);  //冻结敌人

        yield return new WaitForSeconds(_seconds);  //等待指定时间

        FreezeEnemy(false);  //恢复敌人状态
    }

    //冻结敌人一段时间的函数接口
    public virtual void FreezeEnemyForTime(float _seconds)
    {
        StartCoroutine(FreezeEnemyForTime_Coroutine(_seconds));  //启动协程
    }

    #region Counter Attack 反击区域
    //打开反击窗口
    public void OpenCounterAttackWindow()
    {
        canBeStunned = true;  //设置可以被反击
        counterPromptImage.SetActive(true);  //显示反击提示
    }

    //关闭反击窗口
    public void CloseCounterAttackWindow()
    {
        canBeStunned = false;  //设置不能被反击
        counterPromptImage.SetActive(false);  //隐藏反击提示
    }

    //判断是否可以被反击
    public virtual bool CanBeStunnedByCounterAttack()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();  //关闭反击窗口
            return true;
        }
        return false;
    }
    #endregion

    //设置上次动画状态名称
    public virtual void AssignLastAnimBoolName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;  //赋值
    }

    //慢速处理
    public override void SlowSpeedBy(float _percentage, float _duration)
    {
        patrolMoveSpeed = patrolMoveSpeed * (1 - _percentage);  //降低巡逻速度
        battleMoveSpeed = battleMoveSpeed * (1 - _percentage);  //降低战斗速度
       //anim.speed = anim.speed * (1 - _percentage);  //降低动画速度

        Invoke("ReturnDefaultSpeed", _duration);  //恢复速度的延时调用
    }

    //恢复默认速度
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        patrolMoveSpeed = defaultPatrolMoveSpeed;  //恢复默认巡逻速度
        battleMoveSpeed = defaultBattleMoveSpeed;  //恢复默认战斗速度
    }

    //进入战斗状态
    public virtual void GetIntoBattleState()
    {

    }

    //受伤闪烁效果
    public override void DamageFlashEffect()
    {
        fx.StartCoroutine("FlashFX");  //启动闪烁特效协程
    }

    protected virtual void InitializeLastTimeInfo()
    {

    }

}
