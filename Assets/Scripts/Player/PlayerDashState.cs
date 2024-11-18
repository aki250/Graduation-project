using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    // ����ҽ�����״̬ʱ����
    public override void Enter()
    {
        base.Enter();

        //�ڳ�̿�ʼʱ���ü��� ���Ч��
        player.skill.dash.CloneOnDashStart(player.transform.position);

        //��̳���ʱ��
        stateTimer = player.dashDuration;

        //����ڳ���ڼ�Ϊ�޵�״̬
        player.stats.BecomeInvincible(true);
    }

    public override void Exit()
    {
        base.Exit();

        //ֹͣ��ҵĺ����ٶȣ����ִ�ֱ�ٶȲ��䣩
        player.SetVelocity(0, rb.velocity.y);

        //�ڳ�̽���ʱ���ü��ܣ���������Ч����
        player.skill.dash.CloneOnDashEnd(player.transform.position);

        //ȡ������޵�״̬
        player.stats.BecomeInvincible(false);
    }

    public override void Update()
    {
        base.Update();

        //�����ǰ״̬�����ǳ��״̬��ȡ�����г��Ч��
        if (stateMachine.currentState != player.dashState)
        {
            return;
        }

        //���û�нӴ���������ǽ����ײ��ת��ǽ�ڻ���״̬
        if (!player.IsGroundDetected() && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }

        //������ҵĺ����ٶ�Ϊ����ٶȣ������ִ�ֱ�ٶȲ���
        player.SetVelocity(player.dashSpeed * player.dashDirection, 0);

        //������̺�Ĳ�ӰЧ��
        player.fx.CreateAfterimage();

        //���ʱ��������л�������״̬
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
