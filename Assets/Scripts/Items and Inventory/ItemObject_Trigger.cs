using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject;

    private void Awake()
    {
        //获取游戏对象的父对象上的ItemObject组件。
        myItemObject = GetComponentInParent<ItemObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果玩家已死亡，则不执行任何操作。
        if (PlayerManager.instance.player.stats.isDead)
        {
            return;
        }

        //检查进入触发器的游戏对象是否有Player组件。
        if (collision.GetComponent<Player>() != null)
        {
            myItemObject.PickupItem();  //调用myItemObject的PickupItem方法，执行拾取物品的操作。
        }
    }
}