using System.Collections;
using UnityEngine;

public class Player : Entity
{

    public SkillManager skill { get; private set; }  //���ܹ�����
    public GameObject sword { get; private set; }  //��ҵĽ�

    [Header("�ƶ�")]
    public float moveSpeed;  //����ƶ��ٶ�
    public float jumpForce;  //�����Ծ����
    public float wallJumpXSpeed;  //ǽ��ʱ��X�����ٶ�
    public float wallJumpDuration;  //ǽ������ʱ��
    private float defaultMoveSpeed;  //Ĭ���ٶ�
    private float defaultJumpForce;  //Ĭ����Ծ�߶�

    [Header("����")]
    public Vector2[] attackMovement;  //����ʱ���ƶ���ʽ
    public float counterAttackDuration = 0.2f;  //��������ʱ��

    [Header("���")]
    public float dashSpeed;  //����ٶ�
    public float dashDuration;  //��̳���ʱ��
    public float dashDirection { get; private set; }  //��̷���
    private float defaultDashSpeed;  //Ĭ�ϳ���ٶ�

    [Header("���Ӽ��")]
    [SerializeField] private BoxCollider2D pitCheck;  //���Ӷ�
    [SerializeField] private BoxCollider2D downablePlatformCheck;  //�����½���ƽ̨

    public bool isNearPit { get; set; }  //�Ƿ�ӽ��Ӷ�
    public DownablePlatform lastPlatform { get; set; }  //��һ�����½�ƽ̨
    public bool isOnPlatform { get; set; } = false;  //����Ƿ�վ��ƽ̨��

    public bool isBusy { get; private set; }  //æµ״̬
    public PlayerFX fx { get; private set; }  //��Ч���

    #region States and Statemachine
    //���״̬���͸���״̬
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

        //��ʼ������״̬
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

    //��ʼ��ʱ�趨Ĭ��ֵ
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

        CheckForDashInput();  //���������

        //�����ض���ʱʹ�ü���
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Crystal"]) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.UseSkillIfAvailable();
        }

        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Flask"]))
        {
            Inventory.instance.UseFlask_ConsiderCooldown(null);
        }
    }

    //����Ƿ��³�̼�
    private void CheckForDashInput()
    {
        if (skill.dash.dashUnlocked == false) return;  //��̼���δ����

        if (IsWallDetected()) return;  //��⵽ǽ�ڣ������г��

        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Dash"]) && SkillManager.instance.dash.UseSkillIfAvailable())
        {
            //����������׼����Ͷ����״̬�������ؽ�����׼��
            if (stateMachine.currentState == aimSwordState || stateMachine.currentState == throwSwordState)
            {
                skill.sword.ShowDots(false);
            }

            dashDirection = Input.GetAxisRaw("Horizontal");

            if (dashDirection == 0)
            {
                dashDirection = facingDirection;
            }

            stateMachine.ChangeState(dashState);  //�л������״̬
        }
    }

    //������ɺ����
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    //�������з��乥��
    public void AirLaunchJumpTrigger()
    {
        airLaunchAttackState.SetAirLaunchJumpTrigger();
    }

    //�����»�����
    public void DownStrikeTrigger()
    {
        downStrikeState.SetFallingStrikeTrigger();
    }

    //ֹͣ�»�����
    public void DownStrikeAnimStopTrigger()
    {
        downStrikeState.SetAnimStopTrigger();
    }

    //���æµʱ���޷�������������
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    //����ҷ����½�
    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    //���𽣲��л�����Ӧ״̬
    public void CatchSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    //�������Ƿ�û�н�
    public bool HasNoSword()
    {
        if (!sword) return true;

        sword.GetComponent<SwordSkillController>().ReturnSword();
        return false;
    }

    //�������ʱ���л�������״̬
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deathState);
    }

    //�������һ��ʱ���ڼ���
    public override void SlowSpeedBy(float _percentage, float _duration)
    {
        moveSpeed *= (1 - _percentage);
        jumpForce *= (1 - _percentage);
        dashSpeed *= (1 - _percentage);
        anim.speed *= (1 - _percentage);

        Invoke("ReturnDefaultSpeed", _duration);  //�ָ�Ĭ���ٶ�
    }

    //�ָ�Ĭ�ϵ��ٶ�
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    //����ʱ����˸Ч��
    public override void DamageFlashEffect()
    {
        fx.StartCoroutine("FlashFX");
    }

    //��ƽ̨����
    public void JumpOffPlatform()
    {
        if (isOnPlatform)
        {
            lastPlatform.TurnOffPlatformColliderForTime(0.5f);
        }
    }
}
