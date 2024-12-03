using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, IGameProgressionSaveManager
{
    public static Inventory instance;

    //װ��
    public List<InventorySlot> inventorySlotList;
    public Dictionary<ItemData, InventorySlot> inventorySlotDictionary;

    //����
    public List<InventorySlot> stashSlotList;
    public Dictionary<ItemData, InventorySlot> stashSlotDictionary;

    //װ����Ʒ
    public List<InventorySlot> equippedEquipmentSlotList;
    public Dictionary<ItemData_Equipment, InventorySlot> equippedEquipmentSlotDictionary;


    public List<ItemData> startItems;   //��ʼ��Ʒ

    [Header("��ƷUI")]
    [SerializeField] private Transform referenceInventory;  //��Ʒ��UI
    [SerializeField] private Transform referenceStash;  //���ϵ�UI
    [SerializeField] private Transform referenceEquippedEquipments; //װ����UI
    [SerializeField] private Transform referenceStatPanel;  //��ɫ�������UI

    private InventorySlot_UI[] inventorySlotUI;
    private InventorySlot_UI[] stashSlotUI;
    private EquippedEquipmentSlot_UI[] equippedEquipmentSlotUI;
    private StatSlot_UI[] statSlotUI;

    //private float flaskLastUseTime;
    //private bool flaskUsed = false;


    [Header("��Ʒ���ݿ�")]
    public List<ItemData> itemDataBase; //��Ʒ���ݻ�����
    public List<InventorySlot> loadedInventorySlots;    //������Ʒ������
    public List<ItemData_Equipment> loadedEquippedEquipment;    //����װ������

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //��ʼ����桢������װ���ۼ����ֵ�
        inventorySlotList = new List<InventorySlot>();
        inventorySlotDictionary = new Dictionary<ItemData, InventorySlot>();

        stashSlotList = new List<InventorySlot>();
        stashSlotDictionary = new Dictionary<ItemData, InventorySlot>();

        equippedEquipmentSlotList = new List<InventorySlot>();
        equippedEquipmentSlotDictionary = new Dictionary<ItemData_Equipment, InventorySlot>();

        inventorySlotUI = referenceInventory.GetComponentsInChildren<InventorySlot_UI>();
        stashSlotUI = referenceStash.GetComponentsInChildren<InventorySlot_UI>();
        equippedEquipmentSlotUI = referenceEquippedEquipments.GetComponentsInChildren<EquippedEquipmentSlot_UI>();
        statSlotUI = referenceStatPanel.GetComponentsInChildren<StatSlot_UI>();

        AddStartItems();    //��ӳ�ʼ��Ʒ
        RefreshEquipmentEffectUseState();   //ˢ��װ��Ч��״̬
    }

    private void AddStartItems()
    {
        //���س�ʼװ����Ʒ
        foreach (var equipment in loadedEquippedEquipment)
        {
            EquipItem(equipment);
        }

        //���ؿ����Ʒ
        if (loadedInventorySlots.Count > 0)
        {
            foreach (var slot in loadedInventorySlots)
            {
                for (int i = 0; i < slot.stackSize; i++)
                {
                    AddItem(slot.item); //�����������Ʒ
                }
            }

            return;
        }
        //��û�м��ص���Ʒ������ӳ�ʼ��Ʒ
        for (int i = 0; i < startItems.Count; i++)
        {
            if (startItems[i] != null)
            {
                AddItem(startItems[i]); //��ӳ�ʼ��Ʒ
            }
        }
    }

    private void Update()
    {
    }

    //��������UI�۵���ʾ
    private void UpdateAllSlotUI()
    {
        //��������UI�ۣ���ȷ��û�ж����UI
        for (int i = 0; i < inventorySlotUI.Length; i++)
        {
            inventorySlotUI[i].CleanUpInventorySlotUI();
        }

        for (int i = 0; i < stashSlotUI.Length; i++)
        {
            stashSlotUI[i].CleanUpInventorySlotUI();
        }

        for (int i = 0; i < equippedEquipmentSlotUI.Length; i++)
        {
            equippedEquipmentSlotUI[i].CleanUpInventorySlotUI();
        }

        //���¿��UI
        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            inventorySlotUI[i].UpdateInventorySlotUI(inventorySlotList[i]);
        }

        //���²�����UI
        for (int i = 0; i < stashSlotList.Count; i++)
        {
            stashSlotUI[i].UpdateInventorySlotUI(stashSlotList[i]);
        }

        //��������װ������UI��
        for (int i = 0; i < equippedEquipmentSlotUI.Length; i++)
        {
            //����װ������װ���ֵ䣬�����Ƿ���ƥ�����͵�װ��
            foreach (var search in equippedEquipmentSlotDictionary)
            {
                //���װ�������뵱ǰUI�۵�����ƥ��
                if (search.Key.equipmentType == equippedEquipmentSlotUI[i].equipmentType)
                {
                    //����UI�۵���ʾ������ƥ���װ����
                    equippedEquipmentSlotUI[i].UpdateInventorySlotUI(search.Value);
                }
            }
        }

        UpdateStatUI();     //���½�ɫ����ͳ����Ϣ

    }

    public void UpdateStatUI()
    {
        for (int i = 0; i < statSlotUI.Length; i++)
        {
            statSlotUI[i].UpdateStatValue_UI();
        }
    }

    //װ��
    public void EquipItem(ItemData _item)
    {
        //�������ItemData������Ʒ��ת��ΪItemData_Equipment����
        ItemData_Equipment _newEquipmentToEquip = _item as ItemData_Equipment;

        //�����µ�װ���۲�Ϊ�������װ��
        InventorySlot newEquipmentSlot = new InventorySlot(_newEquipmentToEquip);

        //�洢��װ���ľ�װ��
        ItemData_Equipment _oldEquippedEquipment = null;

        //�����ֵ��е�������װ��װ���������Ƿ���װ����ͬ���͵�װ��
        //�����var search�� KeyValuePair<ItemData_Equipment, InventorySlot> ����
        foreach (var search in equippedEquipmentSlotDictionary)
        {
            //����Ѿ�װ������ͬ���͵�װ������¼��ǰ����װ��װ��
            if (search.Key.equipmentType == _newEquipmentToEquip.equipmentType)
            {
                _oldEquippedEquipment = search.Key;
            }
        }

        //����о�װ���ѱ�װ��������ж�²���
        if (_oldEquippedEquipment != null)
        {
            //ж�¾�װ�����Ƴ����������
            UnequipEquipmentWithoutAddingBackToInventory(_oldEquippedEquipment);

            AddItem(_oldEquippedEquipment); //���µ�װ���Żر���
        }

        //���µ�װ������ӵ���װ����װ�����б���
        equippedEquipmentSlotList.Add(newEquipmentSlot);

        //���µ�װ����װ���۵Ķ�Ӧ��ϵ��ӵ��ֵ���
        equippedEquipmentSlotDictionary.Add(_newEquipmentToEquip, newEquipmentSlot);

        //Ϊ��װ��Ӧ���丽�ӵ����Ի����η�
        _newEquipmentToEquip.AddModifiers();

        RemoveItem(_newEquipmentToEquip);   //�ӱ������Ƴ���װ��

        //UpdateInventoryAndStashUI();

        //�����װ����ҩˮƿ��Flask���������UI��ʾҩˮ��ͼ��
        if (_newEquipmentToEquip.equipmentType == EquipmentType.Flask)
        {
            Flask_UI.instance.SetFlaskImage(_newEquipmentToEquip);
        }
    }


    //ж��װ������������ӻر���
    public void UnequipEquipmentWithoutAddingBackToInventory(ItemData_Equipment _equipmentToRemove)
    {
        //���Դ���װ����װ���ֵ��в���ָ����װ��,TryGetValue�������ڰ�ȫ�ػ�ȡ�ֵ���ָ������ֵ
        if (equippedEquipmentSlotDictionary.TryGetValue(_equipmentToRemove, out InventorySlot value))
        {
            //����װ����װ�����б����Ƴ���װ��
            equippedEquipmentSlotList.Remove(value);

            //����װ����װ���ֵ����Ƴ���װ����װ���۵Ķ�Ӧ��ϵ
            equippedEquipmentSlotDictionary.Remove(_equipmentToRemove);

            //�Ƴ���װ�������η��򸽼�����
            _equipmentToRemove.RemoveModifiers();
        }

        //���ж�µ�װ����ҩˮƿ��Flask���������UI��ʾΪ��
        if (_equipmentToRemove.equipmentType == EquipmentType.Flask)
        {
            Flask_UI.instance.SetFlaskImage(null);
        }
    }

    //��鱳���ռ�
    public bool CanAddEquipmentToInventory()
    {
        //��������е���Ʒ������
        if (inventorySlotList.Count >= inventorySlotUI.Length)
        {
            Debug.Log("No more space in inventory");  //��������ռ���������ʾ
            return false;  
        }

        //����������п�λ������ true����ʾ�������װ��
        return true;
    }

    //�����Ʒ������
    public void AddItem(ItemData _item)
    {
        //�����Ʒ��װ���ұ����пռ䣬��װ����ӵ�����
        if (_item.itemType == ItemType.Equipment && CanAddEquipmentToInventory())
        {
            //�����װ���Ѿ��ڱ����У�������ѵ���
            if (inventorySlotDictionary.TryGetValue(_item, out InventorySlot value))
            {
                value.AddStack();
            }
            else  //�����װ�����ڱ����У�������ӵ�����
            {
                InventorySlot newItem = new InventorySlot(_item);  // �����µ�װ���۲���ʼ����ѵ���Ϊ 1
                inventorySlotList.Add(newItem);  // ���µ�װ������ӵ������б���
                inventorySlotDictionary.Add(_item, newItem);  // ���ֵ�����Ӹ�װ������۵�ӳ��
            }
        }
        //�����Ʒ�ǲ��ϣ��򽫲�����ӵ����
        else if (_item.itemType == ItemType.Material)
        {
            //���������Ѵ��ڸò��ϣ���������ѵ���
            if (stashSlotDictionary.TryGetValue(_item, out InventorySlot value))
            {
                value.AddStack();
            }
            else  // ��������û�иò��ϣ��򴴽��µĿ��۲����
            {
                InventorySlot newItem = new InventorySlot(_item);  //��ʼ���µĿ���
                stashSlotList.Add(newItem);  //���µĿ�����ӵ�����б���
                stashSlotDictionary.Add(_item, newItem);  //���ֵ�����Ӹ���Ʒ����۵�ӳ��
            }
        }

        UpdateAllSlotUI();
    }

    //�ӱ����������Ƴ���Ʒ
    public void RemoveItem(ItemData _item)
    {
        //�ӱ����в�����Ʒ
        if (inventorySlotDictionary.TryGetValue(_item, out InventorySlot value))        //out 
        {
            //��Ʒ<=1�������б���ֵ����Ƴ�����Ʒ
            if (value.stackSize <= 1)
            {
                inventorySlotList.Remove(value);  //�ӱ����б����Ƴ�����Ʒ��
                inventorySlotDictionary.Remove(_item);  //���ֵ����Ƴ�����Ʒ��۵�ӳ��
            }
            else 
            {
                value.RemoveStack(); 
            }
        }

        //�ӿ���в�����Ʒ
        if (stashSlotDictionary.TryGetValue(_item, out InventorySlot stashValue))
        {
            //��Ʒ<=1�ӿ���б���ֵ����Ƴ�����Ʒ
            if (stashValue.stackSize <= 1)
            {
                stashSlotList.Remove(stashValue);  // �ӿ���б����Ƴ�����Ʒ��
                stashSlotDictionary.Remove(_item);  // ���ֵ����Ƴ�����Ʒ��۵�ӳ��
            }
            else  //����ٶѵ�����
            {
                stashValue.RemoveStack();  
            }
        }

        // ����������Ʒ�۵�UI��ʾ
        UpdateAllSlotUI();
    }

    //��ȡ��װ����װ���б�
    public List<InventorySlot> GetEquippedEquipmentList()
    {
        return equippedEquipmentSlotList; 
    }

    //��ȡ����е���Ʒ�б�
    public List<InventorySlot> GetStashList()
    {
        return stashSlotList; 
    }

    //����װ�����ͻ�ȡ��װ����װ��
    public ItemData_Equipment GetEquippedEquipmentByType(EquipmentType _type)
    {
        ItemData_Equipment equippedEquipment = null;

        //������װ��װ�����ֵ䣬����ƥ���װ������
        foreach (KeyValuePair<ItemData_Equipment, InventorySlot> search in equippedEquipmentSlotDictionary)
        {
            if (search.Key.equipmentType == _type)  //����ҵ�ƥ���װ������
            {
                equippedEquipment = search.Key;  //���ҵ���װ����ֵ�����ص�װ������
            }
        }

        return equippedEquipment;  //����ƥ���װ����û���ҵ���Ϊnull
    }

    //����������Ʒ�ϳ���װ��
    public bool CraftIfAvailable(ItemData_Equipment _equipmentToCraft, List<InventorySlot> _requiredMaterials)
    {
        List<InventorySlot> materialsToRemove = new List<InventorySlot>();  //���ڴ洢��Ҫ�Ƴ��Ĳ���

        //������������б�
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            //��������Ƿ����������
            if (stashSlotDictionary.TryGetValue(_requiredMaterials[i].item, out InventorySlot stashValue))
            {
                //�������иò��ϵĶѵ�������������������
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("Not enough materials");  // �����ʾ��Ϣ
                    return false;  //������ϲ��㣬���� false
                }
                else 
                {
                    //��������ϼ����Ƴ��б�
                    for (int k = 0; k < _requiredMaterials[i].stackSize; k++)
                    {
                        materialsToRemove.Add(stashValue);  //�����ϼ����б�׼���Ƴ�
                    }
                }
            }
            else 
            {
                Debug.Log("Not enough materials"); 
                return false; 
            }
        }

        //�����Ƴ��Ĳ����б���һ�ӿ�����Ƴ���Ӧ��Ʒ
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].item); 
        }

        //���ºϳɵ�װ����ӵ�����������
        AddItem(_equipmentToCraft);
        Debug.Log($"Crafted {_equipmentToCraft.itemName}");  //����ϳɳɹ�����Ϣ

        return true; 
    }

    //ҩˮFlask
    public void UseFlask_ConsiderCooldown(Transform _spawnTransform)
    {
        //��ȡ��װ��Flask
        ItemData_Equipment flask = GetEquippedEquipmentByType(EquipmentType.Flask);

        if (flask == null)
        {
            return; 
        }

        //����ҩˮ��Ч��������spawnTransform
        flask.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //����Ч��
    public void UseArmorEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        //��ȡ��ǰװ������
        ItemData_Equipment armor = GetEquippedEquipmentByType(EquipmentType.Armor);

        if (armor == null)
        {
            return;
        }

        //ִ�п���Ч������������Ч����λ��
        armor.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //ʹ�ý���Ч��
    public void UseSwordEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        // ��ȡ��ǰװ���Ľ�
        ItemData_Equipment sword = GetEquippedEquipmentByType(EquipmentType.Weapon);

        // ���û��װ������ֱ�ӷ���
        if (sword == null)
        {
            return;
        }

        //ִ�н���Ч������������Ч����λ��
        sword.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //�ͷŽ��İ���Ч��
    public void ReleaseSwordArcane_ConsiderCooldown()
    {
        //��ȡ��ǰװ���Ľ�
        ItemData_Equipment sword = GetEquippedEquipmentByType(EquipmentType.Weapon);

        if (sword == null)
        {
            return;
        }

        //�ͷŽ��İ���Ч��
        sword.ReleaseSwordArcane_ConsiderCooldown();
    }

    //ʹ�û����Ч��
    public void UseCharmEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        //��ȡ��ǰװ�������
        ItemData_Equipment charm = GetEquippedEquipmentByType(EquipmentType.Charm);

        if (charm == null)
        {
            return;
        }

        //ִ�л������Ч������������Ч����λ��
        charm.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //ˢ������װ����ʹ��״̬
    private void RefreshEquipmentEffectUseState()
    {
        //�������б����е�װ����ˢ��ÿ��װ����ʹ��״̬
        foreach (var search in inventorySlotDictionary)
        {
            var equipment = search.Key as ItemData_Equipment;
            equipment.RefreshUseState();  //ˢ��װ��ʹ��״̬
        }

        //����������װ����װ����ˢ��ÿ��װ����ʹ��״̬
        foreach (var search in equippedEquipmentSlotDictionary)
        {
            var equipment = search.Key as ItemData_Equipment;
            equipment.RefreshUseState();  //ˢ��װ����ʹ��״̬
        }
    }

    //������Ϸ����
    public void LoadData(GameData _data)
    {
        //���ر������ݣ�itemID �� stackSize��
        foreach (var pair in _data.inventory)
        {
            //�������е���Ʒ���ݣ����Ҵ浵�е�itemIDƥ�����Ʒ
            foreach (var item in itemDataBase)
            {
                // �����Ʒ���ڣ����� itemID ƥ��
                if (item != null && item.itemID == pair.Key)
                {
                    // ����һ���µ���Ʒ�ۣ������öѵ�����
                    InventorySlot slotToLoad = new InventorySlot(item);
                    slotToLoad.stackSize = pair.Value; // ���ø���Ʒ�Ķѵ�����

                    // ����Ʒ����ӵ��Ѽ��صı������б���
                    loadedInventorySlots.Add(slotToLoad);
                }
            }
        }

        //������װ����װ������
        foreach (var equipmentID in _data.equippedEquipmentIDs)
        {
            //����������Ʒ���ݣ�������浵�е�װ�� ID ƥ���װ��
            foreach (var equipment in itemDataBase)
            {
                //�����Ʒ���ڣ�itemIDƥ��
                if (equipment != null && equipment.itemID == equipmentID)
                {
                    //װ����Ʒ��ӵ��Ѽ��ص�װ���б���
                    loadedEquippedEquipment.Add(equipment as ItemData_Equipment);
                }
            }
        }
    }

    //������Ϸ����
    public void SaveData(ref GameData _data)
    {
        //������еı������ݣ���ֹÿ�μ�����Ϸʱ���������ظ�
        _data.inventory.Clear();
        _data.equippedEquipmentIDs.Clear();

        //����ǰ�����е���Ʒ���ݱ��浽��Ϸ������
        foreach (KeyValuePair<ItemData, InventorySlot> search in inventorySlotDictionary)
        {
            //������Ʒ��itemID������
            _data.inventory.Add(search.Key.itemID, search.Value.stackSize);
        }

        //����ǰ�������е���Ʒ���ݱ��浽��Ϸ������
        foreach (var search in stashSlotDictionary)
        {
            //������Ʒ��itemID�Ͷѵ�����
            _data.inventory.Add(search.Key.itemID, search.Value.stackSize);
        }

        //����ǰ��װ����װ�����ݱ��浽��Ϸ������
        foreach (var search in equippedEquipmentSlotDictionary)
        {
            //����װ����ƷitemID
            _data.equippedEquipmentIDs.Add(search.Key.itemID);
        }
    }

#if UNITY_EDITOR
    //�༭��ģʽ�µĲ˵�����������Ʒ���ݿ�
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase()
    {
        //��ȡ������Ʒ���ݲ���䵽���ݿ���
        itemDataBase = GetItemDataBase();
    }

    // ��ȡ������Ʒ���ݵ��б�
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();

        // ��ȡָ��·���µ�������Ʒ�ʲ�����
        // AssetDatabase.FindAssets ���ڲ���ָ��·���µ�������Դ�����ص��� GUID ����
        string[] assetNames = AssetDatabase.FindAssets("", new string[] { "Assets/ItemData/Items" });

        //�����ʲ�����
        foreach (string SOName in assetNames)
        {
            //��ȡ����Ʒ·��
            var SOPath = AssetDatabase.GUIDToAssetPath(SOName);
            //����·���µ���Ʒ����
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOPath);
            //������Ʒ������ӵ���Ʒ���ݿ���
            itemDataBase.Add(itemData);
        }

        //�������õ���Ʒ�����б�
        return itemDataBase;
    }
#endif


}
