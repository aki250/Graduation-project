using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineVirtualCamera cm;

    [Header("����ͷ����")]
    public float defaultCameraLensSize;
    public float targetCameraLensSize;
    public float cameraLensSizeChangeSpeed;

    [Header("�����Yλ��")]
    public float defaultCameraYPosition;
    public float targetCameraYPosition;
    public float cameraYPositionChangeSpeed;

    private Player player;
    public CinemachineFramingTransposer ft { get; set; }    //�������ܵ���
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;  
        }
        else
        {
            Destroy(gameObject);  
        }
        //��ȡCinemachine���
        ft = cm.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
    }

    //���վ�ڿ��½�ƽ̨ʱ������������ı仯
    public void CameraMovementOnDownablePlatform()
    {
        //���վ��ƽ̨��isOnPlatform����������仯
        if (player.isOnPlatform)
        {
            //������Ľ���С��Ŀ�꽹�࣬�������󽹾�
            if (cm.m_Lens.OrthographicSize < targetCameraLensSize)
            {
                //ʹ��Mathf.Lerp��ƽ���ظı佹���С
                cm.m_Lens.OrthographicSize = Mathf.Lerp(cm.m_Lens.OrthographicSize, targetCameraLensSize, cameraLensSizeChangeSpeed * Time.deltaTime);

                //ȷ�����಻�ᳬ��Ŀ�꽹��
                if (cm.m_Lens.OrthographicSize >= targetCameraLensSize - 0.01f)
                {
                    cm.m_Lens.OrthographicSize = targetCameraLensSize;
                }
            }

            //����������Yλ�ô���Ŀ��λ�ã����𽥼�СYλ��
            if (ft.m_ScreenY > targetCameraYPosition)
            {
                //ʹ��Mathf.Lerp��ƽ���ظı�Yλ��
                ft.m_ScreenY = Mathf.Lerp(ft.m_ScreenY, targetCameraYPosition, cameraYPositionChangeSpeed * Time.deltaTime);

                //ȷ��Yλ�ò��ᳬ��Ŀ��λ��
                if (ft.m_ScreenY >= targetCameraYPosition + 0.01f)
                {
                    ft.m_ScreenY = targetCameraYPosition;
                }
            }
        }
        //��Ҳ���ƽ̨�ϣ���ִ�лָ���Ĭ�������״̬�Ĵ���
        else
        {
            //��ҽӽ��Ӷ������ı��������λ��
            if (player.isNearPit)
            {
                return;  
            }

            //������������Ĭ�Ͻ��࣬���𽥼�С����
            if (cm.m_Lens.OrthographicSize > defaultCameraLensSize)
            {
                //ʹ��Mathf.Lerp��ƽ���ظı佹���С
                cm.m_Lens.OrthographicSize = Mathf.Lerp(cm.m_Lens.OrthographicSize, defaultCameraLensSize, cameraLensSizeChangeSpeed * Time.deltaTime);

                //ȷ�����಻�ᳬ��Ĭ�Ͻ���
                if (cm.m_Lens.OrthographicSize <= defaultCameraLensSize + 0.01f)
                {
                    cm.m_Lens.OrthographicSize = defaultCameraLensSize;
                }
            }

            //�������Yλ��С��Ĭ��λ�ã���������Yλ��
            if (ft.m_ScreenY < defaultCameraYPosition)
            {
                //ʹ��Mathf.Lerp��ƽ���ظı�Yλ��
                ft.m_ScreenY = Mathf.Lerp(ft.m_ScreenY, defaultCameraYPosition, cameraYPositionChangeSpeed * Time.deltaTime);

                //ȷ��Yλ�ò��ᳬ��Ĭ��λ��
                if (ft.m_ScreenY <= targetCameraYPosition - 0.01f)
                {
                    ft.m_ScreenY = targetCameraYPosition;
                }
            }
        }
    }

}
