using UnityEngine;

                                                                         //����ͷźڶ����ܵ�״̬
public class PlayerReleaseBlackholeSkillState : PlayerState
{
    //����ʱ�䣬��������ͷźڶ�����ʱ������ʱ��
    private float flyTime = 0.4f;
    //�����Ƿ���ʹ�õı�־
    private bool skillUsed;
    //ԭʼ����ֵ��������״̬����ʱ�ָ�����
    private float originalGravity;

    public PlayerReleaseBlackholeSkillState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    //�������ͷźڶ�����״̬ʱ����
    public override void Enter()
    {
        base.Enter();

        //����ԭʼ����ֵ
        originalGravity = rb.gravityScale;

        //���ü���ʹ�ñ�־
        skillUsed = false;
        //����״̬��ʱ��Ϊ����ʱ��
        stateTimer = flyTime;
        //��������Ϊ 0��ʹ�������
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        //�ָ�ԭʼ����ֵ
        rb.gravityScale = originalGravity;
        //ʹ���ʵ�岻͸�������ָ�������ʾ
        player.fx.MakeEntityTransparent(false);
    }

    public override void Update()
    {
        base.Update(); 

        if (stateMachine.currentState != player.blackholeSkillState)
        {
            return;
        }

        //������������������СС�ڻ���� 10����������������С
        if (Camera.main.orthographicSize <= 10)
        {
            Camera.main.orthographicSize += 0.1f;
        }

        //���״̬��ʱ��>0��ʹ�������
        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 15);
        }

        //���״̬��ʱ��<0��ʹ��һ����½�
        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -0.1f);

            //���������δʹ��
            if (!skillUsed)
            {
                //����ڶ����ܿ��ò�ʹ�óɹ��������ü���ʹ�ñ�־Ϊtrue
                if (player.skill.blackhole.UseSkillIfAvailable())
                {
                    skillUsed = true;
                }
            }

            // ��������˳��ڶ�����״̬
            if (player.skill.blackhole.CanExitBlackholeSkill())
            {
                // �л�������״̬
                player.stateMachine.ChangeState(player.airState);
            }
        }
    }
}