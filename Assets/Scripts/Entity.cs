using System.Collections;
using UnityEngine;

                                        //Entity 类是游戏中实体的基础类，提供了碰撞检测、攻击检测、击退效果等通用功能
public class Entity : MonoBehaviour
{
    [Header("墙壁检测")]
    [SerializeField] protected Transform groundCheck; //地面检测的Transform组件
    [SerializeField] protected float groundCheckDistance = 1; //地面检测距离
    [SerializeField] protected Transform wallCheck; //墙壁检测的Transform组件
    [SerializeField] protected float wallCheckDistance = 0.6f; //墙壁检测距离
    [SerializeField] protected LayerMask whatIsGround; //地面层掩码，用于物理射线检测

    public Transform attackCheck; //攻击检测组件
    public float attackCheckRadius = 1.2f; //攻击检测半径

    [Header("击退检测")]
    public Vector2 knockbackMovement = new Vector2(5, 3); //击退移动的初始值
    public Vector2 randomKnockbackMovementOffsetRange; //击退移动的随机偏移范围
    [SerializeField] protected float knockbackDuration = 0.2f; //击退持续时间
    public bool isKnockbacked { get; set; } //是否正在被击退

    public int facingDirection { get; private set; } = 1; //面向方向
    protected bool facingRight = true; //是否面向右侧

    #region components
    public SpriteRenderer sr { get; private set; }
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    public System.Action onFlipped; //当实体翻转时调用的委托

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();  //角色图像
        anim = GetComponentInChildren<Animator>();  //动画
        rb = GetComponent<Rigidbody2D>();   //物理组件
        stats = GetComponent<CharacterStats>();     //状态数据
        cd = GetComponent<CapsuleCollider2D>(); //碰撞体
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    //伤害闪烁效果的实现
    public virtual void DamageFlashEffect()
    {
    }

    public virtual void DamageKnockbackEffect(Transform _attacker, Transform _attackee)
    {
        //根据攻击者和被攻击者的位置计算击退方向
        float _knockbackDirection = CalculateKnockbackDirection(_attacker, _attackee);

        //开始击退效果的协程
        StartCoroutine(HitKnockback(_knockbackDirection));
    }

    protected virtual IEnumerator HitKnockback(float _knockbackDirection)
    {
        //设置正在被击退为true
        isKnockbacked = true;

        //计算击退移动的随机偏移
        float xOffset = Random.Range(0, randomKnockbackMovementOffsetRange.x);
        float yOffset = Random.Range(0, randomKnockbackMovementOffsetRange.y);

        //设置击退速度
        rb.velocity = new Vector2((knockbackMovement.x + xOffset) * _knockbackDirection, knockbackMovement.y + yOffset);

        //击退持续时间
        yield return new WaitForSeconds(knockbackDuration);

        //水平速度为0，结束击退效果
        rb.velocity = new Vector2(0, rb.velocity.y);

        //设置正在被击退为false
        isKnockbacked = false;
    }

    public virtual float CalculateKnockbackDirection(Transform _attacker, Transform _attackee)
    {
        //根据攻击者和被攻击者的位置计算击退方向
        float _knockbackDirection = 0;

        if (_attacker.position.x < _attackee.position.x)
        {
            _knockbackDirection = 1; //攻击者在被攻击者的左侧，则击退方向为右
        }
        else if (_attacker.position.x > _attackee.position.x)
        {
            _knockbackDirection = -1; //攻击者在被攻击者的右侧，则击退方向为左
        }

        return _knockbackDirection; //返回击退方向
    }

    public virtual void SetupKnockbackMovement(Vector2 _knockbackMovement)
    {
        //击退移动的初始值
        knockbackMovement = _knockbackMovement;
    }

    public virtual void SetupZeroKnockbackMovement()
    {
        //击退移动为零的实现
    }

    #region Velocity
    public virtual void SetVelocity(float _xVelocity, float _yVelocity)
    {
        //实体的速度
        if (isKnockbacked) //如果正在被击退，则不设置速度
        {
            return;
        }

        rb.velocity = new Vector2(_xVelocity, _yVelocity); //速度
        FlipController(_xVelocity); //根据速度翻转控制器
    }

    public virtual void SetZeroVelocity()
    {
        //设置实体的速度为零
        if (isKnockbacked) //如果正在被击退，则不设置速度
        {
            return;
        }

        rb.velocity = new Vector2(0, 0); //设置速度为零
    }
    #endregion

    #region Collision
    protected virtual void OnDrawGizmos()
    {
        //绘制Gizmos，用于调试碰撞检测和攻击检测
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    public virtual bool IsGroundDetected()
    {
        //检测是否接触地面
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    public virtual bool IsWallDetected()
    {
        //检测是否接触墙壁
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        //翻转实体的方向
        facingDirection = -facingDirection; //翻转面向方向
        facingRight = !facingRight; //翻转面向右侧的标志
        transform.Rotate(0, 180, 0); //旋转实体

        if (onFlipped != null) //如果有委托，则调用
        {
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        //根据水平速度翻转控制器
        if (_x > 0 && !facingRight) //如果速度为正且面向左侧
        {
            Flip();
        }
        else if (_x < 0 && facingRight) //速度为负且面向右侧
        {
            Flip(); 
        }
    }

    public void SetupDefaultFacingDirection(int _facingDirection)
    {
        //默认面向方向
        facingDirection = _facingDirection;

        if (facingDirection == -1) //如果面向方向为左，则右侧 为false
        {
            facingRight = false; 
        }
    }
    #endregion

    public virtual void Die()
    {
    }

    public virtual void SlowSpeedBy(float _percentage, float _duration)
    {
    }

    protected virtual void ReturnDefaultSpeed()
    {
        //anim.speed = 1; //设置动画速度为1
    }
}