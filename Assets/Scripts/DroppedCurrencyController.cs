using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedCurrencyController : MonoBehaviour
{
    public int droppedCurrency;    //存储货币掉落。

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //检查进入触发器的游戏对象是否有Player组件。
        if (collision.GetComponent<Player>() != null)
        {
            //增加PlayerManager单例的currency属性，增加数量是这个货币掉落对象的droppedCurrency值。
            PlayerManager.instance.currency += droppedCurrency;

            Destroy(gameObject);    //销毁这个货币掉落的游戏对象，因为它已经被玩家拾取。
        }
    }
}