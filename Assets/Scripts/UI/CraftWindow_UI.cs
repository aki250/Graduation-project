using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindow_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;  //显示物品名称的文本组件
    [SerializeField] private TextMeshProUGUI itemStatInfo;  //显示物品属性信息的文本组件
    [SerializeField] private Image itemIcon;  //显示物品图标的组件
    [SerializeField] private Button craftButton;  //制作按钮的组件

    [SerializeField] private Transform referenceRequiredMaterialList;  // 参考所需材料列表的父物体
    private Image[] materialImage;  //存储所需材料图标的数组

    private ItemData_Equipment equipmentToCraft;  // 当前待制作的装备数据

    //在Awake中初始化所需材料图标数组
    private void Awake()
    {
        materialImage = new Image[referenceRequiredMaterialList.childCount];  //初始化数组，大小为所需材料列表的子物体数量

        // 将所需材料列表中的所有子物体的Image组件添加到数组中
        for (int i = 0; i < referenceRequiredMaterialList.childCount; i++)
        {
            materialImage[i] = referenceRequiredMaterialList.GetChild(i).GetComponent<Image>();
        }
    }

    //当该UI元素启用时，更新物品名称和属性信息的语言
    private void OnEnable()
    {
        if (equipmentToCraft != null)
        {
            UpdateItemTextLanguage(equipmentToCraft);  //如果有待制作的装备，更新物品名称和属性信息的语言
        }
    }

    public void SetupCraftWindow(ItemData_Equipment _itemToCraft)
    {
        equipmentToCraft = _itemToCraft;  //设置当前待制作的装备

                                               //清除制造按钮的所有事件监听器，防止重复绑定事件
        craftButton.onClick.RemoveAllListeners();

        if (_itemToCraft.requiredCraftMaterials.Count > materialImage.Length)
        {
            Debug.Log("所需材料数量，超过UI中为其准备的材料槽数量。");
        }

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;  //清除图标颜色
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;  //清除文本颜色
        }

        //一个装备最多需要四种制作材料
        for (int i = 0; i < _itemToCraft.requiredCraftMaterials.Count; i++)
        {
            //设置所需材料的图标和数量
            materialImage[i].sprite = _itemToCraft.requiredCraftMaterials[i].item.icon;  //材料图标
            materialImage[i].color = Color.white;  //显示材料图标

            TextMeshProUGUI requiredMaterialText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();  //材料数量文本

            requiredMaterialText.text = _itemToCraft.requiredCraftMaterials[i].stackSize.ToString();  //所需材料数量
            requiredMaterialText.color = Color.white;  //文本颜色
        }

        //待制作装备的图标、名称、属性信息
        itemIcon.sprite = _itemToCraft.icon;  //装备图标
        itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();  //装备的属性和效果描述

        //更新装备的语言文本
        UpdateItemTextLanguage(_itemToCraft);

        //添加点击按钮时触发的事件，尝试制作该装备（如果满足条件）
        craftButton.onClick.AddListener(() => Inventory.instance.CraftIfAvailable(_itemToCraft, _itemToCraft.requiredCraftMaterials));
    }

    //更新装备制作窗口中的文本内容，根据当前语言环境（中英）调整显示内容
    private void UpdateItemTextLanguage(ItemData_Equipment _itemToCraft)
    {
        if (LanguageManager.instance.localeID == 0) //英
        {
            itemName.text = _itemToCraft.itemName;

            //装备属性信息为英文
            itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();
        }
        else if (LanguageManager.instance.localeID == 1)    //中
        {
            // 设置装备名称为中文名称
            itemName.text = _itemToCraft.itemName_Chinese;

            itemStatInfo.text = _itemToCraft.GetItemStatInfoAndEffectDescription();

            //将装备的英文属性信息翻译成中文，更新显示
            itemStatInfo.text = LanguageManager.instance.TranslateItemStatInfoFromEnglishToChinese(itemStatInfo.text);
        }
    }

}
