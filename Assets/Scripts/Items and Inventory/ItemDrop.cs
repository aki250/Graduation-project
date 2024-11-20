using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    //最大掉落物品数量
    [SerializeField] private int maxItemDropAmount;

    //可能掉落的物品列表
    [SerializeField] private ItemData[] possibleDropItemList;

    //实际掉落的物品列表
    private List<ItemData> actualDropList = new List<ItemData>();

    //掉落物品的预制件，掉落时会根据设置的物品类型来初始化
    [SerializeField] private GameObject dropItemPrefab;


    //生成掉落物品
    public virtual void GenrateDrop()
    {
        //根据掉落几率将物品添加到实际掉落列表中
        for (int i = 0; i < possibleDropItemList.Length; i++)
        {
            //随机生成一个数，如果该数小于等于物品的掉落概率，就将该物品添加到掉落列表中
            if (Random.Range(0, 100) <= possibleDropItemList[i].dropChance)
            {
                actualDropList.Add(possibleDropItemList[i]);
            }
        }

        //从实际掉落列表中随机选择物品进行掉落，并从列表中移除已掉落的物品
        for (int i = 0; i < maxItemDropAmount && actualDropList.Count > 0; i++)
        {
            //随机选择一个物品进行掉落
            ItemData itemToDrop = actualDropList[Random.Range(0, actualDropList.Count - 1)];

            //从掉落列表中移除已掉落的物品
            actualDropList.Remove(itemToDrop);

            //掉落物品
            DropItem(itemToDrop);
        }
    }

    //当敌人死亡时调用此方法
    protected void DropItem(ItemData _itemToDrop)
    {
        //创建新的掉落物品实例，位置在敌人的当前位置
        GameObject newDropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);

        //掉落物品的初速度，模拟物品掉落的效果
        Vector2 dropVelocity = new Vector2(Random.Range(-5, 5), Random.Range(12, 15));

        //掉落物品的名称和图标
        newDropItem.GetComponent<ItemObject>()?.SetupItemDrop(_itemToDrop, dropVelocity);
    }
}
