using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownablePlatform : MonoBehaviour
{
    private Collider2D cd;  //�洢ƽ̨����ײ��

    private void Start()
    {
        //��Start�����л�ȡ��ƽ̨��Collider2D���
        cd = GetComponent<Collider2D>();
    }

    // ����������������һ��ʱ���ڹر�ƽ̨����ײ��
    public void TurnOffPlatformColliderForTime(float _seconds)
    {
        //����Э�̣�ʹƽ̨����ײ����ָ��ʱ���ָ�
        StartCoroutine(TurnOffPlatformColliderForTime_Coroutine(_seconds));
    }

    //ʹ��Э�̴����첽��������ָ��ʱ���ڽ���ƽ̨����ײ��
    private IEnumerator TurnOffPlatformColliderForTime_Coroutine(float _seconds)
    {
        cd.enabled = false;  //��ֹ������������ײ
        yield return new WaitForSeconds(_seconds); 
        cd.enabled = true; 
    }
}
