using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class EquippedEquipmentSlot_UI : InventorySlot_UI
{
    //��ǰװ��������
    public EquipmentType equipmentType;

    //��Unity�༭���У����ö���ѡ��ʱ���Զ�����,�����ڱ༭���и��¸ö�������֣��������ֲ�ͬ���͵�װ����
    private void OnValidate()
    {
        // ����GameObject������Ϊװ������
        gameObject.name = equipmentType.ToString();
    }

    // ����ҵ��װ����ʱ�������¼�
    public override void OnPointerDown(PointerEventData eventData)
    {
        // �����һ�����ɼ���װ����UIʱ��ֱ�ӷ��ز������κδ���
        //��ֹ���� Unity��bug����ʹû��װ����Unity��Ȼ��Ϊװ����������Ʒ��
        //����������bugҪ����Unity
        if (inventorySlot == null || inventorySlot.item == null)
        {
            return;
        }

        //ж��װ�������䷵�ص�������
        Inventory.instance.UnequipEquipmentWithoutAddingBackToInventory(inventorySlot.item as ItemData_Equipment);

        //��װ��������ӵ�����
        Inventory.instance.AddItem(inventorySlot.item as ItemData_Equipment);

        //����װ����UI������װ���۵���ʾ״̬��
        CleanUpInventorySlotUI();  //�������࣬��Ϊ�� AddItem() �л���� UpdateAllSlotUI() ���²�UI

        //���ع�����ʾUI
        ui.itemToolTip.HideToolTip();
    }
}

