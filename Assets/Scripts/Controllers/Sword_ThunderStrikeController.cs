using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_ThunderStrikeController : MonoBehaviour
{
    //����ײ����봥����ʱ���ô˷���
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //��鴥������ײ���������Ƿ��ǵ���
        if (collision.GetComponent<Enemy>() != null)
        {
            //��ȡ��ҵ�ͳ������
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

            //��ȡ���˵�ͳ������
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            //������ҵ�ħ���˺��������Ե�������˺�
            //transform�ǵ�ǰ���������任��Ϣ�����ݸ��˺����������˺�����
            playerStats.DoMagicDamage(enemyStats, transform);
        }
    }

}
