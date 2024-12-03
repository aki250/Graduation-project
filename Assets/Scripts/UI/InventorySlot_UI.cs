using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot_UI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage; //��ʾ��Ʒͼ��Image���
    [SerializeField] protected TextMeshProUGUI itemText; //��ʾ��Ʒ��������Ϣ��TextMeshProUGUI���

    public InventorySlot inventorySlot; //�洢��ǰ�����۵���Ʒ��Ϣ��ֵ�� UpdateInventroySlotUI ��������

    protected UI ui; //��ǰUI�ؼ�����

    //��ʼ��ʱ��ȡ���� UI �ؼ�������
    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
    }

    //���±����۵���ʾ����
    public void UpdateInventorySlotUI(InventorySlot _inventorySlot)
    {
        inventorySlot = _inventorySlot; //������ı��������ݸ�ֵ����ǰ��

        itemImage.color = Color.white; //������Ʒͼ�����ɫΪĬ�ϰ�ɫ

        //�����Ʒ����
        if (inventorySlot != null)
        {
            //��Ʒͼ��
            itemImage.sprite = inventorySlot.item.icon;

            //��Ʒ��������1����ʾ����������
            if (inventorySlot.stackSize > 1)
            {
                itemText.text = inventorySlot.stackSize.ToString(); //��ʾ�ѵ�
            }
            else
            {
                itemText.text = ""; 
            }
        }
    }

    //��������UI��ʾ
    public void CleanUpInventorySlotUI()
    {
        inventorySlot = null; //��յ�ǰ��λ������

        itemImage.sprite = null; //�����Ʒͼ��
        itemImage.color = Color.clear; //����Ʒͼ������Ϊ͸��

        itemText.text = null; //�����ʾ�ı�
    }

    //���û���������۵���
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //����λ��Ʒ����
        if (inventorySlot == null)
        {
            return;
        }

        //��������Ctrl�������������������ӱ������Ƴ���Ʒ
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(inventorySlot.item); //�ӱ������Ƴ���Ʒ
            return; 
        }

        //�����Ʒ��װ�����ͣ�����װ��
        if (inventorySlot.item.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(inventorySlot.item); //װ����Ʒ
        }

        ui.itemToolTip.HideToolTip(); //������Ʒ����ʾ��Ϣ
    }

    //���ָ�������Ʒ��ʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        //��ǰ��λû����Ʒ�����߲�λ��ƷΪ�գ�ֱ�ӷ���
        if (inventorySlot == null || inventorySlot.item == null)
        {
            return;
        }

        //�����Ʒ�ǲ������ͣ�
        if (inventorySlot.item.itemType == ItemType.Material)
        {
            return;
        }

        float yOffset = 0;

        //��ȡ��ǰ��Ʒ��װ������
        ItemData_Equipment equipment = inventorySlot.item as ItemData_Equipment;

        //��Ʒ��λ��λ����Ļ���°벿�֣���Ʒ��ʾ��Ϣ��ʾ����Ʒ�Ϸ�
        if (transform.position.y <= Screen.height * 0.5)
        {
            // �����Ʒ�����ĳ��Ƚϳ���������������ȵ�����ʾ��Ϣ����ʾλ��
            if (equipment.GetItemStatInfoAndEffectDescription().Length >= 50)
            {
                // yOffset����������ĳ��ȱ仯��ȷ��������ʱ���ᳬ����Ļ
                yOffset = Screen.height * 0.01f + (equipment.GetItemStatInfoAndEffectDescription().Length - 50) * Screen.height * 0.001f;
            }
            else
            {
                //��������϶̣�ʹ��Ĭ��ƫ����
                yOffset = Screen.height * 0.01f;
            }
        }
        else //�����Ʒ��λλ����Ļ���ϰ벿�֣���Ʒ��ʾ��ϢӦ����ʾ����Ʒ�·�
        {
            yOffset = -Screen.height * 0.05f; //�·�ƫ����
        }

        //������Ʒ��ʾ���λ�ã�ȷ��������Ʒ�۱������λ��
        ui.itemToolTip.transform.position = new Vector2(transform.position.x - Screen.width * 0.13f, transform.position.y + yOffset);

        //��ʾ��Ʒ��ʾ��Ϣ
        ui.itemToolTip.ShowToolTip(inventorySlot.item as ItemData_Equipment);
    }

    //�����ָ���뿪��Ʒ��ʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        //�����ǰ��λû����Ʒ�����߲�λ��ƷΪ��
        if (inventorySlot == null || inventorySlot.item == null)
        {
            return;
        }

        //������Ʒ��ʾ��Ϣ
        ui.itemToolTip.HideToolTip();
    }

}
