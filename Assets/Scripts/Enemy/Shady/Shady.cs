using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shady : Enemy
{
    [Header("shady�Ա�Ч��")]
    [SerializeField] private GameObject explosionPrefab;  //��ըЧ��Ԥ����
    [SerializeField] private float explosionMaxSize;     //��ը���ߴ�
    [SerializeField] private float explosionGrowSpeed;   //��ը�����ٶ�

    #region States
    //Shady��Ĳ�ͬ״̬
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

        // **��ʼ������״̬**��ÿ��״̬��ͨ��״̬�����й���
        idleState = new ShadyIdleState(this, stateMachine, "Idle", this);  // ����״̬
        moveState = new ShadyMoveState(this, stateMachine, "Move", this);  // �ƶ�״̬
        battleState = new ShadyBattleState(this, stateMachine, "BattleMove", this);  // ս��״̬
        attackState = new ShadyAttackState(this, stateMachine, "Attack", this);  // ����״̬
        explosionState = new ShadyExplosionState(this, stateMachine, "Explosion", this);  // ��ը״̬
        // stunnedState = new ShadyStunnedState(this, stateMachine, "Stunned", this);  // ������״̬ (ע�͵���)
        deathState = new ShadyDeathState(this, stateMachine, "Death", this);  // ����״̬
    }

    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();  //��ʼ���ϴι�����Ϣ��
        stateMachine.Initialize(idleState);  //��ʼΪ����״̬
    }

    protected override void Update()
    {
        base.Update();

        //��ֹ�������ж�ʱ��ʼ����ʾ����ͼ��
        // if (stateMachine.currentState != attackState)
        // {
        //     CloseCounterAttackWindow();
        // }
    }

    // �����Ƿ��ܹ����ε���
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
            stateMachine.ChangeState(battleState);  //�л���ս��״̬
        }
    }

    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;  //��ʼ���ϴι���ʱ��
    }

    //�Ա�
    public override void SpecialAttackTrigger()
    {
        //ʵ������ըЧ��
        GameObject newExplosion = Instantiate(explosionPrefab, attackCheck.position, Quaternion.identity);

        //��ը�ĳɳ��ٶȺ����ߴ�
        newExplosion.GetComponent<ShadyExplosion_Controller>()?.SetupExplosion(stats, explosionGrowSpeed, explosionMaxSize, attackCheckRadius);

        cd.enabled = false;  //�ر���ȴʱ��
        rb.gravityScale = 0;  //ֹͣ��������

        //�Ա���������Ʒ�ͻ���
        EnemyStats myStats = stats as EnemyStats;
        myStats.ZeroHP(); 
        myStats.DropCurrencyAndItem();  //����Ʒ
    }

    public void SelfDestroy()
    {
        Destroy(gameObject); 
    }
}
