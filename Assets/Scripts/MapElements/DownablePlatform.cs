using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownablePlatform : MonoBehaviour
{
    private Collider2D cd;  //存储平台的碰撞器

    private void Start()
    {
        //在Start方法中获取该平台的Collider2D组件
        cd = GetComponent<Collider2D>();
    }

    // 公共方法，用于在一段时间内关闭平台的碰撞器
    public void TurnOffPlatformColliderForTime(float _seconds)
    {
        //启动协程，使平台的碰撞器在指定时间后恢复
        StartCoroutine(TurnOffPlatformColliderForTime_Coroutine(_seconds));
    }

    //使用协程处理异步操作，在指定时间内禁用平台的碰撞器
    private IEnumerator TurnOffPlatformColliderForTime_Coroutine(float _seconds)
    {
        cd.enabled = false;  //禁止与其他物体碰撞
        yield return new WaitForSeconds(_seconds); 
        cd.enabled = true; 
    }
}
