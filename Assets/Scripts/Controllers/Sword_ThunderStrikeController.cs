using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_ThunderStrikeController : MonoBehaviour
{
    //当碰撞体进入触发器时调用此方法
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //检查触发器碰撞到的物体是否是敌人
        if (collision.GetComponent<Enemy>() != null)
        {
            //获取玩家的统计数据
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

            //获取敌人的统计数据
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            //调用玩家的魔法伤害方法，对敌人造成伤害
            //transform是当前物体的世界变换信息，传递给伤害函数用于伤害计算
            playerStats.DoMagicDamage(enemyStats, transform);
        }
    }

}
