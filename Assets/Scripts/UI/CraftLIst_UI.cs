using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftList_UI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform referenceCraftList;  //���ڲο��������б�ĸ�����
    [SerializeField] private GameObject craftSlotPrefab;  //�����۵�Ԥ�Ƽ�

    [SerializeField] private List<ItemData_Equipment> equipmentCraftList;  //���������װ���б�

    private void Start()
    {
        //����Ĭ�ϵ������б���һ���������� - ������
        transform.parent.GetChild(0)?.GetComponent<CraftList_UI>()?.SetupCraftList();
        // ����Ĭ�ϵ����촰�ڣ���һ�����������������б��У�
        transform.parent.GetChild(0)?.GetComponent<CraftList_UI>()?.SetupDefaultCraftWindow();
    }

    //���������б�UI
    public void SetupCraftList()
    {
        //ɾ����ǰ�����UI
        for (int i = 0; i < referenceCraftList.childCount; i++)
        {
            Destroy(referenceCraftList.GetChild(i).gameObject);  //���ٵ�ǰ�����壨����ۣ�
        }

        //����װ���б������µ������
        for (int i = 0; i < equipmentCraftList.Count; i++)
        {
            GameObject newCraftSlot = Instantiate(craftSlotPrefab, referenceCraftList);  //ʵ�����µ������
            newCraftSlot.GetComponent<CraftSlot_UI>()?.SetupCraftSlot(equipmentCraftList[i]);  //�����������
        }
    }

    //����Ĭ�ϵ����촰��UI
    public void SetupDefaultCraftWindow()
    {
        if (equipmentCraftList[0] != null)
        {
            //���װ���б�ĵ�һ����Ŀ��Ϊ�գ��������촰��
            GetComponentInParent<UI>()?.craftWindow.SetupCraftWindow(equipmentCraftList[0]);
        }
    }

    //������������������б�
    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();  //���ʱ�������������б�
    }
}
