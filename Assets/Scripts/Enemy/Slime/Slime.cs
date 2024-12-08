using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//����ʷ��ķ����ö��
public enum SlimeType
{
    big,    
    medium, 
    small   
}

//ʷ��ķ��̳��Ե��� ��
public class Slime : Enemy
{
    [Header("ʷ��ķ")]
    [SerializeField] private SlimeType slimeType; 
    [SerializeField] private int amoutOfSlimeToSpawnAfterDeath; //���������ɵ�ʷ��ķ����
    [SerializeField] private GameObject slimePrefab;    //ʷ��ķԤ����
    [SerializeField] private Vector2 minSlimeSpawnSpeed;    //����ʷ��ķʱ��С�ٶ�
    [SerializeField] private Vector2 maxSlimeSpawnSpeed;    //����ʷ��ķʱ����ٶ�

    #region States
    //ʷ��ķ����״̬
    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeathState deathState { get; private set; }
    #endregion

    //��ʼ������״̬
    protected override void Awake()
    {
        base.Awake();

        //ʵ����״̬����
        idleState = new SlimeIdleState(this, stateMachine, "Idle", this);
        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "Stunned", this);
        deathState = new SlimeDeathState(this, stateMachine, "Idle", this);

        SetupDefaultFacingDirection(-1); //Ĭ���泯��
    }

    //���ó�ʼ״̬
    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();   //��¼���һ�ι���ʱ�䣬��ֹ���ֶ��ع�������һЩ���bug
        stateMachine.Initialize(idleState); //��ʼ״̬Ϊվ��
    }

    protected override void Update()
    {
        base.Update();

        //�����ǰ״̬���ǹ���״̬���رշ�������
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }
    }

    //�ж��Ƿ��ܱ�����������ѣ��״̬
    public override bool CanBeStunnedByCounterAttack()
    {
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState); //�л���ѣ��״̬
            return true;
        }

        return false;
    }

    //ʷ��ķ����ʱ����
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState); //�л�������״̬

        //�����Сʷ��ķ����������ʷ��ķ
        if (slimeType == SlimeType.small)
        {
            return;
        }

        //���򣬸������õ����������µ�ʷ��ķ
        SpawnSlime(amoutOfSlimeToSpawnAfterDeath, slimePrefab);
    }

    //����ս��״̬
    public override void GetIntoBattleState()
    {
        //�����ǰ�Ǵ�ʷ��ķ���ҵ�ǰ״̬��ս���򹥻�״̬�����������ս��״̬
        if (slimeType == SlimeType.big && (stateMachine.currentState == battleState || stateMachine.currentState == attackState))
        {
            return;
        }

        //�����ǰ����ս��״̬��ѣ��״̬������״̬���л���ս��״̬
        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState); //����ս��״̬
        }
    }

    //����ָ��������ʷ��ķ
    private void SpawnSlime(int _amountOfSlimeToSpawn, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlimeToSpawn; i++)
        {
            //ʵ�����µ�ʷ��ķ
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);

            //���������ɵ�ʷ��ķ�ķ���
            newSlime.GetComponent<Slime>()?.SetupSpawnedSlime(facingDirection);
        }
    }

    // �������ɵ�ʷ��ķ���ƶ��ٶȺͷ���
    public void SetupSpawnedSlime(int _facingDirection)
    {
        // ��֤���ɵ�ʷ��ķ�븸����ķ���һ��
        if (facingDirection != _facingDirection)
        {
            Flip(); // ��תʷ��ķ
        }

        // �������ʷ��ķ���ٶ�
        float xVelocity = Random.Range(minSlimeSpawnSpeed.x, maxSlimeSpawnSpeed.x);
        float yVelocity = Random.Range(minSlimeSpawnSpeed.y, maxSlimeSpawnSpeed.y);

        // ʹʷ��ķ����������ʱ������
        isKnockbacked = true;

        // ����ʷ��ķ���ٶȣ�ʹ�䳯ָ�������ƶ�
        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facingDirection, yVelocity);

        // ��ֹʷ��ķ����ʱ���ٶȱ����
        Invoke("CancelKnockback", 1.5f);
    }

    // ȡ��ʷ��ķ�Ļ���״̬
    private void CancelKnockback()
    {
        isKnockbacked = false;
    }

    //��ʼ����¼���һ�ι�����ʱ��
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;
    }
}
