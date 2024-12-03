using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    //��ҵ��������
    [Header("��ҵ���")]
    [SerializeField] private float chanceToDropEquiment; //����װ���ĸ���
    [SerializeField] private float chanceToDropMaterials; //������ϵĸ���

    //��д�������ɷ���
    public override void GenrateDrop()
    {
        //��ȡ��ҵ�ǰװ������Ʒ�ͱ����е���Ʒ�б�
        Inventory inventory = Inventory.instance;

        //��ȡ��ǰװ������Ʒ�б�
        List<InventorySlot> currentEquippedEquipment = inventory.GetEquippedEquipmentList();
        List<InventorySlot> EquipmentToLose = new List<InventorySlot>(); //��Ž�Ҫ�����װ��

        //�����е���Ʒ�б�
        List<InventorySlot> currentStash = inventory.GetStashList();
        List<InventorySlot> materialsToLose = new List<InventorySlot>(); //��Ž�Ҫ����Ĳ���

        //������ǰװ������Ʒ�����յ�������ж��Ƿ����
        for (int i = 0; i < currentEquippedEquipment.Count; i++)
        {
            //�������һ��0��100֮��������������С�ڵ��ڵ���װ���ĸ��ʣ�������װ��
            if (Random.Range(0, 100) <= chanceToDropEquiment)
            {
                EquipmentToLose.Add(currentEquippedEquipment[i]); //��Ҫ�����װ����ӵ��б�
            }
        }

        //���������е���Ʒ�����յ�������ж��Ƿ����
        for (int i = 0; i < currentStash.Count; i++)
        {
            //�������һ��0��100֮��������������С�ڵ��ڵ�����ϵĸ��ʣ������ò���
            if (Random.Range(0, 100) <= chanceToDropMaterials)
            {
                materialsToLose.Add(currentStash[i]); //��Ҫ����Ĳ�����ӵ��б�
            }
        }

        //���ڵ����װ����ж��װ������װ������
        for (int i = 0; i < EquipmentToLose.Count; i++)
        {
            //ж��װ�����Ҳ��������·Żر���
            inventory.UnequipEquipmentWithoutAddingBackToInventory(EquipmentToLose[i].item as ItemData_Equipment);
            //����װ����Ʒ
            DropItem(EquipmentToLose[i].item);
        }

        //���ڵ���Ĳ��ϣ��������ӱ������Ƴ�
        for (int i = 0; i < materialsToLose.Count; i++)
        {
            DropItem(materialsToLose[i].item);  //����������Ʒ

            inventory.RemoveItem(materialsToLose[i].item);  //�ӱ������Ƴ�����
        }
    }
}

