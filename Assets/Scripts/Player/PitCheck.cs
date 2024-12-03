using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ҽӽ����ʱ������ı仯
public class PitCheck : MonoBehaviour
{
    private Player player; 
    private CinemachineVirtualCamera cm;  //Cinemachine�������������

    private Transform followTarget;  //��ǰ����������Ŀ��

    private void Start()
    {
        player = PlayerManager.instance.player;  //��ȡ���ʵ��

        cm = CameraManager.instance.cm;  //��ȡ���������ʵ��
        followTarget = player.transform;  //Ĭ��Ϊ��ҵ�Transform��Ϊ���������Ŀ��
    }

    private void Update()
    {
        cm.Follow = followTarget;  //����������ĸ���Ŀ��Ϊ��ǰ��Ŀ�꣨��һ���ӣ�

        CameraMovementOnPit();  //����Ƿ�����Ӳ��������������
    }

    //��������Ƿ�ӽ�����������������������ͼ��С������Ч����
    private void CameraMovementOnPit()
    {
        //�����ҽӽ���ӣ������������������
        if (player.isNearPit)
        {
            // ����������������СС��Ŀ���С���򽥱�����
            if (cm.m_Lens.OrthographicSize < CameraManager.instance.targetCameraLensSize)
            {
                cm.m_Lens.OrthographicSize = Mathf.Lerp(
                    cm.m_Lens.OrthographicSize,
                    CameraManager.instance.targetCameraLensSize,
                    CameraManager.instance.cameraLensSizeChangeSpeed * Time.deltaTime
                );

                // ȷ������������Ų��ᳬ��Ŀ��ֵ
                if (cm.m_Lens.OrthographicSize >= CameraManager.instance.targetCameraLensSize - 0.01f)
                {
                    cm.m_Lens.OrthographicSize = CameraManager.instance.targetCameraLensSize;
                }
            }
        }
        else
        {
            // ������վ��ƽ̨�ϣ���Ӱ��������ĵ���
            if (player.isOnPlatform)
            {
                return;
            }

            //�����Ҳ�����Ӳ����������������С����Ĭ��ֵ���𲽻ָ�ΪĬ������
            if (cm.m_Lens.OrthographicSize > CameraManager.instance.defaultCameraLensSize)
            {
                cm.m_Lens.OrthographicSize = Mathf.Lerp(
                    cm.m_Lens.OrthographicSize,
                    CameraManager.instance.defaultCameraLensSize,
                    CameraManager.instance.cameraLensSizeChangeSpeed * Time.deltaTime
                );

                //ȷ������������Ų���С��Ĭ��ֵ
                if (cm.m_Lens.OrthographicSize <= CameraManager.instance.defaultCameraLensSize + 0.01f)
                {
                    cm.m_Lens.OrthographicSize = CameraManager.instance.defaultCameraLensSize;
                }
            }
        }
    }

    //����ҽ�����Ӵ�������������ҽӽ���Ӳ��ı�������ĸ���Ŀ��Ϊ���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Pit>())  //�ж���ײ�����Ƿ�ΪPit����
        {
            player.isNearPit = true;  //��ҽ����������

            followTarget = collision.transform;  //�ı�������������
        }
    }

    //������뿪��Ӵ���������������ͷ���ٽӽ���ӣ����ָ��������
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Pit>())  //�ж���ײ�����Ƿ�ΪPit����
        {
            player.isNearPit = false;  //����뿪�������

            followTarget = player.transform;  //�ָ�������������
        }
    }
}
