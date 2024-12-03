using System.Collections;
using System.Collections.Generic;
using UnityEngine;
                                
                                         //强制扣血范围，为了让角色在新手教程加血更直观
public class WoundZone : MapElement
{
    private bool hasDamagedPlayer = false;  //记录玩家是否已经受到伤害

    //当玩家进入该区域时触发
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //确保只有一次伤害，避免重复伤害
        if (!hasDamagedPlayer)
        {
            //如果碰撞物体是玩家
            if (collision.GetComponent<Player>() != null)
            {
                //设置伤害值，默认是30
                int damage = 30;

                //如果玩家当前的HP低于30，确保伤害后玩家不会死亡，保留1点HP
                if (PlayerManager.instance.player.stats.currentHP < 30)
                {
                    damage = PlayerManager.instance.player.stats.currentHP - 1;  // 伤害等于当前HP - 1
                }

                //调用玩家的受伤害方法进行伤害处理
                collision.GetComponent<PlayerStats>()?.TakeDamage(damage, transform, collision.transform, false);

                //设置玩家已受到伤害，避免多次伤害
                hasDamagedPlayer = true;

                //将当前地图元素的ID加入已使用的地图元素列表
                GameManager.instance.UsedMapElementIDList.Add(mapElementID);
            }
        }
    }
}
