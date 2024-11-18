using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                            //��ҵķ���״̬
public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;    //�������Ƿ�����ڵ�ǰ�����д�����¡��

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter(); 

        stateTimer = player.counterAttackDuration;  //����״̬��ʱ��Ϊ��ҵķ�������ʱ��
        player.anim.SetBool("SuccessfulCounterAttack", false);  //��ʼ������Ϊfalse

        canCreateClone = true;  //ȷ��ÿ�ν��뷴��״̬ʱ����Ҷ��ܴ�����¡��
    }

 
    public override void Exit()
    {
        base.Exit(); 
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.currentState != this)
        {
            return;     //�����ǰ״̬���Ƿ���״̬����ֱ�ӷ���
        }

        player.SetZeroVelocity();   //��������ٶ�Ϊ��

        //�����ҹ�����Χ�ڵ�������ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            //�����⵽���Ǽ�ʸ����������ת��ʸ
            if (hit.GetComponent<Arrow_Controller>() != null)
            {
                hit.GetComponent<Arrow_Controller>().FlipArrow();
                SuccessfulCounterAttack(); //���÷�������
            }

            //�����⵽���ǵ���
            if (hit.GetComponent<Enemy>() != null)
            {
                Enemy enemy = hit.GetComponent<Enemy>();

                //������˿��Ա���������
                if (enemy.CanBeStunnedByCounterAttack())
                {
                    SuccessfulCounterAttack(); //���÷�������

                    //�����ָ�����ֵ/����ֵ
                    player.skill.parry.RecoverHPFPInSuccessfulParry();

                    //������Դ�����¡��
                    if (canCreateClone)
                    {
                        //�ڵ��˺󷽴�����¡�岢��������
                        player.skill.parry.MakeMirageInSuccessfulParry(new Vector3(enemy.transform.position.x - 1.5f * enemy.facingDirection, enemy.transform.position.y));
                        canCreateClone = false;  //ÿ�η���ֻ�ܴ���һ����¡��
                    }
                }
            }
        }

        //���״̬��ʱ��С��0�򴥷��˴����������л�������״̬
        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    //�ɹ�����ʱ�ķ���
    private void SuccessfulCounterAttack()
    {
        //����״̬��ʱ��Ϊһ����ֵ����Ϊ����ɹ�������״̬��ͨ�� triggerCalled �˳�
        stateTimer = 10;

        player.anim.SetBool("SuccessfulCounterAttack", true); //���ö���Ϊ true
        player.fx.ScreenShake(player.fx.shakeDirection_medium); //��Ļ��Ч��
    }
}
