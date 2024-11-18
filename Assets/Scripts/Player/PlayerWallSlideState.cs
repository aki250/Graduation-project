using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                        //�����ǽ����״̬
public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.wallSlideState)
        {
            return;
        }

        //�����Ҳ��ټ�⵽ǽ��
        if (!player.IsWallDetected())
        {
            //�л�������״̬
            stateMachine.ChangeState(player.airState);
        }

        //�����Ұ����·���������ٻ����ٶ�
        if (yInput < 0)
        {
            player.SetVelocity(0, rb.velocity.y); //���ֵ�ǰ��ֱ�ٶ�
        }
        else
        {
            player.SetVelocity(0, rb.velocity.y * 0.2f); //���ٴ�ֱ�ٶ���ģ�⻬��
        }

        //�����Ұ�����Ծ����Space��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //�л���ǽ����Ծ״̬
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

        //�����ҵ�ˮƽ���벻Ϊ������������һ��
        if (xInput != 0 && player.facingDirection != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }

        //�����Ҽ�⵽����
        if (player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}