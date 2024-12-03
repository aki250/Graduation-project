using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        // �������ڽ����״̬ʱ�Ǳ���״̬������΢����һ�ξ���
        stateTimer = 0.1f; //��ʱ��0.1��

        //��ʾ��׼��
        player.skill.sword.ShowDots(true);
    }

    public override void Exit()
    {
        base.Exit();

        //�����Ͷ�������������ƶ�����ʼЭ��ʹ�����0.1���ڴ���æµ״̬
        player.StartCoroutine(player.BusyFor(0.1f));
    }

    public override void Update()
    {
        base.Update(); 

        if (stateMachine.currentState != player.aimSwordState)
        {
            return;
        }

        if (stateTimer < 0)
        {
            player.SetZeroVelocity(); //ֹͣ��ҵ��ƶ��ٶ�
        }

        //��ȡ���λ�ò�����ת��Ϊ��������
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //�������λ������ת��ҵĳ���ȷ����ҳ�����ȷ
        if (mousePosition.x < player.transform.position.x && player.facingDirection == 1)
        {
            player.Flip();
        }
        else if (mousePosition.x > player.transform.position.x && player.facingDirection == -1)
        {
            player.Flip();
        }

        //�������ɿ��Ҽ�����׼���������˳�AimSword״̬
        if (Input.GetKeyUp(KeyBindManager.instance.keybindsDictionary["Aim"]))
        {
            //�˳�AimSword״̬ʱ��������ʾ��׼��
            player.skill.sword.ShowDots(false);

            stateMachine.ChangeState(player.idleState);
        }

        //�����Ұ�������������������л���Ͷ������״̬
        if (Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            //��ThrowSword�����л����ThrowSword()��������������CreateSword()���������ջ����ShowDots(false)��������׼��
            stateMachine.ChangeState(player.throwSwordState);
        }
    }
}
