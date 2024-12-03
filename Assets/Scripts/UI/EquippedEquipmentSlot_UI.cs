using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class EquippedEquipmentSlot_UI : InventorySlot_UI
{
    //当前装备槽类型
    public EquipmentType equipmentType;

    //在Unity编辑器中，当该对象被选中时会自动调用,用于在编辑器中更新该对象的名字，便于区分不同类型的装备槽
    private void OnValidate()
    {
        // 更新GameObject的名字为装备类型
        gameObject.name = equipmentType.ToString();
    }

    // 当玩家点击装备槽时触发的事件
    public override void OnPointerDown(PointerEventData eventData)
    {
        // 当点击一个不可见的装备槽UI时，直接返回并不做任何处理
        //防止出现 Unity的bug，即使没有装备，Unity仍然认为装备槽是有物品的
        //如果出现这个bug要重启Unity
        if (inventorySlot == null || inventorySlot.item == null)
        {
            return;
        }

        //卸下装备并将其返回到背包中
        Inventory.instance.UnequipEquipmentWithoutAddingBackToInventory(inventorySlot.item as ItemData_Equipment);

        //将装备重新添加到背包
        Inventory.instance.AddItem(inventorySlot.item as ItemData_Equipment);

        //清理装备槽UI（更新装备槽的显示状态）
        CleanUpInventorySlotUI();  //这里冗余，因为在 AddItem() 中会调用 UpdateAllSlotUI() 更新槽UI

        //隐藏工具提示UI
        ui.itemToolTip.HideToolTip();
    }
}

