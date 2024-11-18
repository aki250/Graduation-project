using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Stat
{
    //基础值
    [SerializeField] private int baseValue;

    //修正值列表
    public List<int> modifiers;

    //获取最终统计值：基础值加上所有修正值的累加
    public int GetValue()
    {
        //初始为基础值
        int finalValue = baseValue;

        //遍历所有修正值，并加到基础值上，得到最终的统计值
        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        //返回计算后的最终值
        return finalValue;
    }

    //设置默认的基础值：在创建Stat对象时或需要重置基础值时使用
    public void SetDefaultValue(int _defaultValue)
    {
        baseValue = _defaultValue;
    }

    //添加修正值：将一个修正值添加到修正值列表中
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    // 移除修正值：从修正值列表中移除指定的修正值
    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
