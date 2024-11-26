using UnityEngine;
                                                    //���˸���״̬
public class EnemyState
{
    protected EnemyStateMachine stateMachine;          //״̬���������״̬
    protected Enemy enemyBase;                          //���˻�����ʵ�����������˹������ԣ�����

    protected Rigidbody2D rb; //�������
    protected Animator anim; //���������

    private string animBoolName; //��������

    protected float stateTimer; //״̬��ʱ�������ڿ���״̬�ĳ���ʱ��
    protected bool triggerCalled; //��������־�����ڱ�Ƕ����������Ƿ��ѱ�����


    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        enemyBase = _enemyBase; //������ʵ��
        stateMachine = _stateMachine; //״̬��
        animBoolName = _animBoolName; //������������
    }

    public virtual void Enter()
    {
        triggerCalled = false; //��ʼ����������־Ϊfalse

        rb = enemyBase.rb; //��ȡ���˸������
        anim = enemyBase.anim; //��ȡ���˵Ķ��������

        anim.SetBool(animBoolName, true); //true�����Ŷ�Ӧ����
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false); //ֹͣ����

        enemyBase.AssignLastAnimBoolName(animBoolName); //����ǰ���������������Ʊ���Ϊ���һ��ʹ�õ�����
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime; //����״̬��ʱ��
    }

    // �������������õķ���
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true; //��Ǵ������ѱ�����
    }
}
