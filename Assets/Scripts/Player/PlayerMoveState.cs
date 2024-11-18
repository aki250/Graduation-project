using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 
    }

    public override void Exit()
    {
        base.Exit(); 

        AudioManager.instance.StopSFX(14); //ֹͣ��Ч�����14
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.moveState)
        {
            return;
        }

        //������Ч�����14
        AudioManager.instance.PlaySFX(14, player.transform);

        //������ҵ�ˮƽ�ٶȣ���ֱ�ٶȱ��ֲ���
        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);

        //�����ҵ�ˮƽ����Ϊ0������Ҽ�⵽ǽ�����л���վ��
        if (xInput == 0 || player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}