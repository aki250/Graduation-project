using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    //玩家掉落的设置
    [Header("玩家掉落")]
    [SerializeField] private float chanceToDropEquiment; //掉落装备的概率
    [SerializeField] private float chanceToDropMaterials; //掉落材料的概率

    //重写掉落生成方法
    public override void GenrateDrop()
    {
        //获取玩家当前装备的物品和背包中的物品列表
        Inventory inventory = Inventory.instance;

        //获取当前装备的物品列表
        List<InventorySlot> currentEquippedEquipment = inventory.GetEquippedEquipmentList();
        List<InventorySlot> EquipmentToLose = new List<InventorySlot>(); //存放将要掉落的装备

        //背包中的物品列表
        List<InventorySlot> currentStash = inventory.GetStashList();
        List<InventorySlot> materialsToLose = new List<InventorySlot>(); //存放将要掉落的材料

        //遍历当前装备的物品，按照掉落概率判断是否掉落
        for (int i = 0; i < currentEquippedEquipment.Count; i++)
        {
            //随机产生一个0到100之间的数，如果该数小于等于掉落装备的概率，则掉落该装备
            if (Random.Range(0, 100) <= chanceToDropEquiment)
            {
                EquipmentToLose.Add(currentEquippedEquipment[i]); //将要掉落的装备添加到列表
            }
        }

        //遍历背包中的物品，按照掉落概率判断是否掉落
        for (int i = 0; i < currentStash.Count; i++)
        {
            //随机产生一个0到100之间的数，如果该数小于等于掉落材料的概率，则掉落该材料
            if (Random.Range(0, 100) <= chanceToDropMaterials)
            {
                materialsToLose.Add(currentStash[i]); //将要掉落的材料添加到列表
            }
        }

        //对于掉落的装备，卸下装备并将装备丢弃
        for (int i = 0; i < EquipmentToLose.Count; i++)
        {
            //卸下装备，且不将其重新放回背包
            inventory.UnequipEquipmentWithoutAddingBackToInventory(EquipmentToLose[i].item as ItemData_Equipment);
            //丢弃装备物品
            DropItem(EquipmentToLose[i].item);
        }

        //对于掉落的材料，丢弃并从背包中移除
        for (int i = 0; i < materialsToLose.Count; i++)
        {
            DropItem(materialsToLose[i].item);  //丢弃材料物品

            inventory.RemoveItem(materialsToLose[i].item);  //从背包中移除材料
        }
    }
}

