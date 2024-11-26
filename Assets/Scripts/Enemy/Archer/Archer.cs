using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Enemy
{
    [Header("������")]
    [SerializeField] private GameObject arrowPrefab;  //����������ļ�ʸԤ����
    public Vector2 jumpVelocity;  //��������Ծ�ĳ�ʼ�ٶ�
    public float jumpCooldown;  //��Ծ����ȴʱ��
    public float jumpJudgeDistance; //����빭���ֵľ��룬����Ҿ����㹻��ʱ�������ֻ�ѡ����ԾԶ��
    public float lastTimeJumped { get; set; }  //�ϴ���Ծ��ʱ��
    [SerializeField] private float arrowFlySpeed;  //��ʸ���е��ٶ�

    [Header("�Ӷ����")]
    [SerializeField] private Transform groundBehindCheck;  //���ڼ�鹭��������Ƿ��е���
    [SerializeField] private Vector2 groundBehindCheckSize;  // �������Ĵ�С

    #region ״̬
    public ArcherIdleState idleState { get; private set; }  //����״̬
    public ArcherMoveState moveState { get; private set; }  //�ƶ�״̬
    public ArcherBattleState battleState { get; private set; }  //ս��״̬
    public ArcherAttackState attackState { get; private set; }  //����״̬
    public ArcherJumpState jumpState { get; private set; }  //��Ծ״̬
    public ArcherStunnedState stunnedState { get; private set; }  //ѣ��״̬
    public ArcherDeathState deathState { get; private set; }  //����״̬
    #endregion

    //��ʼ��״̬������
    protected override void Awake()
    {
        base.Awake();

        //����״̬���еĸ���״̬
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
        stateMachine.Initialize(idleState);  //��ʼ��Ϊ����״̬
    }

    protected override void Update()
    {
        base.Update();

        //��������ֵĹ���״̬û�б���ϣ��رշ�������
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }
    }

    //�ж��Ƿ����ͨ������������ѣ��
    public override bool CanBeStunnedByCounterAttack()
    {
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState);  //�л���ѣ��״̬
            return true;
        }

        return false;
    }

    //����������ʱ�Ĵ���
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);  //�л�������״̬
    }

    //����ս��״̬�����ǵ�ǰ״̬�Ѿ���ս��״̬����������״̬
    public override void GetIntoBattleState()
    {
        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);  //�л���ս��״̬
        }
    }

    //�����ֵ����⹥�������
    public override void SpecialAttackTrigger()
    {
        //ʵ����һ���µļ�ʸ
        GameObject newArrow = Instantiate(arrowPrefab, attackCheck.position, Quaternion.identity);

        //�����ʸ�ķ��з���ָ�����
        Vector2 flyDirection = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        Vector2 finalFlySpeed = new Vector2(flyDirection.normalized.x * arrowFlySpeed, flyDirection.normalized.y * arrowFlySpeed);

        //���ü�ʸ�ķ����ٶȺ�����
        newArrow.GetComponent<Arrow_Controller>()?.SetupArrow(finalFlySpeed, stats);
    }

    //��ʼ����Ծ�͹�����ʱ����Ϣ
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;
        lastTimeJumped = 0;  //��ʼ����Ծʱ��
    }

    //��鹭��������Ƿ��е���
    public bool GroundBehindCheck()
    {
        return Physics2D.BoxCast(groundBehindCheck.position, groundBehindCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    //��鹭��������Ƿ���ǽ��
    public bool WallBehindCheck()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -facingDirection, wallCheckDistance + 2, whatIsGround);
    }

    //Gizmos����
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireCube(groundBehindCheck.position, groundBehindCheckSize);  //���Ƽ������
    }

    //�������Ƿ��ڷ�Χ��
    public override RaycastHit2D IsPlayerDetected()
    {
        return Physics2D.CircleCast(wallCheck.position, playerScanDistance, Vector2.right * facingDirection, 0, whatIsPlayer);
    }
}
