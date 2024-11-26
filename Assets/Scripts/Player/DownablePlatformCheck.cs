using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownablePlatformCheck : MonoBehaviour
{
    private Player player; 

    private void Start()
    {
        player = PlayerManager.instance.player; //通过PlayerManager管理
    }

    private void Update()
    {
        // 每一帧调用CameraManager的CameraMovementOnDownablePlatform方法，处理摄像机的移动
        //玩家站在可下落平台上的时候，摄像机的行为（例如，跟随玩家）
        CameraManager.instance.CameraMovementOnDownablePlatform();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰撞物体是一个DownablePlatform
        if (collision.gameObject.GetComponent<DownablePlatform>() != null)
        {
            // 设置玩家的lastPlatform为当前碰撞到的DownablePlatform对象
            // 这样玩家可以知道自己站在哪个平台上
            player.lastPlatform = collision.gameObject.GetComponent<DownablePlatform>();

            player.isOnPlatform = true; // 将isOnPlatform标记为true，表示玩家站在了平台上
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 如果碰撞物体是一个DownablePlatform（可下落平台）
        if (collision.gameObject.GetComponent<DownablePlatform>() != null)
        {
            // 重置玩家的lastPlatform为当前碰撞到的DownablePlatform对象
            // 将isOnPlatform标记为false，表示玩家不再站在平台上
            player.lastPlatform = collision.gameObject.GetComponent<DownablePlatform>();
            player.isOnPlatform = false;
        }
    }
}
