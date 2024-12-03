using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    //播放指定区域的音效
    [SerializeField] private int areaSoundIndex;

    //用于保存渐变停止音效的协程，以便在音效恢复时停止该协程
    private Coroutine stopSFXGradually;

    //当玩家进入触发区域时调用
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //检查碰撞的物体是否为玩家
        if (collision.GetComponent<Player>() != null)
        {
            //如果有正在执行的渐变停止协程，则停止该协程，防止音效渐隐
            if (stopSFXGradually != null)
            {
                StopCoroutine(stopSFXGradually);
            }

            //复音效的音量至1
            AudioManager.instance.sfx[areaSoundIndex].volume = 1;
            //播放指定的音效，无需传入具体位置
            AudioManager.instance.PlaySFX(areaSoundIndex, null);
        }
    }

    //当玩家离开触发区域时调用
    private void OnTriggerExit2D(Collider2D collision)
    {
        //如果当前对象被禁用，则直接返回，避免冗余操作
        if (!gameObject.activeSelf)
        {
            return;
        }

        //检查碰撞的物体是否为玩家
        if (collision.GetComponent<Player>() != null)
        {
            //AudioManager.instance.StopSFXGradually(areaSoundIndex);
            //渐变停止音效的协程，使音效逐渐降低直至停止
            stopSFXGradually = StartCoroutine(AudioManager.instance.DecreaseVolumeGradually(AudioManager.instance.sfx[areaSoundIndex]));
        }
    }
}
