using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

                                                            //表示玩家在地面上的状态
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

        //如果按下攻击键，则切换到玩家的普通攻击状态
        if (Input.GetKeyDown(/*KeyCode.Mouse0*/ KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            stateMachine.ChangeState(player.primaryAttackState);
        }

        //如果同时按下前进键（W）和攻击键，则切换到玩家的空中发射攻击状态
        if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyBindManager.instance.keybindsDictionary["Attack"]))
        {
            stateMachine.ChangeState(player.airLaunchAttackState);
        }

        //如果按下招架键，并且招架技能已解锁且可用，则使用招架技能
        if (Input.GetKeyDown(/*KeyCode.Q*/ KeyBindManager.instance.keybindsDictionary["Parry"]) && player.skill.parry.parryUnlocked && player.skill.parry.SkillIsReadyToUse())
        {
            //stateMachine.ChangeState(player.counterAttackState);
            SkillManager.instance.parry.UseSkillIfAvailable();
        }

        //黑洞技能已解锁且可用，则切换到黑洞技能状态
        if (Input.GetKeyDown( KeyBindManager.instance.keybindsDictionary["Blackhole"]) && player.skill.blackhole.blackholeUnlocked && player.skill.blackhole.SkillIsReadyToUse())
        {
            stateMachine.ChangeState(player.blackholeSkillState);
        }

        //如果按下瞄准键，并且玩家没有剑且投掷剑技能已解锁，则切换到瞄准剑状态
        if (Input.GetKeyDown(/*KeyCode.Mouse1*/ KeyBindManager.instance.keybindsDictionary["Aim"]) && player.HasNoSword() && player.skill.sword.throwSwordSkillUnlocked)
        {
            stateMachine.ChangeState(player.aimSwordState);
        }

        // 如果玩家没有检测到地面，则切换到空中状态
        if (!player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.airState);
        }

        //如果玩家在平台上并且检测到地面
        if (player.IsGroundDetected() && player.isOnPlatform)
        {
            // 如果按下后退键（S）并且跳跃键，则玩家从平台上跳下
            if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
            {
                player.JumpOffPlatform();
                return;
            }
        }

        //如果玩家检测到地面并且按下跳跃键，则切换到跳跃状态
        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }
}