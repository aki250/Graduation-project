using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Flask_UI : MonoBehaviour
{
    public static Flask_UI instance;

    //ҩˮƿ��ͼ�� ����ȴͼ��
    public Image flaskImage;
    public Image flaskCooldownImage;

    //���ڴ洢װ����ҩˮƿ
    //private ItemData_Equipment flask;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        //�ڿ�ʼʱ����Ƿ���װ��ҩˮƿ
        //���û��װ��ҩˮƿ��������ͼ��
        SetFlaskImage(Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Flask));
    }

    //��װ������Ʒ��ҩˮƿʱ�����ô˺�����ͨ����Inventory.EquipItem���ã�
    //���ҩˮƿ��ж�£�����ô˺������Ƴ���ʾ
    public void SetFlaskImage(ItemData_Equipment _flask)
    {
        if (_flask == null)
        {
            //���û��ҩˮƿ������UI
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            //����ҩˮƿ��ͼ��
            flaskImage.sprite = _flask.icon;
            // ������ȴͼ�꣨ͨ������ȴʱ��ʾ��ҩˮƿ��ͬ��ͼ�꣬�������в�ͬ��Ч����
            flaskCooldownImage.sprite = _flask.icon;
        }
    }
}
