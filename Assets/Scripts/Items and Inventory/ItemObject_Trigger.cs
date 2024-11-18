using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject;

    private void Awake()
    {
        //��ȡ��Ϸ����ĸ������ϵ�ItemObject�����
        myItemObject = GetComponentInParent<ItemObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //����������������ִ���κβ�����
        if (PlayerManager.instance.player.stats.isDead)
        {
            return;
        }

        //�����봥��������Ϸ�����Ƿ���Player�����
        if (collision.GetComponent<Player>() != null)
        {
            myItemObject.PickupItem();  //����myItemObject��PickupItem������ִ��ʰȡ��Ʒ�Ĳ�����
        }
    }
}