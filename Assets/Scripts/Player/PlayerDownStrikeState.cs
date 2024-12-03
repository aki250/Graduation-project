using UnityEngine;

//����»�״̬�࣬����������»�ʱ��״̬�߼�
public class PlayerDownStrikeState : PlayerState
{
    //�жϴ����»�����׶�
    public bool fallingStrikeTrigger { get; private set; } = false;

    //�жϴ��������׶�
    public bool animStopTrigger { get; private set; } = false;

    //�ж�����ֹͣ����
    private bool animStopTriggerHasBeenSet = false;

    //�ж�������Ļ��
    private bool screenShakeHasBeenSet = false;

    //��ʼ���»�״̬
    public PlayerDownStrikeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //������ҵĳ�ʼ�ٶȣ�ģ���������
        player.SetVelocity(0, -10);

        //�����»�״̬��صĴ�����
        fallingStrikeTrigger = false;
        animStopTrigger = false;
        animStopTriggerHasBeenSet = false;
        screenShakeHasBeenSet = false;

        // ����״̬��ʱ��Ϊ���޴󣬱�ʾ���״̬�����Զ�������ֱ���ֶ�����
        stateTimer = Mathf.Infinity;
    }

    public override void Exit()
    {
        base.Exit(); 

        // �ָ����������ٶ�
        player.anim.speed = 1;
    }

    public override void Update()
    {
        base.Update(); 

        //��ǰ״̬�����»�״̬��ֱ�ӷ���
        if (stateMachine.currentState != this)
        {
            return;
        }

        //����Ȼ���΢��������Ϊ�»�������
        if (!fallingStrikeTrigger)
        {
            player.SetVelocity(0, 1.2f);  //��������߶�
        }
        else
        {
            //��ҿ�ʼ�����»��������ٶ�����
            player.SetVelocity(0, -17);

            //������ȫ�����׼����������ʱ��ֹͣ����
            if (animStopTrigger && !animStopTriggerHasBeenSet)
            {
                player.anim.speed = 0;
                animStopTriggerHasBeenSet = true;  //������ֹͣ������־
            }

            //����Ҽ�⵽����ʱ���ָ���������
            if (player.IsGroundDetected())
            {
                player.anim.speed = 1;  // �ָ���������

                //�����Ļ�𶯻�δ���ã���ִ����Ļ�𶯺ͳ�����Ч
                if (!screenShakeHasBeenSet)
                {
                    player.fx.ScreenShake(player.fx.shakeDirection_medium);  // ������Ļ����Ч
                    player.fx.PlayDownStrikeDustFX();  // �����»�������Ч
                    screenShakeHasBeenSet = true;  // ��������Ч��ִ��
                }
            }

            //�����������л���վ��
            if (triggerCalled)
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    //�������乥���׶�
    public void SetFallingStrikeTrigger()
    {
        fallingStrikeTrigger = true;
    }

    //��������ֹͣ�׶�
    public void SetAnimStopTrigger()
    {
        animStopTrigger = true;
    }
}
