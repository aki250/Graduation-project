using UnityEngine;

public class ShadyStunnedState : ShadyState
{
    private float moveTimer;

    public ShadyStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Shady _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //����ѣ��ʱ��ɫ��˸
        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        //ѣ��״̬����ʱ��
        stateTimer = enemy.stunDuration;
        //���Ƶ����ƶ���ʱ����ѣ��״̬�µ��˵��ƶ�ʱ��ܶ�
        moveTimer = 0.1f;

        // ���õ��˵��ٶȣ�ʹ��������õ�ѣ���ƶ�������ٶȻ���
        // ͨ�� `-enemy.facingDirection` ����ת���򣨵��˿��ܳ�����ң�
        rb.velocity = new Vector2(enemy.stunMovement.x * -enemy.facingDirection, enemy.stunMovement.y);
    }

    public override void Exit()
    {
        base.Exit(); 

        //ȡ��ѣ��ʱ��˸Ч��
        enemy.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();  

        //����moveTimer�����Ƶ��˵�ѣ�λ���ʱ��
        moveTimer -= Time.deltaTime;

        //С��0����ֹͣ���˵�ˮƽ�ƶ�
        if (moveTimer < 0)
        {
            //ֻ������ֱ�����ٶ�
            enemy.SetVelocity(0, rb.velocity.y);
        }

        // ѣ��ʱ��������л��ؿ���״̬
        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
