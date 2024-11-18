using System.Collections;
using System.Collections.Generic;
using UnityEngine;

                                                                //地图加钱！
public class CurrencyEarningZone : MapElement
{
    //是否已经给玩家发放货币
    private bool hasGivenPlayerCurrency = false;

    //当玩家进入触发区时
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果已经发放过货币，则不再发放
        if (!hasGivenPlayerCurrency)
        {
            //检查碰撞对象是否是玩家
            if (collision.GetComponent<Player>() != null)
            {
                //增加玩家货币数量
                PlayerManager.instance.currency += 1000;

                //标记已经发放货币
                hasGivenPlayerCurrency = true;

                //将当前地图元素的ID加入已使用地图元素ID列表
                GameManager.instance.UsedMapElementIDList.Add(mapElementID);
            }
        }
    }
}
