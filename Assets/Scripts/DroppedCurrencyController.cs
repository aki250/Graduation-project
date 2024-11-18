using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCurrencyController : MonoBehaviour
{
    public int droppedCurrency;    //�洢���ҵ��䡣

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�����봥��������Ϸ�����Ƿ���Player�����
        if (collision.GetComponent<Player>() != null)
        {
            //����PlayerManager������currency���ԣ�����������������ҵ�������droppedCurrencyֵ��
            PlayerManager.instance.currency += droppedCurrency;

            Destroy(gameObject);    //����������ҵ������Ϸ������Ϊ���Ѿ������ʰȡ��
        }
    }
}