using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    //��������Ʒ����
    [SerializeField] private int maxItemDropAmount;

    //���ܵ������Ʒ�б�
    [SerializeField] private ItemData[] possibleDropItemList;

    //ʵ�ʵ������Ʒ�б�
    private List<ItemData> actualDropList = new List<ItemData>();

    //������Ʒ��Ԥ�Ƽ�������ʱ��������õ���Ʒ��������ʼ��
    [SerializeField] private GameObject dropItemPrefab;


    //���ɵ�����Ʒ
    public virtual void GenrateDrop()
    {
        //���ݵ��伸�ʽ���Ʒ��ӵ�ʵ�ʵ����б���
        for (int i = 0; i < possibleDropItemList.Length; i++)
        {
            //�������һ�������������С�ڵ�����Ʒ�ĵ�����ʣ��ͽ�����Ʒ��ӵ������б���
            if (Random.Range(0, 100) <= possibleDropItemList[i].dropChance)
            {
                actualDropList.Add(possibleDropItemList[i]);
            }
        }

        //��ʵ�ʵ����б������ѡ����Ʒ���е��䣬�����б����Ƴ��ѵ������Ʒ
        for (int i = 0; i < maxItemDropAmount && actualDropList.Count > 0; i++)
        {
            //���ѡ��һ����Ʒ���е���
            ItemData itemToDrop = actualDropList[Random.Range(0, actualDropList.Count - 1)];

            //�ӵ����б����Ƴ��ѵ������Ʒ
            actualDropList.Remove(itemToDrop);

            //������Ʒ
            DropItem(itemToDrop);
        }
    }

    //����������ʱ���ô˷���
    protected void DropItem(ItemData _itemToDrop)
    {
        //�����µĵ�����Ʒʵ����λ���ڵ��˵ĵ�ǰλ��
        GameObject newDropItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);

        //������Ʒ�ĳ��ٶȣ�ģ����Ʒ�����Ч��
        Vector2 dropVelocity = new Vector2(Random.Range(-5, 5), Random.Range(12, 15));

        //������Ʒ�����ƺ�ͼ��
        newDropItem.GetComponent<ItemObject>()?.SetupItemDrop(_itemToDrop, dropVelocity);
    }
}
