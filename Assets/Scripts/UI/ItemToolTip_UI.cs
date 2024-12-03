using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class ItemToolTip_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;  //物品名称文本框
    [SerializeField] private TextMeshProUGUI itemTypeText;  //物品类型文本框
    [SerializeField] private TextMeshProUGUI itemStatInfo;  //物品属性信息文本框

    [Range(1, 72)]
    [SerializeField] private int originalItemNameFontSize;      //物品名称原始字体大小

    private void Start()
    {
        //初始化物品名称文本框 的字体大小为原始大小
        itemNameText.fontSize = originalItemNameFontSize;
    }

    //显示物品提示框
    public void ShowToolTip(ItemData_Equipment item)
    {
        if (item == null)
        {
            return;
        }

        //设置物品类型和属性信息文本
        itemTypeText.text = item.equipmentType.ToString();
        itemStatInfo.text = item.GetItemStatInfoAndEffectDescription();

        //根据语言选择设置物品名称文本
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

        //根据物品名称长度调整字体大小
        if (itemNameText.text.Length > 12)
        {
            itemNameText.fontSize *= 0.8f;
        }
        else
        {
            itemNameText.fontSize = originalItemNameFontSize;
        }

        //显示物品提示框
        gameObject.SetActive(true);

    }

    //隐藏物品提示框
    public void HideToolTip()
    {
        itemNameText.fontSize = originalItemNameFontSize;
        gameObject.SetActive(false);
    }
}
