using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                                //�����ǽ����Ծ״̬
public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        //״̬��ʱ��Ϊ��ҵ�ǽ����Ծ����ʱ��
        stateTimer = player.wallJumpDuration;
        //��ҵ��ٶ�Ϊǽ����Ծ��ˮƽ�ٶȳ�����������෴������ʵ�������������Ծ������ֱ�ٶ�Ϊ��Ծ��
        player.SetVelocity(player.wallJumpXSpeed * -player.facingDirection, player.jumpForce);
    }

    // ���˳�ǽ����Ծ״̬ʱ����
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //�����ǰ״̬����ǽ����Ծ״̬����ִ��ʣ��ĸ��º�������
        if (stateMachine.currentState != player.wallJumpState)
        {
            return;
        }

        //���״̬��ʱ��С��0����ǽ����Ծʱ�����
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.airState);
        }

        //�����棬��������
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}