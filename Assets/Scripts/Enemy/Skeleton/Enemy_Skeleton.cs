using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    #region States
    //���õ���״̬
    public SkeletonIdleState idleState { get; private set; }    //����
    public SkeletonMoveState moveState { get; private set; }    //�ƶ�
    public SkeletonBattleState battleState { get; private set; }    //ս��
    public SkeletonAttackState attackState { get; private set; }    //����
    public SkeletonStunnedState stunnedState { get; private set; }  //ѣ��
    public SkeletonDeathState deathState { get; private set; }  //����״̬
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SkeletonIdleState(this, stateMachine, "Idle", this);
        moveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, stateMachine, "Move", this);
        attackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deathState = new SkeletonDeathState(this, stateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();

        //��ʼ���������һ�ι�����ʱ����Ϣ
        InitializeLastTimeInfo();
        //���˳�ʼ״̬Ϊ����״̬
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        //��ֹ���������ڹǼܵ��˹��������ʱ������ʾ
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }

    }

    // ��д���෽�����жϹǼܵ����Ƿ���Ա���������
    public override bool CanBeStunnedByCounterAttack()
    {
        // ���������Է������Σ������ѣ��״̬
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState);  // �л���ѣ��״̬
            return true;  // ����true����ʾ���˱�����
        }

        return false;  // ���򷵻�false
    }

    //��д���������������߼�
    public override void Die()
    {
        base.Die(); 

        stateMachine.ChangeState(deathState);
    }

    //����ս��״̬
    public override void GetIntoBattleState()
    {
        //�����ǰ�Ѿ���ս��״̬�򹥻�״̬�����Ѿ��������Ͳ��ٽ���ս��״̬
        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);  //�л���ս��״̬
        }
    }

    //��ʼ�������ϴι�����ʱ�䣬��ֹ�߼�����
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;  //��ʼ������ʱ��
    }
}
