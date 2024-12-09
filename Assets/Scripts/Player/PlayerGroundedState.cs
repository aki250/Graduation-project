using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

                                                            //��ʾ����ڵ����ϵ�״̬
public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        //������¹����������л�����ҵ���ͨ����״̬
        if (Input.GetKeyDown(/*KeyCode.Mouse0*/ KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }

        //���ͬʱ����ǰ������W���͹����������л�����ҵĿ��з��乥��״̬
        if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            stateMachine.ChangeState(player.airLaunchAttackState);
        }

        //��������мܼ��������мܼ����ѽ����ҿ��ã���ʹ���мܼ���
        if (Input.GetKeyDown(/*KeyCode.Q*/ KeyBindManager.instance.keybindsDictionary["Parry"]) && player.skill.parry.parryUnlocked && player.skill.parry.SkillIsReadyToUse())
        {
            //stateMachine.ChangeState(player.counterAttackState);
            SkillManager.instance.parry.UseSkillIfAvailable();
        }

        //�ڶ������ѽ����ҿ��ã����л����ڶ�����״̬
        if (Input.GetKeyDown( KeyBindManager.instance.keybindsDictionary["Blackhole"]) && player.skill.blackhole.blackholeUnlocked && player.skill.blackhole.SkillIsReadyToUse())
        {
            stateMachine.ChangeState(player.blackholeSkillState);
        }

        //���������׼�����������û�н���Ͷ���������ѽ��������л�����׼��״̬
        if (Input.GetKeyDown(/*KeyCode.Mouse1*/ KeyBindManager.instance.keybindsDictionary["Aim"]) && player.HasNoSword() && player.skill.sword.throwSwordSkillUnlocked)
        {
            stateMachine.ChangeState(player.aimSwordState);
        }

        // ������û�м�⵽���棬���л�������״̬
        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        //��������ƽ̨�ϲ��Ҽ�⵽����
        if (player.IsGroundDetected() && player.isOnPlatform)
        {
            // ������º��˼���S��������Ծ��������Ҵ�ƽ̨������
            if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
            {
                player.JumpOffPlatform();
                return;
            }
        }

        //�����Ҽ�⵽���沢�Ұ�����Ծ�������л�����Ծ״̬
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }
}