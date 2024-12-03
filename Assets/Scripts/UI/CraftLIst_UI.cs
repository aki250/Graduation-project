using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftList_UI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform referenceCraftList;  //用于参考的制造列表的父物体
    [SerializeField] private GameObject craftSlotPrefab;  //制作槽的预制件

    [SerializeField] private List<ItemData_Equipment> equipmentCraftList;  //用于制造的装备列表

    private void Start()
    {
        //设置默认的制造列表（第一个制造类型 - 武器）
        transform.parent.GetChild(0)?.GetComponent<CraftList_UI>()?.SetupCraftList();
        // 设置默认的制造窗口（第一个武器在武器制造列表中）
        transform.parent.GetChild(0)?.GetComponent<CraftList_UI>()?.SetupDefaultCraftWindow();
    }

    //设置制造列表UI
    public void SetupCraftList()
    {
        //删除当前制造槽UI
        for (int i = 0; i < referenceCraftList.childCount; i++)
        {
            Destroy(referenceCraftList.GetChild(i).gameObject);  //销毁当前子物体（制造槽）
        }

        //根据装备列表生成新的制造槽
        for (int i = 0; i < equipmentCraftList.Count; i++)
        {
            GameObject newCraftSlot = Instantiate(craftSlotPrefab, referenceCraftList);  //实例化新的制造槽
            newCraftSlot.GetComponent<CraftSlot_UI>()?.SetupCraftSlot(equipmentCraftList[i]);  //设置新制造槽
        }
    }

    //设置默认的制造窗口UI
    public void SetupDefaultCraftWindow()
    {
        if (equipmentCraftList[0] != null)
        {
            //如果装备列表的第一个项目不为空，设置制造窗口
            GetComponentInParent<UI>()?.craftWindow.SetupCraftWindow(equipmentCraftList[0]);
        }
    }

    //点击触发：更新制造列表
    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();  //点击时重新设置制造列表
    }
}
