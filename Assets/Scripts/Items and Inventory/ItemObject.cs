using UnityEngine;

                           //�ǵ���Unity�༭���У�ÿ�δ�������Ʒʱ��ʹ��InventoryManager�ű��е�Fill up item data base����
public class ItemObject : MapElement
{
    [SerializeField] private Rigidbody2D rb;  //������Ʒ�ĸ��壬����������Ʒ���������Ч��
    [SerializeField] private ItemData item;  //�洢��Ʒ�����ݣ�������Ʒ���ơ�ͼ��ȣ�

    // ��ʼ��ʱ���ã�������Ʒ��ͼ�������
    protected override void Start()
    {
        SetupItemIconAndName();  //��Ʒͼ�������
        base.Start(); 
    }

    //���õ�����Ʒ�����ԣ�������Ʒ���ݺ͵�����ٶ�
    public void SetupItemDrop(ItemData _item, Vector2 _dropVelocity)
    {
        item = _item;  //������Ʒ����
        rb.velocity = _dropVelocity;  //������Ʒ������ٶ�

        SetupItemIconAndName();  //������Ʒͼ�������
    }

    //��Ʒ����ķ���
    public void PickupItem()
    {
        //�����������������Ʒ��װ������Ʒ�����ܼ������Ʒ
        if (!Inventory.instance.CanAddEquipmentToInventory() && item.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 5);  //ʹ�������

            //���ݲ�ͬ������ʾ��ʾ��Ϣ
            if (LanguageManager.instance.localeID == 0)  //Ӣ��
            {
                PlayerManager.instance.player.fx.CreatePopUpText("No more space in inventory!");
            }
            else if (LanguageManager.instance.localeID == 1)  //����
            {
                PlayerManager.instance.player.fx.CreatePopUpText("�����ռ䲻��!");
            }
            return;
        }

        //�򱳰��������Ʒ
        Inventory.instance.AddItem(item);
        //������Ʒ�������Ч
        AudioManager.instance.PlaySFX(18, transform);

        //����Ʒ��MapElementID������ʹ��Ԫ���б�
        GameManager.instance.UsedMapElementIDList.Add(mapElementID);

        Debug.Log($"Picked up item {item.itemName}");  //���������Ϣ����ʾ��������Ʒ
        Destroy(gameObject);  //���ٵ�ǰ��Ʒ����
    }

    //������Ʒ��ͼ�������
    private void SetupItemIconAndName()
    {
        if (item == null)  //�����ƷΪ�գ���������
        {
            return;
        }
        //������Ʒ��ͼ��
        GetComponent<SpriteRenderer>().sprite = item.icon;
        //������Ʒ������
        gameObject.name = $"Item Object - {item.name}";
    }
}
