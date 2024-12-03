using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,  //材料
    Equipment  //装备
}

//创建一个ScriptableObject，用于表示游戏中的物品数据
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;

    //物品英文名
    public string itemName;

    //物品中文名
    public string itemName_Chinese;

    //物品图标
    public Sprite icon;

    //物品ID
    public string itemID;

    //物品掉落几率
    [Range(0, 100)]
    public float dropChance;

    //用于拼接字符串StringBuilder，构建物品描述信息
    protected StringBuilder sb = new StringBuilder();

    //在编辑器中修改数据时调用
    private void OnValidate()
    {
#if UNITY_EDITOR  //该代码块仅在Unity编辑器中执行，构建游戏时不会编译这部分代码
        //获取当前物品数据的文件路径
        string path = AssetDatabase.GetAssetPath(this);

        //将路径转换为GUID，并赋值给物品ID（物品的唯一标识符）
        itemID = AssetDatabase.AssetPathToGUID(path);  //GUID是资源的全局唯一标识符
#endif
    }

    //返回物品的属性和效果描述，可以在子类中重写以提供具体描述
    public virtual string GetItemStatInfoAndEffectDescription()
    {
        return "";  //默认返回空字符串
    }
}
