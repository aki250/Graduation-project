using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                
                                         //ǿ�ƿ�Ѫ��Χ��Ϊ���ý�ɫ�����ֽ̳̼�Ѫ��ֱ��
public class WoundZone : MapElement
{
    private bool hasDamagedPlayer = false;  //��¼����Ƿ��Ѿ��ܵ��˺�

    //����ҽ��������ʱ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ȷ��ֻ��һ���˺��������ظ��˺�
        if (!hasDamagedPlayer)
        {
            //�����ײ���������
            if (collision.GetComponent<Player>() != null)
            {
                //�����˺�ֵ��Ĭ����30
                int damage = 30;

                //�����ҵ�ǰ��HP����30��ȷ���˺�����Ҳ�������������1��HP
                if (PlayerManager.instance.player.stats.currentHP < 30)
                {
                    damage = PlayerManager.instance.player.stats.currentHP - 1;  // �˺����ڵ�ǰHP - 1
                }

                //������ҵ����˺����������˺�����
                collision.GetComponent<PlayerStats>()?.TakeDamage(damage, transform, collision.transform, false);

                //����������ܵ��˺����������˺�
                hasDamagedPlayer = true;

                //����ǰ��ͼԪ�ص�ID������ʹ�õĵ�ͼԪ���б�
                GameManager.instance.UsedMapElementIDList.Add(mapElementID);
            }
        }
    }
}
