using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindow_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;  //��ʾ��Ʒ���Ƶ��ı����
    [SerializeField] private TextMeshProUGUI itemStatInfo;  //��ʾ��Ʒ������Ϣ���ı����
    [SerializeField] private Image itemIcon;  //��ʾ��Ʒͼ������
    [SerializeField] private Button craftButton;  //������ť�����

    [SerializeField] private Transform referenceRequiredMaterialList;  // �ο���������б�ĸ�����
    private Image[] materialImage;  //�洢�������ͼ�������

    private ItemData_Equipment equipmentToCraft;  // ��ǰ��������װ������

    //��Awake�г�ʼ���������ͼ������
    private void Awake()
    {
        materialImage = new Image[referenceRequiredMaterialList.childCount];  //��ʼ�����飬��СΪ��������б������������

        // ����������б��е������������Image�����ӵ�������
        for (int i = 0; i < referenceRequiredMaterialList.childCount; i++)
        {
            materialImage[i] = referenceRequiredMaterialList.GetChild(i).GetComponent<Image>();
        }
    }

    //����UIԪ������ʱ��������Ʒ���ƺ�������Ϣ������
    private void OnEnable()
    {
        if (equipmentToCraft != null)
        {
            UpdateItemTextLanguage(equipmentToCraft);  //����д�������װ����������Ʒ���ƺ�������Ϣ������
        }
    }

    public void SetupCraftWindow(ItemData_Equipment _itemToCraft)
    {
        equipmentToCraft = _itemToCraft;  //���õ�ǰ��������װ��

                                               //������찴ť�������¼�����������ֹ�ظ����¼�
        craftButton.onClick.RemoveAllListeners();

        if (_itemToCraft.requiredCraftMaterials.Count > materialImage.Length)
        {
            Debug.Log("�����������������UI��Ϊ��׼���Ĳ��ϲ�������");
        }

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;  //���ͼ����ɫ
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;  //����ı���ɫ
        }

        //һ��װ�������Ҫ������������
        for (int i = 0; i < _itemToCraft.requiredCraftMaterials.Count; i++)
        {
            //����������ϵ�ͼ�������
            materialImage[i].sprite = _itemToCraft.requiredCraftMaterials[i].item.icon;  //����ͼ��
            materialImage[i].color = Color.white;  //��ʾ����ͼ��

            TextMeshProUGUI requiredMaterialText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();  //���������ı�

            requiredMaterialText.text = _itemToCraft.requiredCraftMaterials[i].stackSize.ToString();  //�����������
            requiredMaterialText.color = Color.white;  //�ı���ɫ
        }

        //������װ����ͼ�ꡢ���ơ�������Ϣ
        itemIcon.sprite = _itemToCraft.icon;  //װ��ͼ��
        itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();  //װ�������Ժ�Ч������

        //����װ���������ı�
        UpdateItemTextLanguage(_itemToCraft);

        //��ӵ����ťʱ�������¼�������������װ�����������������
        craftButton.onClick.AddListener(() => Inventory.instance.CraftIfAvailable(_itemToCraft, _itemToCraft.requiredCraftMaterials));
    }

    //����װ�����������е��ı����ݣ����ݵ�ǰ���Ի�������Ӣ��������ʾ����
    private void UpdateItemTextLanguage(ItemData_Equipment _itemToCraft)
    {
        if (LanguageManager.instance.localeID == 0) //Ӣ
        {
            itemName.text = _itemToCraft.itemName;

            //װ��������ϢΪӢ��
            itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();
        }
        else if (LanguageManager.instance.localeID == 1)    //��
        {
            // ����װ������Ϊ��������
            itemName.text = _itemToCraft.itemName_Chinese;

            itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();

            //��װ����Ӣ��������Ϣ��������ģ�������ʾ
            itemStatInfo.text = LanguageManager.instance.TranslateItemStatInfoFromEnglishToChinese(itemStatInfo.text);
        }
    }

}
