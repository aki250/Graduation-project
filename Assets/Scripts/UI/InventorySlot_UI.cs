using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot_UI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage; //显示物品图标Image组件
    [SerializeField] protected TextMeshProUGUI itemText; //显示物品数量或信息的TextMeshProUGUI组件

    public InventorySlot inventorySlot; //存储当前背包槽的物品信息，值由 UpdateInventroySlotUI 方法传入

    protected UI ui; //当前UI控件引用

    //初始化时获取父级 UI 控件的引用
    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
    }

    //更新背包槽的显示内容
    public void UpdateInventorySlotUI(InventorySlot _inventorySlot)
    {
        inventorySlot = _inventorySlot; //将传入的背包槽数据赋值给当前槽

        itemImage.color = Color.white; //重置物品图标的颜色为默认白色

        //检查物品数据
        if (inventorySlot != null)
        {
            //物品图标
            itemImage.sprite = inventorySlot.item.icon;

            //物品数量大于1，显示数量，否则
            if (inventorySlot.stackSize > 1)
            {
                itemText.text = inventorySlot.stackSize.ToString(); //显示堆叠
            }
            else
            {
                itemText.text = ""; 
            }
        }
    }

    //清理背包槽UI显示
    public void CleanUpInventorySlotUI()
    {
        inventorySlot = null; //清空当前槽位的数据

        itemImage.sprite = null; //清空物品图标
        itemImage.color = Color.clear; //将物品图标设置为透明

        itemText.text = null; //清空显示文本
    }

    //当用户点击背包槽调用
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //检查槽位物品数据
        if (inventorySlot == null)
        {
            return;
        }

        //按下了左Ctrl键并点击了鼠标左键，则从背包中移除物品
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(inventorySlot.item); //从背包中移除物品
            return; 
        }

        //如果物品是装备类型，则将其装备
        if (inventorySlot.item.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(inventorySlot.item); //装备物品
        }

        ui.itemToolTip.HideToolTip(); //隐藏物品的提示信息
    }

    //鼠标指针进入物品槽时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        //当前槽位没有物品，或者槽位物品为空，直接返回
        if (inventorySlot == null || inventorySlot.item == null)
        {
            return;
        }

        //如果物品是材料类型，
        if (inventorySlot.item.itemType == ItemType.Material)
        {
            return;
        }

        float yOffset = 0;

        //获取当前物品的装备数据
        ItemData_Equipment equipment = inventorySlot.item as ItemData_Equipment;

        //物品槽位，位于屏幕的下半部分，物品提示信息显示在物品上方
        if (transform.position.y <= Screen.height * 0.5)
        {
            // 如果物品描述的长度较长，则根据描述长度调整提示信息的显示位置
            if (equipment.GetItemStatInfoAndEffectDescription().Length >= 50)
            {
                // yOffset会根据描述的长度变化，确保长描述时不会超出屏幕
                yOffset = Screen.height * 0.01f + (equipment.GetItemStatInfoAndEffectDescription().Length - 50) * Screen.height * 0.001f;
            }
            else
            {
                //如果描述较短，使用默认偏移量
                yOffset = Screen.height * 0.01f;
            }
        }
        else //如果物品槽位位于屏幕的上半部分，物品提示信息应该显示在物品下方
        {
            yOffset = -Screen.height * 0.05f; //下方偏移量
        }

        //更新物品提示框的位置，确保它与物品槽保持相对位置
        ui.itemToolTip.transform.position = new Vector2(transform.position.x - Screen.width * 0.13f, transform.position.y + yOffset);

        //显示物品提示信息
        ui.itemToolTip.ShowToolTip(inventorySlot.item as ItemData_Equipment);
    }

    //当鼠标指针离开物品槽时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        //如果当前槽位没有物品，或者槽位物品为空
        if (inventorySlot == null || inventorySlot.item == null)
        {
            return;
        }

        //隐藏物品提示信息
        ui.itemToolTip.HideToolTip();
    }

}
