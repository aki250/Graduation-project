using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                //��ͼ��Ǯ��
public class CurrencyEarningZone : MapElement
{
    //�Ƿ��Ѿ�����ҷ��Ż���
    private bool hasGivenPlayerCurrency = false;

    //����ҽ��봥����ʱ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //����Ѿ����Ź����ң����ٷ���
        if (!hasGivenPlayerCurrency)
        {
            //�����ײ�����Ƿ������
            if (collision.GetComponent<Player>() != null)
            {
                //������һ�������
                PlayerManager.instance.currency += 1000;

                //����Ѿ����Ż���
                hasGivenPlayerCurrency = true;

                //����ǰ��ͼԪ�ص�ID������ʹ�õ�ͼԪ��ID�б�
                GameManager.instance.UsedMapElementIDList.Add(mapElementID);
            }
        }
    }
}
