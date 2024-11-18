using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword; //�洢����λ�ã������ж�λ�ú���ת

    public PlayerCatchSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        sword = player.sword.transform; //��ȡ������еĽ���λ��

        player.fx.PlayDustFX(); //���ų�����Ч
        player.fx.ScreenShake(player.fx.shakeDirection_medium); //������Ļ����Ч�������𶯷���

        //���ݽ���λ������ת��ҵĳ���
        if (sword.position.x < player.transform.position.x && player.facingDirection == 1)
        {
            player.Flip(); //������������߲�������泯�ң���ת��ҳ���
        }
        else if (sword.position.x > player.transform.position.x && player.facingDirection == -1)
        {
            player.Flip(); //�����������ұ߲�������泯����ת��ҳ���
        }

        stateTimer = 0.1f; //����״̬��ʱ�������ƻ��˵ĳ���ʱ��
        rb.velocity = new Vector2(player.moveSpeed * -player.facingDirection, rb.velocity.y); // ������ҵĻ����ٶ�
    }

    public override void Exit()
    {
        base.Exit(); 

        player.StartCoroutine(player.BusyFor(0.1f)); //�������΢ͣ��һ��ʱ�䣬��ֹ��������������
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != player.catchSwordState)
        {
            return; //�����ǰ״̬����catchSwordState����������������
        }

        //���״̬��ʱ��С��0����ʾ���˶���������ֹͣ����
        if (stateTimer < 0)
        {
            player.SetVelocity(0, rb.velocity.y); //�������ˮƽ�ٶ�Ϊ0����ֱ�ٶȲ���
        }

        //����������������㣨���粶׽�꽣�󣩣��л��ؿ���״̬
        if (triggerCalled)
        {
            player.stateMachine.ChangeState(player.idleState); //�л�������״̬
        }
    }
}
