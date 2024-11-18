using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }


    public override void Enter()
    {
        base.Enter();

        UI.instance.SwitchToEndScreen();    //�л�����Ϸ������Ļ
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.deathState)
        {
            return;
        }

        player.SetZeroVelocity(); // ��������ٶ�Ϊ�㣬��ֹ���������״̬���ƶ�
    }
}