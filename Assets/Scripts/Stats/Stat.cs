using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Stat
{
    //����ֵ
    [SerializeField] private int baseValue;

    //����ֵ�б�
    public List<int> modifiers;

    //��ȡ����ͳ��ֵ������ֵ������������ֵ���ۼ�
    public int GetValue()
    {
        //��ʼΪ����ֵ
        int finalValue = baseValue;

        //������������ֵ�����ӵ�����ֵ�ϣ��õ����յ�ͳ��ֵ
        foreach (int modifier in modifiers)
        {
            finalValue += modifier;
        }

        //���ؼ���������ֵ
        return finalValue;
    }

    //����Ĭ�ϵĻ���ֵ���ڴ���Stat����ʱ����Ҫ���û���ֵʱʹ��
    public void SetDefaultValue(int _defaultValue)
    {
        baseValue = _defaultValue;
    }

    //�������ֵ����һ������ֵ��ӵ�����ֵ�б���
    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    // �Ƴ�����ֵ��������ֵ�б����Ƴ�ָ��������ֵ
    public void RemoveModifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }
}
