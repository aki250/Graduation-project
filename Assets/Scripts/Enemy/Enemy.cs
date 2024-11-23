using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(CapsuleCollider2D))] 
[RequireComponent(typeof(EnemyStats))] 
[RequireComponent(typeof(EntityFX))] 
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [Header("�ƶ�")] 
    public float patrolMoveSpeed;  //Ѳ���ٶ�
    public float patrolStayTime;  //Ѳ��ʱ��
    private float defaultPatrolMoveSpeed;  //Ĭ��Ѳ���ٶ�

    [Header("��췶Χ")] 
    public float playerScanDistance = 10;  //���ɨ�����
    public float playerHearDistance = 3;  //���������Χ

    [SerializeField] protected LayerMask whatIsPlayer;  //��Ҳ�

    [Header("ս���ٶ�")] 
    public float battleMoveSpeed;  //ս���ƶ��ٶ�
    public float aggressiveTime = 7;  //����ʱ��

    private float defaultBattleMoveSpeed;  //Ĭ��ս���ƶ��ٶ�

    [Header("����")] 
    public float attackDistance = 2;  //��������
    public float attackCooldown = 1.5f;  //������ȴʱ��
    public float minAttackCooldown = 1;  //��С������ȴʱ��
    public float maxAttackCooldown = 2;  //��󹥻���ȴʱ��
    [HideInInspector] public float lastTimeAttacked;  //�ϴι���ʱ��

    [Header("ѣ���趨")]
    protected bool canBeStunned;  //�Ƿ��ܱ�ѣ��
    public float stunDuration = 1;  //ѣ�γ���ʱ��
    public Vector2 stunMovement = new Vector2(3, 3);  //ѣ��ʱ���ƶ���
    [SerializeField] protected GameObject counterPromptImage;  //������ʾͼƬ

    public EnemyStateMachine stateMachine { get; private set; } 
    protected Player player { get; private set; } 
    public EntityFX fx { get; private set; }

    public string lastAnimBoolName { get; private set; } 

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine(); 
        fx = GetComponent<EntityFX>();  //��ȡEntityFX���

        defaultBattleMoveSpeed = battleMoveSpeed;  //Ĭ��ս���ƶ��ٶ�
        defaultPatrolMoveSpeed = patrolMoveSpeed;  //Ĭ��Ѳ���ƶ��ٶ�
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

        Gizmos.color = Color.yellow;  //����Gizmos
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDirection, transform.position.y));  // ���ƹ�����Χ��
    }

    //�������Ƿ񱻵�����⵽
    public virtual RaycastHit2D IsPlayerDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, playerScanDistance, whatIsPlayer);
    }

    //����������
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    //���⹥��������
    public virtual void SpecialAttackTrigger()
    {

    }

    //�������
    public virtual void FreezeEnemy(bool _freeze)
    {
        if (_freeze)
        {
            battleMoveSpeed = 0;  //ս���ƶ��ٶ�Ϊ0
            patrolMoveSpeed = 0;  //Ѳ���ƶ��ٶ�Ϊ0
            anim.speed = 0;  //�����ٶ�Ϊ0
        }
        else
        {
            battleMoveSpeed = defaultBattleMoveSpeed;  //�ָ�Ĭ��ս���ƶ��ٶ�
            patrolMoveSpeed = defaultPatrolMoveSpeed;  //�ָ�Ĭ��Ѳ���ƶ��ٶ�
            anim.speed = 1;  //�ָ������ٶ�Ϊ1
        }
    }

    //�������һ��ʱ��
    protected virtual IEnumerator FreezeEnemyForTime_Coroutine(float _seconds)
    {
        FreezeEnemy(true);  //�������

        yield return new WaitForSeconds(_seconds);  //�ȴ�ָ��ʱ��

        FreezeEnemy(false);  //�ָ�����״̬
    }

    //�������һ��ʱ��ĺ����ӿ�
    public virtual void FreezeEnemyForTime(float _seconds)
    {
        StartCoroutine(FreezeEnemyForTime_Coroutine(_seconds));  //����Э��
    }

    #region Counter Attack ��������
    //�򿪷�������
    public void OpenCounterAttackWindow()
    {
        canBeStunned = true;  //���ÿ��Ա�����
        counterPromptImage.SetActive(true);  //��ʾ������ʾ
    }

    //�رշ�������
    public void CloseCounterAttackWindow()
    {
        canBeStunned = false;  //���ò��ܱ�����
        counterPromptImage.SetActive(false);  //���ط�����ʾ
    }

    //�ж��Ƿ���Ա�����
    public virtual bool CanBeStunnedByCounterAttack()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();  //�رշ�������
            return true;
        }
        return false;
    }
    #endregion

    //�����ϴζ���״̬����
    public virtual void AssignLastAnimBoolName(string _animBoolName)
    {
        lastAnimBoolName = _animBoolName;  //��ֵ
    }

    //���ٴ���
    public override void SlowSpeedBy(float _percentage, float _duration)
    {
        patrolMoveSpeed = patrolMoveSpeed * (1 - _percentage);  //����Ѳ���ٶ�
        battleMoveSpeed = battleMoveSpeed * (1 - _percentage);  //����ս���ٶ�
       //anim.speed = anim.speed * (1 - _percentage);  //���Ͷ����ٶ�

        Invoke("ReturnDefaultSpeed", _duration);  //�ָ��ٶȵ���ʱ����
    }

    //�ָ�Ĭ���ٶ�
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        patrolMoveSpeed = defaultPatrolMoveSpeed;  //�ָ�Ĭ��Ѳ���ٶ�
        battleMoveSpeed = defaultBattleMoveSpeed;  //�ָ�Ĭ��ս���ٶ�
    }

    //����ս��״̬
    public virtual void GetIntoBattleState()
    {

    }

    //������˸Ч��
    public override void DamageFlashEffect()
    {
        fx.StartCoroutine("FlashFX");  //������˸��ЧЭ��
    }

    protected virtual void InitializeLastTimeInfo()
    {

    }

}
