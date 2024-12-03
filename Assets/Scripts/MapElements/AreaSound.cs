using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    //����ָ���������Ч
    [SerializeField] private int areaSoundIndex;

    //���ڱ��潥��ֹͣ��Ч��Э�̣��Ա�����Ч�ָ�ʱֹͣ��Э��
    private Coroutine stopSFXGradually;

    //����ҽ��봥������ʱ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�����ײ�������Ƿ�Ϊ���
        if (collision.GetComponent<Player>() != null)
        {
            //���������ִ�еĽ���ֹͣЭ�̣���ֹͣ��Э�̣���ֹ��Ч����
            if (stopSFXGradually != null)
            {
                StopCoroutine(stopSFXGradually);
            }

            //����Ч��������1
            AudioManager.instance.sfx[areaSoundIndex].volume = 1;
            //����ָ������Ч�����贫�����λ��
            AudioManager.instance.PlaySFX(areaSoundIndex, null);
        }
    }

    //������뿪��������ʱ����
    private void OnTriggerExit2D(Collider2D collision)
    {
        //�����ǰ���󱻽��ã���ֱ�ӷ��أ������������
        if (!gameObject.activeSelf)
        {
            return;
        }

        //�����ײ�������Ƿ�Ϊ���
        if (collision.GetComponent<Player>() != null)
        {
            //AudioManager.instance.StopSFXGradually(areaSoundIndex);
            //����ֹͣ��Ч��Э�̣�ʹ��Ч�𽥽���ֱ��ֹͣ
            stopSFXGradually = StartCoroutine(AudioManager.instance.DecreaseVolumeGradually(AudioManager.instance.sfx[areaSoundIndex]));
        }
    }
}
