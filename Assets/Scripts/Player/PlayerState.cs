using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;  //״̬���������л���ͬ��״̬
    protected Player player;    //��ȡ������Player���
    protected Rigidbody2D rb;   //����������

    //������ص��ֶ�
    private string animBoolName;    //�����������ƣ����ڿ��ƶ���״̬
    protected float xInput;     //ˮƽ����
    protected float yInput;    //��ֱ����

    // ״̬��ʱ��������״̬������ʱ��
    protected float stateTimer;  //״̬����ʱ��ļ�ʱ��
    protected bool triggerCalled;   //������������״̬�����ڼ�⶯���Ƿ��Ѿ����

    //ʹ�ù��캯������ʼ��״̬����
    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        player = _player;   //���������Ҷ���ֵ��player
        stateMachine = _stateMachine;   //�������״̬������ֵ��stateMachine
        animBoolName = _animBoolName;   //������Ķ�����������ֵ��animBoolName
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);    //���Ŷ�Ӧ�Ķ���

        rb = player.rb;
        triggerCalled = false;  //��ʼ��������״̬Ϊfalse����ʾ������δ���
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;    //ÿ֡����״̬��ʱ��

        xInput = Input.GetAxisRaw("Horizontal");  //��ȡˮƽ
        yInput = Input.GetAxisRaw("Vertical");    //��ȡ��ֱ

        //���¿��ƶ����е�y���ٶ�
        player.anim.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);   //ֹͣ����
    }

    //������ɴ�����
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true; //�����������Ϊ�ѵ��ã������������л�״̬
    }
}
