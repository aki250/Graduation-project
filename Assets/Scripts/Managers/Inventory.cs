using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Inventory : MonoBehaviour, IGameProgressionSaveManager
{
    public static Inventory instance;

    //装备
    public List<InventorySlot> inventorySlotList;
    public Dictionary<ItemData, InventorySlot> inventorySlotDictionary;

    //材料
    public List<InventorySlot> stashSlotList;
    public Dictionary<ItemData, InventorySlot> stashSlotDictionary;

    //装备物品
    public List<InventorySlot> equippedEquipmentSlotList;
    public Dictionary<ItemData_Equipment, InventorySlot> equippedEquipmentSlotDictionary;


    public List<ItemData> startItems;   //初始物品

    [Header("物品UI")]
    [SerializeField] private Transform referenceInventory;  //物品栏UI
    [SerializeField] private Transform referenceStash;  //材料的UI
    [SerializeField] private Transform referenceEquippedEquipments; //装备栏UI
    [SerializeField] private Transform referenceStatPanel;  //角色属性面板UI

    private InventorySlot_UI[] inventorySlotUI;
    private InventorySlot_UI[] stashSlotUI;
    private EquippedEquipmentSlot_UI[] equippedEquipmentSlotUI;
    private StatSlot_UI[] statSlotUI;

    //private float flaskLastUseTime;
    //private bool flaskUsed = false;


    [Header("物品数据库")]
    public List<ItemData> itemDataBase; //物品数据基础库
    public List<InventorySlot> loadedInventorySlots;    //加载物品栏数据
    public List<ItemData_Equipment> loadedEquippedEquipment;    //加载装备数据

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
        //初始化库存、背包、装备槽及其字典
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

        AddStartItems();    //添加初始物品
        RefreshEquipmentEffectUseState();   //刷新装备效果状态
    }

    private void AddStartItems()
    {
        //加载初始装备物品
        foreach (var equipment in loadedEquippedEquipment)
        {
            EquipItem(equipment);
        }

        //加载库存物品
        if (loadedInventorySlots.Count > 0)
        {
            foreach (var slot in loadedInventorySlots)
            {
                for (int i = 0; i < slot.stackSize; i++)
                {
                    AddItem(slot.item); //向库存中添加物品
                }
            }

            return;
        }
        //若没有加载的物品，则添加初始物品
        for (int i = 0; i < startItems.Count; i++)
        {
            if (startItems[i] != null)
            {
                AddItem(startItems[i]); //添加初始物品
            }
        }
    }

    private void Update()
    {
    }

    //更新所有UI槽的显示
    private void UpdateAllSlotUI()
    {
        //清理所有UI槽，以确保没有额外的UI
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

        //更新库存UI
        for (int i = 0; i < inventorySlotList.Count; i++)
        {
            inventorySlotUI[i].UpdateInventorySlotUI(inventorySlotList[i]);
        }

        //更新材料栏UI
        for (int i = 0; i < stashSlotList.Count; i++)
        {
            stashSlotUI[i].UpdateInventorySlotUI(stashSlotList[i]);
        }

        //遍历所有装备栏的UI槽
        for (int i = 0; i < equippedEquipmentSlotUI.Length; i++)
        {
            //遍历装备栏的装备字典，查找是否有匹配类型的装备
            foreach (var search in equippedEquipmentSlotDictionary)
            {
                //如果装备类型与当前UI槽的类型匹配
                if (search.Key.equipmentType == equippedEquipmentSlotUI[i].equipmentType)
                {
                    //更新UI槽的显示，传入匹配的装备槽
                    equippedEquipmentSlotUI[i].UpdateInventorySlotUI(search.Value);
                }
            }
        }

        UpdateStatUI();     //更新角色面板的统计信息

    }

    public void UpdateStatUI()
    {
        for (int i = 0; i < statSlotUI.Length; i++)
        {
            statSlotUI[i].UpdateStatValue_UI();
        }
    }

    //装备
    public void EquipItem(ItemData _item)
    {
        //将传入的ItemData类型物品，转换为ItemData_Equipment类型
        ItemData_Equipment _newEquipmentToEquip = _item as ItemData_Equipment;

        //创建新的装备槽并为其分配新装备
        InventorySlot newEquipmentSlot = new InventorySlot(_newEquipmentToEquip);

        //存储已装备的旧装备
        ItemData_Equipment _oldEquippedEquipment = null;

        //遍历字典中的所有已装备装备，查找是否已装备相同类型的装备
        //这里的var search是 KeyValuePair<ItemData_Equipment, InventorySlot> 类型
        foreach (var search in equippedEquipmentSlotDictionary)
        {
            //如果已经装备了相同类型的装备，记录当前的已装备装备
            if (search.Key.equipmentType == _newEquipmentToEquip.equipmentType)
            {
                _oldEquippedEquipment = search.Key;
            }
        }

        //如果有旧装备已被装备，进行卸下操作
        if (_oldEquippedEquipment != null)
        {
            //卸下旧装备并移除其相关数据
            UnequipEquipmentWithoutAddingBackToInventory(_oldEquippedEquipment);

            AddItem(_oldEquippedEquipment); //脱下的装备放回背包
        }

        //将新的装备槽添加到已装备的装备槽列表中
        equippedEquipmentSlotList.Add(newEquipmentSlot);

        //将新的装备与装备槽的对应关系添加到字典中
        equippedEquipmentSlotDictionary.Add(_newEquipmentToEquip, newEquipmentSlot);

        //为新装备应用其附加的属性或修饰符
        _newEquipmentToEquip.AddModifiers();

        RemoveItem(_newEquipmentToEquip);   //从背包中移除新装备

        //UpdateInventoryAndStashUI();

        //如果新装备是药水瓶（Flask），则更新UI显示药水的图标
        if (_newEquipmentToEquip.equipmentType == EquipmentType.Flask)
        {
            Flask_UI.instance.SetFlaskImage(_newEquipmentToEquip);
        }
    }


    //卸下装备但不将其添加回背包
    public void UnequipEquipmentWithoutAddingBackToInventory(ItemData_Equipment _equipmentToRemove)
    {
        //尝试从已装备的装备字典中查找指定的装备,TryGetValue方法用于安全地获取字典中指定键的值
        if (equippedEquipmentSlotDictionary.TryGetValue(_equipmentToRemove, out InventorySlot value))
        {
            //从已装备的装备槽列表中移除该装备
            equippedEquipmentSlotList.Remove(value);

            //从已装备的装备字典中移除该装备与装备槽的对应关系
            equippedEquipmentSlotDictionary.Remove(_equipmentToRemove);

            //移除该装备的修饰符或附加属性
            _equipmentToRemove.RemoveModifiers();
        }

        //如果卸下的装备是药水瓶（Flask），则更新UI显示为空
        if (_equipmentToRemove.equipmentType == EquipmentType.Flask)
        {
            Flask_UI.instance.SetFlaskImage(null);
        }
    }

    //检查背包空间
    public bool CanAddEquipmentToInventory()
    {
        //如果背包中的物品槽数满
        if (inventorySlotList.Count >= inventorySlotUI.Length)
        {
            Debug.Log("No more space in inventory");  //输出背包空间已满的提示
            return false;  
        }

        //如果背包还有空位，返回 true，表示可以添加装备
        return true;
    }

    //添加物品到背包
    public void AddItem(ItemData _item)
    {
        //如果物品是装备且背包有空间，则将装备添加到背包
        if (_item.itemType == ItemType.Equipment && CanAddEquipmentToInventory())
        {
            //如果该装备已经在背包中，增加其堆叠数
            if (inventorySlotDictionary.TryGetValue(_item, out InventorySlot value))
            {
                value.AddStack();
            }
            else  //如果该装备不在背包中，则将其添加到背包
            {
                InventorySlot newItem = new InventorySlot(_item);  // 创建新的装备槽并初始化其堆叠数为 1
                inventorySlotList.Add(newItem);  // 将新的装备槽添加到背包列表中
                inventorySlotDictionary.Add(_item, newItem);  // 在字典中添加该装备与库存槽的映射
            }
        }
        //如果物品是材料，则将材料添加到库存
        else if (_item.itemType == ItemType.Material)
        {
            //如果库存中已存在该材料，则增加其堆叠数
            if (stashSlotDictionary.TryGetValue(_item, out InventorySlot value))
            {
                value.AddStack();
            }
            else  // 如果库存中没有该材料，则创建新的库存槽并添加
            {
                InventorySlot newItem = new InventorySlot(_item);  //初始化新的库存槽
                stashSlotList.Add(newItem);  //将新的库存槽添加到库存列表中
                stashSlotDictionary.Add(_item, newItem);  //在字典中添加该物品与库存槽的映射
            }
        }

        UpdateAllSlotUI();
    }

    //从背包或库存中移除物品
    public void RemoveItem(ItemData _item)
    {
        //从背包中查找物品
        if (inventorySlotDictionary.TryGetValue(_item, out InventorySlot value))        //out 
        {
            //物品<=1，背包列表和字典中移除该物品
            if (value.stackSize <= 1)
            {
                inventorySlotList.Remove(value);  //从背包列表中移除该物品槽
                inventorySlotDictionary.Remove(_item);  //从字典中移除该物品与槽的映射
            }
            else 
            {
                value.RemoveStack(); 
            }
        }

        //从库存中查找物品
        if (stashSlotDictionary.TryGetValue(_item, out InventorySlot stashValue))
        {
            //物品<=1从库存列表和字典中移除该物品
            if (stashValue.stackSize <= 1)
            {
                stashSlotList.Remove(stashValue);  // 从库存列表中移除该物品槽
                stashSlotDictionary.Remove(_item);  // 从字典中移除该物品与槽的映射
            }
            else  //则减少堆叠数量
            {
                stashValue.RemoveStack();  
            }
        }

        // 更新所有物品槽的UI显示
        UpdateAllSlotUI();
    }

    //获取已装备的装备列表
    public List<InventorySlot> GetEquippedEquipmentList()
    {
        return equippedEquipmentSlotList; 
    }

    //获取库存中的物品列表
    public List<InventorySlot> GetStashList()
    {
        return stashSlotList; 
    }

    //根据装备类型获取已装备的装备
    public ItemData_Equipment GetEquippedEquipmentByType(EquipmentType _type)
    {
        ItemData_Equipment equippedEquipment = null;

        //遍历已装备装备的字典，查找匹配的装备类型
        foreach (KeyValuePair<ItemData_Equipment, InventorySlot> search in equippedEquipmentSlotDictionary)
        {
            if (search.Key.equipmentType == _type)  //如果找到匹配的装备类型
            {
                equippedEquipment = search.Key;  //将找到的装备赋值给返回的装备对象
            }
        }

        return equippedEquipment;  //返回匹配的装备，没有找到则为null
    }

    //根据需求物品合成新装备
    public bool CraftIfAvailable(ItemData_Equipment _equipmentToCraft, List<InventorySlot> _requiredMaterials)
    {
        List<InventorySlot> materialsToRemove = new List<InventorySlot>();  //用于存储将要移除的材料

        //遍历所需材料列表
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            //检查库存中是否有所需材料
            if (stashSlotDictionary.TryGetValue(_requiredMaterials[i].item, out InventorySlot stashValue))
            {
                //如果库存中该材料的堆叠数量不足以满足需求
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("Not enough materials");  // 输出提示信息
                    return false;  //如果材料不足，返回 false
                }
                else 
                {
                    //将所需材料加入移除列表
                    for (int k = 0; k < _requiredMaterials[i].stackSize; k++)
                    {
                        materialsToRemove.Add(stashValue);  //将材料加入列表，准备移除
                    }
                }
            }
            else 
            {
                Debug.Log("Not enough materials"); 
                return false; 
            }
        }

        //遍历移除的材料列表，逐一从库存中移除相应物品
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].item); 
        }

        //将新合成的装备添加到背包或库存中
        AddItem(_equipmentToCraft);
        Debug.Log($"Crafted {_equipmentToCraft.itemName}");  //输出合成成功的信息

        return true; 
    }

    //药水Flask
    public void UseFlask_ConsiderCooldown(Transform _spawnTransform)
    {
        //获取已装备Flask
        ItemData_Equipment flask = GetEquippedEquipmentByType(EquipmentType.Flask);

        if (flask == null)
        {
            return; 
        }

        //调用药水的效果，传递spawnTransform
        flask.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //盔甲效果
    public void UseArmorEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        //获取当前装备盔甲
        ItemData_Equipment armor = GetEquippedEquipmentByType(EquipmentType.Armor);

        if (armor == null)
        {
            return;
        }

        //执行盔甲效果，传递生成效果的位置
        armor.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //使用剑的效果
    public void UseSwordEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        // 获取当前装备的剑
        ItemData_Equipment sword = GetEquippedEquipmentByType(EquipmentType.Weapon);

        // 如果没有装备剑，直接返回
        if (sword == null)
        {
            return;
        }

        //执行剑的效果，传递生成效果的位置
        sword.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //释放剑的奥术效果
    public void ReleaseSwordArcane_ConsiderCooldown()
    {
        //获取当前装备的剑
        ItemData_Equipment sword = GetEquippedEquipmentByType(EquipmentType.Weapon);

        if (sword == null)
        {
            return;
        }

        //释放剑的奥术效果
        sword.ReleaseSwordArcane_ConsiderCooldown();
    }

    //使用护身符效果
    public void UseCharmEffect_ConsiderCooldown(Transform _spawnTransform)
    {
        //获取当前装备护身符
        ItemData_Equipment charm = GetEquippedEquipmentByType(EquipmentType.Charm);

        if (charm == null)
        {
            return;
        }

        //执行护身符的效果，传递生成效果的位置
        charm.ExecuteItemEffect_ConsiderCooldown(_spawnTransform);
    }

    //刷新所有装备的使用状态
    private void RefreshEquipmentEffectUseState()
    {
        //遍历所有背包中的装备，刷新每个装备的使用状态
        foreach (var search in inventorySlotDictionary)
        {
            var equipment = search.Key as ItemData_Equipment;
            equipment.RefreshUseState();  //刷新装备使用状态
        }

        //遍历所有已装备的装备，刷新每个装备的使用状态
        foreach (var search in equippedEquipmentSlotDictionary)
        {
            var equipment = search.Key as ItemData_Equipment;
            equipment.RefreshUseState();  //刷新装备的使用状态
        }
    }

    //加载游戏数据
    public void LoadData(GameData _data)
    {
        //加载背包数据（itemID 和 stackSize）
        foreach (var pair in _data.inventory)
        {
            //遍历所有的物品数据，查找存档中的itemID匹配的物品
            foreach (var item in itemDataBase)
            {
                // 如果物品存在，并且 itemID 匹配
                if (item != null && item.itemID == pair.Key)
                {
                    // 创建一个新的物品槽，并设置堆叠数量
                    InventorySlot slotToLoad = new InventorySlot(item);
                    slotToLoad.stackSize = pair.Value; // 设置该物品的堆叠数量

                    // 将物品槽添加到已加载的背包槽列表中
                    loadedInventorySlots.Add(slotToLoad);
                }
            }
        }

        //加载已装备的装备数据
        foreach (var equipmentID in _data.equippedEquipmentIDs)
        {
            //遍历所有物品数据，查找与存档中的装备 ID 匹配的装备
            foreach (var equipment in itemDataBase)
            {
                //如果物品存在，itemID匹配
                if (equipment != null && equipment.itemID == equipmentID)
                {
                    //装备物品添加到已加载的装备列表中
                    loadedEquippedEquipment.Add(equipment as ItemData_Equipment);
                }
            }
        }
    }

    //保存游戏数据
    public void SaveData(ref GameData _data)
    {
        //清空已有的背包数据，防止每次加载游戏时背包数据重复
        _data.inventory.Clear();
        _data.equippedEquipmentIDs.Clear();

        //将当前背包中的物品数据保存到游戏数据中
        foreach (KeyValuePair<ItemData, InventorySlot> search in inventorySlotDictionary)
        {
            //保存物品的itemID和数量
            _data.inventory.Add(search.Key.itemID, search.Value.stackSize);
        }

        //将当前储藏室中的物品数据保存到游戏数据中
        foreach (var search in stashSlotDictionary)
        {
            //保存物品的itemID和堆叠数量
            _data.inventory.Add(search.Key.itemID, search.Value.stackSize);
        }

        //将当前已装备的装备数据保存到游戏数据中
        foreach (var search in equippedEquipmentSlotDictionary)
        {
            //保存装备物品itemID
            _data.equippedEquipmentIDs.Add(search.Key.itemID);
        }
    }

#if UNITY_EDITOR
    //编辑器模式下的菜单项，用于填充物品数据库
    [ContextMenu("Fill up item data base")]
    private void FillUpItemDataBase()
    {
        //获取所有物品数据并填充到数据库中
        itemDataBase = GetItemDataBase();
    }

    // 获取所有物品数据的列表
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();

        // 获取指定路径下的所有物品资产名称
        // AssetDatabase.FindAssets 用于查找指定路径下的所有资源，返回的是 GUID 数组
        string[] assetNames = AssetDatabase.FindAssets("", new string[] { "Assets/ItemData/Items" });

        //遍历资产名称
        foreach (string SOName in assetNames)
        {
            //获取该物品路径
            var SOPath = AssetDatabase.GUIDToAssetPath(SOName);
            //加载路径下的物品数据
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOPath);
            //加载物品数据添加到物品数据库中
            itemDataBase.Add(itemData);
        }

        //返回填充好的物品数据列表
        return itemDataBase;
    }
#endif


}
