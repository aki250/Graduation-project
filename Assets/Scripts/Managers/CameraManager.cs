using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineVirtualCamera cm;

    [Header("摄像头焦距")]
    public float defaultCameraLensSize;
    public float targetCameraLensSize;
    public float cameraLensSizeChangeSpeed;

    [Header("摄像机Y位置")]
    public float defaultCameraYPosition;
    public float targetCameraYPosition;
    public float cameraYPositionChangeSpeed;

    private Player player;
    public CinemachineFramingTransposer ft { get; set; }    //摄像机框架调整
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
        //获取Cinemachine组件
        ft = cm.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
    }

    //玩家站在可下降平台时，，，摄像机的变化
    public void CameraMovementOnDownablePlatform()
    {
        //玩家站在平台上isOnPlatform，则摄像机变化
        if (player.isOnPlatform)
        {
            //摄像机的焦距小于目标焦距，则逐渐增大焦距
            if (cm.m_Lens.OrthographicSize < targetCameraLensSize)
            {
                //使用Mathf.Lerp来平滑地改变焦距大小
                cm.m_Lens.OrthographicSize = Mathf.Lerp(cm.m_Lens.OrthographicSize, targetCameraLensSize, cameraLensSizeChangeSpeed * Time.deltaTime);

                //确保焦距不会超出目标焦距
                if (cm.m_Lens.OrthographicSize >= targetCameraLensSize - 0.01f)
                {
                    cm.m_Lens.OrthographicSize = targetCameraLensSize;
                }
            }

            //如果摄像机的Y位置大于目标位置，则逐渐减小Y位置
            if (ft.m_ScreenY > targetCameraYPosition)
            {
                //使用Mathf.Lerp来平滑地改变Y位置
                ft.m_ScreenY = Mathf.Lerp(ft.m_ScreenY, targetCameraYPosition, cameraYPositionChangeSpeed * Time.deltaTime);

                //确保Y位置不会超出目标位置
                if (ft.m_ScreenY >= targetCameraYPosition + 0.01f)
                {
                    ft.m_ScreenY = targetCameraYPosition;
                }
            }
        }
        //玩家不在平台上，则执行恢复到默认摄像机状态的代码
        else
        {
            //玩家接近坑洞，不改变摄像机的位置
            if (player.isNearPit)
            {
                return;  
            }

            //摄像机焦距大于默认焦距，则逐渐减小焦距
            if (cm.m_Lens.OrthographicSize > defaultCameraLensSize)
            {
                //使用Mathf.Lerp来平滑地改变焦距大小
                cm.m_Lens.OrthographicSize = Mathf.Lerp(cm.m_Lens.OrthographicSize, defaultCameraLensSize, cameraLensSizeChangeSpeed * Time.deltaTime);

                //确保焦距不会超出默认焦距
                if (cm.m_Lens.OrthographicSize <= defaultCameraLensSize + 0.01f)
                {
                    cm.m_Lens.OrthographicSize = defaultCameraLensSize;
                }
            }

            //摄像机的Y位置小于默认位置，则逐渐增大Y位置
            if (ft.m_ScreenY < defaultCameraYPosition)
            {
                //使用Mathf.Lerp来平滑地改变Y位置
                ft.m_ScreenY = Mathf.Lerp(ft.m_ScreenY, defaultCameraYPosition, cameraYPositionChangeSpeed * Time.deltaTime);

                //确保Y位置不会超出默认位置
                if (ft.m_ScreenY <= targetCameraYPosition - 0.01f)
                {
                    ft.m_ScreenY = targetCameraYPosition;
                }
            }
        }
    }

}
