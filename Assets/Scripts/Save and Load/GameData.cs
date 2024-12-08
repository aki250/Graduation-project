using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

[System.Serializable]
public class GameData
{
    //��ǰ��ҵĻ�������
    public int currecny;

    //���������ݣ��洢�����������״̬
    public SerializableDictionary<string, bool> skillTree;

    //��ұ�����Ʒ���洢��ƷID��ѵ�����
    public SerializableDictionary<string, int> inventory;

    //���װ��ID�б�
    public List<string> equippedEquipmentIDs;

    //�洢���ļ���ID�ͼ���״̬
    public SerializableDictionary<string, bool> checkpointsDictionary;

    //����������ID
    public string closestActivatedCheckpointID;

    //��󼤻����ID
    public string lastActivatedCheckpointID;

    [Header("�����������")]
    //�������ʱ�����������
    public int droppedCurrencyAmount;

    //�������λ��
    public Vector2 deathPosition;

    [Header("��ͼԪ��")]
    //��ʹ�õĵ�ͼԪ��ID�б���̽���������Ѵ������¼��ȣ�
    public List<int> UsedMapElementIDList;

    public bool isNew;

    //���캯������ʼ�����б���Ĭ��ֵ
    public GameData()
    {
        //��Ϸ�������
        this.currecny = 0;  //��ʼ����Ϊ0
        this.droppedCurrencyAmount = 0;  //����ʱ����Ļ�������Ĭ��Ϊ0
        this.deathPosition = Vector2.zero;  //�������ʱ��λ��Ĭ��Ϊ(0,0)

        //��ʼ����ʹ�õ�ͼԪ�ص�ID�б�
        UsedMapElementIDList = new List<int>();

        //��ʼ��������
        skillTree = new SerializableDictionary<string, bool>();

        //��ʼ��������Ʒ
        inventory = new SerializableDictionary<string, int>();

        //��ʼ����װ����ƷID�б�
        equippedEquipmentIDs = new List<string>();

        //��ʼ�������ֵ�
        checkpointsDictionary = new SerializableDictionary<string, bool>();

        //��ʼ���������󼤻�ļ���IDΪ��
        closestActivatedCheckpointID = string.Empty;
        lastActivatedCheckpointID = string.Empty;
    }
}
