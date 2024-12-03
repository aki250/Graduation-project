using System.Collections;
using UnityEngine;

                                        //Entity ������Ϸ��ʵ��Ļ����࣬�ṩ����ײ��⡢������⡢����Ч����ͨ�ù���
public class Entity : MonoBehaviour
{
    [Header("ǽ�ڼ��")]
    [SerializeField] protected Transform groundCheck; //�������Transform���
    [SerializeField] protected float groundCheckDistance = 1; //���������
    [SerializeField] protected Transform wallCheck; //ǽ�ڼ���Transform���
    [SerializeField] protected float wallCheckDistance = 0.6f; //ǽ�ڼ�����
    [SerializeField] protected LayerMask whatIsGround; //��������룬�����������߼��

    public Transform attackCheck; //����������
    public float attackCheckRadius = 1.2f; //�������뾶

    [Header("���˼��")]
    public Vector2 knockbackMovement = new Vector2(5, 3); //�����ƶ��ĳ�ʼֵ
    public Vector2 randomKnockbackMovementOffsetRange; //�����ƶ������ƫ�Ʒ�Χ
    [SerializeField] protected float knockbackDuration = 0.2f; //���˳���ʱ��
    public bool isKnockbacked { get; set; } //�Ƿ����ڱ�����

    public int facingDirection { get; private set; } = 1; //������
    protected bool facingRight = true; //�Ƿ������Ҳ�

    #region components
    public SpriteRenderer sr { get; private set; }
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    public System.Action onFlipped; //��ʵ�巭תʱ���õ�ί��

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();  //��ɫͼ��
        anim = GetComponentInChildren<Animator>();  //����
        rb = GetComponent<Rigidbody2D>();   //�������
        stats = GetComponent<CharacterStats>();     //״̬����
        cd = GetComponent<CapsuleCollider2D>(); //��ײ��
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    //�˺���˸Ч����ʵ��
    public virtual void DamageFlashEffect()
    {
    }

    public virtual void DamageKnockbackEffect(Transform _attacker, Transform _attackee)
    {
        //���ݹ����ߺͱ������ߵ�λ�ü�����˷���
        float _knockbackDirection = CalculateKnockbackDirection(_attacker, _attackee);

        //��ʼ����Ч����Э��
        StartCoroutine(HitKnockback(_knockbackDirection));
    }

    protected virtual IEnumerator HitKnockback(float _knockbackDirection)
    {
        //�������ڱ�����Ϊtrue
        isKnockbacked = true;

        //��������ƶ������ƫ��
        float xOffset = Random.Range(0, randomKnockbackMovementOffsetRange.x);
        float yOffset = Random.Range(0, randomKnockbackMovementOffsetRange.y);

        //���û����ٶ�
        rb.velocity = new Vector2((knockbackMovement.x + xOffset) * _knockbackDirection, knockbackMovement.y + yOffset);

        //���˳���ʱ��
        yield return new WaitForSeconds(knockbackDuration);

        //ˮƽ�ٶ�Ϊ0����������Ч��
        rb.velocity = new Vector2(0, rb.velocity.y);

        //�������ڱ�����Ϊfalse
        isKnockbacked = false;
    }

    public virtual float CalculateKnockbackDirection(Transform _attacker, Transform _attackee)
    {
        //���ݹ����ߺͱ������ߵ�λ�ü�����˷���
        float _knockbackDirection = 0;

        if (_attacker.position.x < _attackee.position.x)
        {
            _knockbackDirection = 1; //�������ڱ������ߵ���࣬����˷���Ϊ��
        }
        else if (_attacker.position.x > _attackee.position.x)
        {
            _knockbackDirection = -1; //�������ڱ������ߵ��Ҳ࣬����˷���Ϊ��
        }

        return _knockbackDirection; //���ػ��˷���
    }

    public virtual void SetupKnockbackMovement(Vector2 _knockbackMovement)
    {
        //�����ƶ��ĳ�ʼֵ
        knockbackMovement = _knockbackMovement;
    }

    public virtual void SetupZeroKnockbackMovement()
    {
        //�����ƶ�Ϊ���ʵ��
    }

    #region Velocity
    public virtual void SetVelocity(float _xVelocity, float _yVelocity)
    {
        //ʵ����ٶ�
        if (isKnockbacked) //������ڱ����ˣ��������ٶ�
        {
            return;
        }

        rb.velocity = new Vector2(_xVelocity, _yVelocity); //�ٶ�
        FlipController(_xVelocity); //�����ٶȷ�ת������
    }

    public virtual void SetZeroVelocity()
    {
        //����ʵ����ٶ�Ϊ��
        if (isKnockbacked) //������ڱ����ˣ��������ٶ�
        {
            return;
        }

        rb.velocity = new Vector2(0, 0); //�����ٶ�Ϊ��
    }
    #endregion

    #region Collision
    protected virtual void OnDrawGizmos()
    {
        //����Gizmos�����ڵ�����ײ���͹������
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDirection, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    public virtual bool IsGroundDetected()
    {
        //����Ƿ�Ӵ�����
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    public virtual bool IsWallDetected()
    {
        //����Ƿ�Ӵ�ǽ��
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsGround);
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        //��תʵ��ķ���
        facingDirection = -facingDirection; //��ת������
        facingRight = !facingRight; //��ת�����Ҳ�ı�־
        transform.Rotate(0, 180, 0); //��תʵ��

        if (onFlipped != null) //�����ί�У������
        {
            onFlipped();
        }
    }

    public virtual void FlipController(float _x)
    {
        //����ˮƽ�ٶȷ�ת������
        if (_x > 0 && !facingRight) //����ٶ�Ϊ�����������
        {
            Flip();
        }
        else if (_x < 0 && facingRight) //�ٶ�Ϊ���������Ҳ�
        {
            Flip(); 
        }
    }

    public void SetupDefaultFacingDirection(int _facingDirection)
    {
        //Ĭ��������
        facingDirection = _facingDirection;

        if (facingDirection == -1) //���������Ϊ�����Ҳ� Ϊfalse
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
        //anim.speed = 1; //���ö����ٶ�Ϊ1
    }
}