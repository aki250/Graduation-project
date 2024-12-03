using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class ItemToolTip_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;  //��Ʒ�����ı���
    [SerializeField] private TextMeshProUGUI itemTypeText;  //��Ʒ�����ı���
    [SerializeField] private TextMeshProUGUI itemStatInfo;  //��Ʒ������Ϣ�ı���

    [Range(1, 72)]
    [SerializeField] private int originalItemNameFontSize;      //��Ʒ����ԭʼ�����С

    private void Start()
    {
        //��ʼ����Ʒ�����ı��� �������СΪԭʼ��С
        itemNameText.fontSize = originalItemNameFontSize;
    }

    //��ʾ��Ʒ��ʾ��
    public void ShowToolTip(ItemData_Equipment item)
    {
        if (item == null)
        {
            return;
        }

        //������Ʒ���ͺ�������Ϣ�ı�
        itemTypeText.text = item.equipmentType.ToString();
        itemStatInfo.text = item.GetItemStatInfoAndEffectDescription();

        //��������ѡ��������Ʒ�����ı�
        if (LanguageManager.instance.localeID == 0)
        {
            itemNameText.text = item.itemName;
        }
        else if (LanguageManager.instance.localeID == 1)
        {
            itemNameText.text = item.itemName_Chinese;
            itemTypeText.text = LanguageManager.instance.EnglishToChineseEquipmentTypeDictionary[itemTypeText.text];
            itemStatInfo.text = LanguageManager.instance.TranslateItemStatInfoFromEnglishToChinese(itemStatInfo.text);
        }

        //������Ʒ���Ƴ��ȵ��������С
        if (itemNameText.text.Length > 12)
        {
            itemNameText.fontSize *= 0.8f;
        }
        else
        {
            itemNameText.fontSize = originalItemNameFontSize;
        }

        //��ʾ��Ʒ��ʾ��
        gameObject.SetActive(true);

    }

    //������Ʒ��ʾ��
    public void HideToolTip()
    {
        itemNameText.fontSize = originalItemNameFontSize;
        gameObject.SetActive(false);
    }
}
