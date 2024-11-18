using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//可序列化字典类，继承Dictionary<TKey, TValue> ,实现ISerializationCallbackReceiver接口
//允许将字典数据序列化和反序列化，便于在Unity编辑器中显示和保存
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    //存储字典的键列表
    [SerializeField] private List<TKey> keys = new List<TKey>();

    //存储字典的值列表
    [SerializeField] private List<TValue> values = new List<TValue>();

    //在序列化之前调用，将字典中的键值对分别保存到 keys 和 values 列表中
    public void OnBeforeSerialize()
    {
        //清空现有keys和values
        keys.Clear();
        values.Clear();

        //重新遍历将键和值分别添加到对应的列表中
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    //在反序列化之后调用，将keys和values列表中的数据恢复到字典中
    public void OnAfterDeserialize()
    {
        //清空当前字典
        this.Clear();

        //检查keys和values列表的数量是否匹配
        if (keys.Count != values.Count)
        {
            //如果不匹配，输出错误信息
            Debug.Log("Error! \nNumber of keys is not equal to number of values!");
        }

        //将key和values列表中的数据重新添加到字典中
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
