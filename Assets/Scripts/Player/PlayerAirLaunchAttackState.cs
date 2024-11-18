using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirLaunchAttackState : PlayerState
{
    //����Ƿ񴥷��˿��з��𹥻�
    public bool airLaunchJumpTrigger { get; private set; } = false;

    //��¼����Ƿ��Ѿ�����
    private bool hasJumped = false;

    //��ʼ����Һ�״̬��
    public PlayerAirLaunchAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //���ÿ��з��𹥻���־
        airLaunchJumpTrigger = false;
        //������Ծ״̬
        hasJumped = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //�����ǰ״̬���ǿ��з��𹥻�״̬�����˳�����
        if (player.stateMachine.currentState != this)
        {
            return;
        }

        //��������˿��з��𹥻�����δִ����Ծ
        if (airLaunchJumpTrigger && !hasJumped)
        {
            //������ҵ���Ծ�ٶȣ���ֱ�����ϸ���һ����ʼ�ٶ�
            player.SetVelocity(0, 17); // ����17����Ծ�Ĵ�ֱ�ٶ�
            //���Ϊ����Ծ
            hasJumped = true;
        }

        //�ı�״̬��Ϊ����״̬
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.airState);
        }
    }

    //���ڴ������з��𹥻�
    public void SetAirLaunchJumpTrigger()
    {
        airLaunchJumpTrigger = true; //���ÿ��з��𹥻���־Ϊ��
    }
}
