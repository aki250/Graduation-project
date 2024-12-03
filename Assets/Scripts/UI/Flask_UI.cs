using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Flask_UI : MonoBehaviour
{
    public static Flask_UI instance;

    //药水瓶的图标 和冷却图标
    public Image flaskImage;
    public Image flaskCooldownImage;

    //用于存储装备的药水瓶
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
        //在开始时检查是否有装备药水瓶
        //如果没有装备药水瓶，则隐藏图标
        SetFlaskImage(Inventory.instance.GetEquippedEquipmentByType(EquipmentType.Flask));
    }

    //当装备的物品是药水瓶时，调用此函数（通常由Inventory.EquipItem调用）
    //如果药水瓶被卸下，则调用此函数来移除显示
    public void SetFlaskImage(ItemData_Equipment _flask)
    {
        if (_flask == null)
        {
            //如果没有药水瓶，隐藏UI
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            //设置药水瓶的图标
            flaskImage.sprite = _flask.icon;
            // 设置冷却图标（通常在冷却时显示与药水瓶相同的图标，但可能有不同的效果）
            flaskCooldownImage.sprite = _flask.icon;
        }
    }
}
