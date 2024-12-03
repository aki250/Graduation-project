using UnityEngine.EventSystems;

public class CraftSlot_UI : InventorySlot_UI
{
    private ItemData_Equipment equipment;  //存储当前制造槽的装备数据

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        SetupCraftSlot(equipment);  //设置当前制造槽装备
    }

    //制造槽的UI内容
    public void SetupCraftSlot(ItemData_Equipment _item)
    {
        if (_item == null)
        {
            return;  
        }

        equipment = _item;  //将传入的装备数据赋值给当前槽

        inventorySlot.item = _item;  //设置库存槽物品为当前装备
        itemImage.sprite = _item.icon;  //设置装备图标

        if (LanguageManager.instance.localeID == 0)
        {
            itemText.text = _item.itemName;  //英文
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            itemText.text = _item.itemName_Chinese;  //中文
        }

        //如果装备名称长度超过12个字符，调整字体大小
        if (itemText.text.Length > 12)
        {
            itemText.fontSize = itemText.fontSize * 0.8f;  //字体缩小
        }
        else
        {
            itemText.fontSize = 24;  //默认字体大小
        }
    }

    //当玩家点击制造槽时，打开对应的制造窗口
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(inventorySlot.item as ItemData_Equipment);  //设置制造窗口并显示当前装备的详细信息
    }
}
