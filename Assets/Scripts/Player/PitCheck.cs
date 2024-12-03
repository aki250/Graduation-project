using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制玩家接近深坑时摄像机的变化
public class PitCheck : MonoBehaviour
{
    private Player player; 
    private CinemachineVirtualCamera cm;  //Cinemachine虚拟摄像机对象

    private Transform followTarget;  //当前摄像机跟随的目标

    private void Start()
    {
        player = PlayerManager.instance.player;  //获取玩家实例

        cm = CameraManager.instance.cm;  //获取虚拟摄像机实例
        followTarget = player.transform;  //默认为玩家的Transform作为摄像机跟随目标
    }

    private void Update()
    {
        cm.Follow = followTarget;  //设置摄像机的跟随目标为当前的目标（玩家或深坑）

        CameraMovementOnPit();  //检查是否处于深坑并调整摄像机缩放
    }

    //根据玩家是否接近深坑来调整摄像机的正交视图大小（缩放效果）
    private void CameraMovementOnPit()
    {
        //如果玩家接近深坑，逐步增加摄像机的缩放
        if (player.isNearPit)
        {
            // 如果摄像机的正交大小小于目标大小，则渐变增加
            if (cm.m_Lens.OrthographicSize < CameraManager.instance.targetCameraLensSize)
            {
                cm.m_Lens.OrthographicSize = Mathf.Lerp(
                    cm.m_Lens.OrthographicSize,
                    CameraManager.instance.targetCameraLensSize,
                    CameraManager.instance.cameraLensSizeChangeSpeed * Time.deltaTime
                );

                // 确保摄像机的缩放不会超过目标值
                if (cm.m_Lens.OrthographicSize >= CameraManager.instance.targetCameraLensSize - 0.01f)
                {
                    cm.m_Lens.OrthographicSize = CameraManager.instance.targetCameraLensSize;
                }
            }
        }
        else
        {
            // 如果玩家站在平台上，则不影响摄像机的调整
            if (player.isOnPlatform)
            {
                return;
            }

            //如果玩家不在深坑并且摄像机的正交大小大于默认值，逐步恢复为默认缩放
            if (cm.m_Lens.OrthographicSize > CameraManager.instance.defaultCameraLensSize)
            {
                cm.m_Lens.OrthographicSize = Mathf.Lerp(
                    cm.m_Lens.OrthographicSize,
                    CameraManager.instance.defaultCameraLensSize,
                    CameraManager.instance.cameraLensSizeChangeSpeed * Time.deltaTime
                );

                //确保摄像机的缩放不会小于默认值
                if (cm.m_Lens.OrthographicSize <= CameraManager.instance.defaultCameraLensSize + 0.01f)
                {
                    cm.m_Lens.OrthographicSize = CameraManager.instance.defaultCameraLensSize;
                }
            }
        }
    }

    //当玩家进入深坑触发器，设置玩家接近深坑并改变摄像机的跟随目标为深坑
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Pit>())  //判断碰撞对象是否为Pit类型
        {
            player.isNearPit = true;  //玩家进入深坑区域

            followTarget = collision.transform;  //改变摄像机跟随深坑
        }
    }

    //当玩家离开深坑触发器，设置摄像头不再接近深坑，并恢复跟随玩家
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Pit>())  //判断碰撞对象是否为Pit类型
        {
            player.isNearPit = false;  //玩家离开深坑区域

            followTarget = player.transform;  //恢复摄像机跟随玩家
        }
    }
}
