using UnityEngine;

public class DeathBringer : Enemy
{
    [Header("������")]
    [SerializeField] private BoxCollider2D teleportRegion;  //�����������ײ��
    [SerializeField] private Vector2 surroundingCheckSize;  //�����Χ�����Ĵ�С
    public float defaultChanceToTeleport;  //Ĭ�ϵĴ��͸���
    public float chanceToTeleport { get; set; }  //���͸���

    [Header("��������")]
    [SerializeField] private GameObject spellPrefab;  //����Ԥ����
    [SerializeField] private float spellCastStateCooldown;  //����ʩ��״̬����ȴʱ��
    public float lastTimeEnterSpellCastState { get; set; }  //�ϴν���ʩ��״̬��ʱ��
    public int castAmount;  //������ʩ�Ŵ���
    public float castCooldown;  //����ʩ�ŵ���ȴʱ��

    [Header("bossս��״̬")]
    [SerializeField] private GameObject bossNameAndHPUI;  //��ʾBoss���ֺ�Ѫ����UI
    public int stage { get; set; } = 1;  //Boss�׶Σ�Ĭ��Ϊ�׶�1

    #region States
    public DeathBringerIdleState idleState { get; private set; }  //����״̬
    public DeathBringerMoveState moveState { get; private set; }  //�ƶ�״̬
    public DeathBringerBattleState battleState { get; private set; }  //ս��״̬
    public DeathBringerAttackState attackState { get; private set; }  //����״̬
    public DeathBringerTeleportState teleportState { get; private set; }  //����״̬
    public DeathBringerCastState castState { get; private set; }  //ʩ��״̬
    public DeathBringerStunnedState stunnedState { get; private set; }  //ѣ��״̬
    public DeathBringerDeathState deathState { get; private set; }  //����״̬
    #endregion

    //��ʼ��״̬���趨
    protected override void Awake()
    {
        base.Awake();

        //����״̬���еĸ���״̬
        idleState = new DeathBringerIdleState(this, stateMachine, "Idle", this);
        moveState = new DeathBringerMoveState(this, stateMachine, "Move", this);
        battleState = new DeathBringerBattleState(this, stateMachine, "Move", this);
        attackState = new DeathBringerAttackState(this, stateMachine, "Attack", this);
        teleportState = new DeathBringerTeleportState(this, stateMachine, "Teleport", this);
        castState = new DeathBringerCastState(this, stateMachine, "Cast", this);
        stunnedState = new DeathBringerStunnedState(this, stateMachine, "Idle", this);
        deathState = new DeathBringerDeathState(this, stateMachine, "Idle", this);

        SetupDefaultFacingDirection(-1);  //����Ĭ�ϵĳ�����-1 ��ʾ����
    }

    protected override void Start()
    {
        base.Start();

        InitializeLastTimeInfo();  //��ʼ���ϴεĹ���ʱ��
        chanceToTeleport = defaultChanceToTeleport;  //���ô��͵ĸ���
        stage = 1;  //��ʼ��Ϊ�׶�1

        stateMachine.Initialize(idleState);  //��ʼ��Ϊ����״̬
    }

    protected override void Update()
    {
        base.Update();

        //���Boss�ĵ�ǰHP����60%������׶�2
        if (stats.currentHP <= stats.getMaxHP() * 0.6f)
        {
            stage = 2;  //����׶�2
        }

        //���Boss�Ĺ������жϣ��رշ�������
        if (stateMachine.currentState != attackState)
        {
            CloseCounterAttackWindow();
        }
    }

    //�ж��Ƿ���Ա�����������ѣ��״̬
    public override bool CanBeStunnedByCounterAttack()
    {
        if (base.CanBeStunnedByCounterAttack())
        {
            stateMachine.ChangeState(stunnedState);  //�л���ѣ��״̬
            return true;
        }

        return false;
    }

    //Boss����ʱ�Ĵ���
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);  //�л�������״̬

        //��������������ʾ��ͬ�ĸ�лҳ��
        if (LanguageManager.instance.localeID == 0)  //Ӣ��
        {
            UI.instance.SwitchToThankYouForPlaying("Achieved ending - Breaking the 4th wall");
        }
        else if (LanguageManager.instance.localeID == 1)  //����
        {
            UI.instance.SwitchToThankYouForPlaying("��ɽ�� �� ���Ƶ�����ǽ");
        }
    }

    //����ս��״̬����ֹ�жϹ�����ʩ��
    public override void GetIntoBattleState()
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState || stateMachine.currentState == castState)
        {
            return;  //��ǰ��ս����ʩ��״̬ʱ�����ܽ���ս��״̬
        }

        if (stateMachine.currentState != battleState && stateMachine.currentState != stunnedState && stateMachine.currentState != deathState)
        {
            stateMachine.ChangeState(battleState);  //�л���ս��״̬
        }
    }

    //��ʼ���ϴεĹ���ʱ��
    protected override void InitializeLastTimeInfo()
    {
        lastTimeAttacked = 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //�����·��ļ����
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - HasGroundBelow().distance));
        //������Χ�����ļ������
        Gizmos.DrawWireCube(transform.position, surroundingCheckSize);
    }

    //����Ƿ��е���
    private RaycastHit2D HasGroundBelow()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 100, whatIsGround);  //����·��Ƿ��е���
    }

    //�����Χ�Ƿ����ϰ���
    private RaycastHit2D HasSomethingSurrounded()
    {
        return Physics2D.BoxCast(transform.position, surroundingCheckSize, 0, Vector2.zero, 0, whatIsGround);  //�����Χ�Ƿ��е���
    }

    //���ѡ��һ������λ��
    public void FindTeleportPosition()
    {
        float x = Random.Range(teleportRegion.bounds.min.x + 3, teleportRegion.bounds.max.x - 3);  //���ѡ��X��λ��
        float y = Random.Range(teleportRegion.bounds.min.y + 3, teleportRegion.bounds.max.y - 3);  //���ѡ��Y��λ��

        transform.position = new Vector3(x, y);
        transform.position = new Vector3(transform.position.x, transform.position.y - HasGroundBelow().distance + (cd.size.y / 2));  // ����λ��ʹ��λ�ڵ�����

        // �����λ��û�е���������ϰ������Ҫ����ѡ��λ��
        if (!HasGroundBelow() || HasSomethingSurrounded())
        {
            Debug.Log("Need to find new teleport position");
            FindTeleportPosition();
        }
    }

    // �ж��Ƿ���Խ��д���
    public bool CanTeleport()
    {
        if (stage == 1)
        {
            return false;  // �׶�1ʱ���ܴ���
        }

        if (Random.Range(0, 100) <= chanceToTeleport)
        {
            return true;  // ���ݴ��͸��ʾ����Ƿ���
        }

        return false;
    }

    // �ж��Ƿ����ʩ�ŷ���
    public bool CanCastSpell()
    {
        if (stage == 1)
        {
            return false;  // �׶�1ʱ����ʩ�ŷ���
        }

        if (Time.time - lastTimeEnterSpellCastState >= spellCastStateCooldown)
        {
            return true;  // �����ȴʱ����ˣ������ʩ�ŷ���
        }

        return false;
    }

    // ʩ�ŷ���
    public void CastSpell()
    {
        Vector3 spellSpawnPosition;

        // �����������ƶ�����������������ǰ��
        if (player.rb.velocity.x != 0)
        {
            spellSpawnPosition = new Vector3(player.transform.position.x + player.facingDirection * 3, player.transform.position.y + 1.65f);
        }
        // ������û���ƶ������������������Ϸ�
        else
        {
            spellSpawnPosition = new Vector3(player.transform.position.x, player.transform.position.y + 1.65f);
        }

        // ʵ����������������������
        GameObject newSpell = Instantiate(spellPrefab, spellSpawnPosition, Quaternion.identity);
        newSpell.GetComponent<SpellController>()?.SetupSpell(stats);
    }

    // ��ʾBoss�����ֺ�Ѫ��
    public void ShowBossHPAndName()
    {
        bossNameAndHPUI.SetActive(true);
    }

    // �ر�Boss�����ֺ�Ѫ��UI
    public void CloseBossHPAndName()
    {
        bossNameAndHPUI.SetActive(false);
    }
}
