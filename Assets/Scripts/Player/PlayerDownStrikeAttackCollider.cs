using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                //����»�������ײ���࣬����������»�״̬ʱ����˷�������ײ����������Ӧ�Ĺ����߼�����
public class PlayerDownStrikeAttackCollider : MonoBehaviour
{
    private Player player;  //��ȡ��ҵ�״̬����Ϊ

    private float downStrikAttackCooldown = 10f;  //�»���������ȴʱ��
    private float downStrikeAttackTimer;  //��ʱ�������ڿ�����ȴʱ��

    //�ڽű���ʼʱ��ȡ��Ҷ���
    private void Start()
    {
        player = PlayerManager.instance.player;  //��ȡ��Ҷ���ͨ�� PlayerManager ������
    }

    private void Update()
    {
        downStrikeAttackTimer -= Time.deltaTime;  //ÿ֡������ȴʱ��

        //�������ѽӴ����棬���õ�ǰ���壨�����ǹ�����ײ�壩
        if (player.IsGroundDetected())
        {
            gameObject.SetActive(false);
        }
    }

    //���ü�ʱ��
    private void OnEnable()
    {
        downStrikeAttackTimer = 0;  //����ʱ����ȴ��ʱ������
    }

    //����ײ�����������巢�������Ӵ�ʱ����
    private void OnTriggerStay2D(Collider2D collision)
    {
        //�����ҵ�ǰ�����»�״̬
        if (player.stateMachine.currentState == player.downStrikeState)
        {
            // �����ײ���Ƿ�����˷�����ײ
            if (collision.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.GetComponent<Enemy>();  //��ȡ�������

                //����ԭʼ�ĵ��˻��������������ڹ�����ָ���
                Vector2 originalEnemyKnockbackMovement = enemy.knockbackMovement;
                //���õ����µĻ���������ʹ������ҵĴ�ֱ�ٶȣ�Y���ٶȣ�����һ��
                enemy.knockbackMovement = new Vector2(0, player.rb.velocity.y);

                // ���õ����µ��ٶȣ�ˮƽ�봹ֱ��������ҵ��ٶȣ�
                enemy.SetVelocity(player.rb.velocity.x, player.rb.velocity.y);

                //�����ȴʱ���ѹ��������˺�����
                if (downStrikeAttackTimer < 0)
                {
                    player.stats.DoDamge(enemy.stats);  //��ҶԵ�������˺�
                    downStrikeAttackTimer = downStrikAttackCooldown;  //������ȴ��ʱ��
                }

                //�ָ����˵Ļ�������,Ϊ�˺ÿ��㣬�Ӿͼ���
                enemy.knockbackMovement = originalEnemyKnockbackMovement;
            }
        }
    }

    //����ײ����������������Ӵ�ʱ����
    private void OnTriggerExit2D(Collider2D collision)
    {
        //�����ҵ�ǰ�����»�״̬
        if (player.stateMachine.currentState == player.downStrikeState)
        {
            //�����ײ���Ƿ�����˷�����ײ
            if (collision.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.GetComponent<Enemy>();  //��ȡ�������

                //ֱ�����õ��˵��ٶȣ����ٿ��ǻ��ˣ�
                enemy.SetVelocity(player.rb.velocity.x, player.rb.velocity.y);

                //��ҶԵ�������˺�
                player.stats.DoDamge(enemy.stats);
            }
        }
    }
}
