using UnityEngine;

                           //记得在Unity编辑器中，每次创建新物品时，使用InventoryManager脚本中的Fill up item data base函数
public class ItemObject : MapElement
{
    [SerializeField] private Rigidbody2D rb;  //用于物品的刚体，用来控制物品掉落的物理效果
    [SerializeField] private ItemData item;  //存储物品的数据（例如物品名称、图标等）

    // 初始化时调用，设置物品的图标和名称
    protected override void Start()
    {
        SetupItemIconAndName();  //物品图标和名称
        base.Start(); 
    }

    //设置掉落物品的属性，包括物品数据和掉落的速度
    public void SetupItemDrop(ItemData _item, Vector2 _dropVelocity)
    {
        item = _item;  //设置物品数据
        rb.velocity = _dropVelocity;  //设置物品掉落的速度

        SetupItemIconAndName();  //设置物品图标和名称
    }

    //物品捡起的方法
    public void PickupItem()
    {
        //如果背包已满并且物品是装备类物品，则不能捡起该物品
        if (!Inventory.instance.CanAddEquipmentToInventory() && item.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 5);  //使其飞起来

            //根据不同语言显示提示信息
            if (LanguageManager.instance.localeID == 0)  //英文
            {
                PlayerManager.instance.player.fx.CreatePopUpText("No more space in inventory!");
            }
            else if (LanguageManager.instance.localeID == 1)  //中文
            {
                PlayerManager.instance.player.fx.CreatePopUpText("背包空间不足!");
            }
            return;
        }

        //向背包中添加物品
        Inventory.instance.AddItem(item);
        //播放物品捡起的音效
        AudioManager.instance.PlaySFX(18, transform);

        //将物品的MapElementID加入已使用元素列表
        GameManager.instance.UsedMapElementIDList.Add(mapElementID);

        Debug.Log($"Picked up item {item.itemName}");  //输出调试信息，表示捡起了物品
        Destroy(gameObject);  //销毁当前物品对象
    }

    //设置物品的图标和名称
    private void SetupItemIconAndName()
    {
        if (item == null)  //如果物品为空，跳过设置
        {
            return;
        }
        //设置物品的图标
        GetComponent<SpriteRenderer>().sprite = item.icon;
        //设置物品的名称
        gameObject.name = $"Item Object - {item.name}";
    }
}
