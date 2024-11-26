using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownablePlatformCheck : MonoBehaviour
{
    private Player player; 

    private void Start()
    {
        player = PlayerManager.instance.player; //ͨ��PlayerManager����
    }

    private void Update()
    {
        // ÿһ֡����CameraManager��CameraMovementOnDownablePlatform������������������ƶ�
        //���վ�ڿ�����ƽ̨�ϵ�ʱ�����������Ϊ�����磬������ң�
        CameraManager.instance.CameraMovementOnDownablePlatform();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �����ײ������һ��DownablePlatform
        if (collision.gameObject.GetComponent<DownablePlatform>() != null)
        {
            // ������ҵ�lastPlatformΪ��ǰ��ײ����DownablePlatform����
            // ������ҿ���֪���Լ�վ���ĸ�ƽ̨��
            player.lastPlatform = collision.gameObject.GetComponent<DownablePlatform>();

            player.isOnPlatform = true; // ��isOnPlatform���Ϊtrue����ʾ���վ����ƽ̨��
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �����ײ������һ��DownablePlatform��������ƽ̨��
        if (collision.gameObject.GetComponent<DownablePlatform>() != null)
        {
            // ������ҵ�lastPlatformΪ��ǰ��ײ����DownablePlatform����
            // ��isOnPlatform���Ϊfalse����ʾ��Ҳ���վ��ƽ̨��
            player.lastPlatform = collision.gameObject.GetComponent<DownablePlatform>();
            player.isOnPlatform = false;
        }
    }
}
