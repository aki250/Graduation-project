using UnityEngine.EventSystems;

public class CraftSlot_UI : InventorySlot_UI
{
    private ItemData_Equipment equipment;  //�洢��ǰ����۵�װ������

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        SetupCraftSlot(equipment);  //���õ�ǰ�����װ��
    }

    //����۵�UI����
    public void SetupCraftSlot(ItemData_Equipment _item)
    {
        if (_item == null)
        {
            return;  
        }

        equipment = _item;  //�������װ�����ݸ�ֵ����ǰ��

        inventorySlot.item = _item;  //���ÿ�����ƷΪ��ǰװ��
        itemImage.sprite = _item.icon;  //����װ��ͼ��

        if (LanguageManager.instance.localeID == 0)
        {
            itemText.text = _item.itemName;  //Ӣ��
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            itemText.text = _item.itemName_Chinese;  //����
        }

        //���װ�����Ƴ��ȳ���12���ַ������������С
        if (itemText.text.Length > 12)
        {
            itemText.fontSize = itemText.fontSize * 0.8f;  //������С
        }
        else
        {
            itemText.fontSize = 24;  //Ĭ�������С
        }
    }

    //����ҵ�������ʱ���򿪶�Ӧ�����촰��
    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.craftWindow.SetupCraftWindow(inventorySlot.item as ItemData_Equipment);  //�������촰�ڲ���ʾ��ǰװ������ϸ��Ϣ
    }
}
