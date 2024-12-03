using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    //��ϼ���������������
    public int comboCounter { get; private set; }

    private float lastTimeAttacked; //��¼���һ�ι���ʱ�䣬��������������ʱ�����

    private float comboWindow = 0.4f;   //��������ʱ�䣬������ͷ���һ����������������ʱ�䴰��

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        AudioManager.instance.PlaySFX(2, player.transform);

        if (stateMachine.currentState != player.primaryAttackState)
        {
            return;
        }

        //�����ϼ���������2���ߵ�ǰʱ��������һ�ι���ʱ�������������ʱ�䣬��������ϼ�������������ʱ����Ͳ�������������
        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
        {
            comboCounter = 0;
        }

        //���ö����е���ϼ���������
        player.anim.SetInteger("ComboCounter", comboCounter);

        //�������������������һ��
        float attackDirection = player.facingDirection;

        //���¸�ֵxInput�Է�ֹxInput��ֵû�м�ʱ���µ��µĴ���
        xInput = Input.GetAxisRaw("Horizontal");

        //xInput��Ϊ0���򹥻�������xInputһ��
        if (xInput != 0)
        {
            attackDirection = xInput;
        }

        //������ҵ��ٶ�Ϊ�����ƶ������ж�Ӧ��ϼ�������ֵ
        player.SetVelocity(player.attackMovement[comboCounter].x * attackDirection, player.attackMovement[comboCounter].y);

        //����״̬��ʱ��Ϊ0.1f�������ƶ�״̬�б���һ���Ĺ���
        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        //��Э��ʹ���æµ0.05��
        player.StartCoroutine(player.BusyFor(0.05f));

        //��ϼ���������
        comboCounter++;
        //�������һ�ι���ʱ��Ϊ��ǰʱ��
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //���״̬��ʱ��С��0��������ٶ�����Ϊ��
        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }

        //���������������
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}