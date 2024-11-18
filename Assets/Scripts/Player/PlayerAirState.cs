using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    //��ʼ��Player��StateMachine
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        //ȷ��״̬����ǰ״̬�ǿ���״̬
        if (stateMachine.currentState != player.airState)
        {
            return; //������ǿ���״̬����ֱ�ӷ���
        }

        //����к�������ʱ��������ҵ�ˮƽ�ٶ�
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);
        }

        //�������Ƿ�����ǽ�ڲ��Ҳ���ƽ̨��
        if (player.IsWallDetected() && !player.isOnPlatform)
        {
            //�����ǽ�ڻ���״̬���л���ǽ�ڻ���״̬
            stateMachine.ChangeState(player.wallSlideState);
        }

        //�������S�����ҹ�����ť�����£��л����»�״̬
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            stateMachine.ChangeState(player.downStrikeState);
        }

        if (player.IsGroundDetected())
        {
            //�޸���Ҵӿ������ʱ�Ŀ������⣬����ˮƽ����
            xInput = Input.GetAxisRaw("Horizontal");

            if (xInput != 0)
            {
                //�����ˮƽ���룬�л����ƶ�״̬
                stateMachine.ChangeState(player.moveState);
            }
            else
            {
                //���û��ˮƽ���룬�л�����ֹ״̬
                stateMachine.ChangeState(player.idleState);
            }
        }
    }
}
