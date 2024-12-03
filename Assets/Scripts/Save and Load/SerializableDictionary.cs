using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����л��ֵ��࣬�̳�Dictionary<TKey, TValue> ,ʵ��ISerializationCallbackReceiver�ӿ�
//�����ֵ��������л��ͷ����л���������Unity�༭������ʾ�ͱ���
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    //�洢�ֵ�ļ��б�
    [SerializeField] private List<TKey> keys = new List<TKey>();

    //�洢�ֵ��ֵ�б�
    [SerializeField] private List<TValue> values = new List<TValue>();

    //�����л�֮ǰ���ã����ֵ��еļ�ֵ�Էֱ𱣴浽keys��values�б���
    public void OnBeforeSerialize()
    {
        //�������keys��values
        keys.Clear();
        values.Clear();

        //���±���������ֵ�ֱ���ӵ���Ӧ���б���
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    //�ڷ����л�֮����ã���keys��values�б��е����ݻָ����ֵ���
    public void OnAfterDeserialize()
    {
        //��յ�ǰ�ֵ�
        this.Clear();

        //���keys��values�б�������Ƿ�ƥ��
        if (keys.Count != values.Count)
        {
            //�����ƥ�䣬���������Ϣ
            Debug.Log("����! \n key��values��ƥ��");
        }

        //��key��values�б��е�����������ӵ��ֵ���
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
