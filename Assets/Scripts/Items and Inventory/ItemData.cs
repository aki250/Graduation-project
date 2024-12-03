using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,  //����
    Equipment  //װ��
}

//����һ��ScriptableObject�����ڱ�ʾ��Ϸ�е���Ʒ����
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;

    //��ƷӢ����
    public string itemName;

    //��Ʒ������
    public string itemName_Chinese;

    //��Ʒͼ��
    public Sprite icon;

    //��ƷID
    public string itemID;

    //��Ʒ���伸��
    [Range(0, 100)]
    public float dropChance;

    //����ƴ���ַ���StringBuilder��������Ʒ������Ϣ
    protected StringBuilder sb = new StringBuilder();

    //�ڱ༭�����޸�����ʱ����
    private void OnValidate()
    {
#if UNITY_EDITOR  //�ô�������Unity�༭����ִ�У�������Ϸʱ��������ⲿ�ִ���
        //��ȡ��ǰ��Ʒ���ݵ��ļ�·��
        string path = AssetDatabase.GetAssetPath(this);

        //��·��ת��ΪGUID������ֵ����ƷID����Ʒ��Ψһ��ʶ����
        itemID = AssetDatabase.AssetPathToGUID(path);  //GUID����Դ��ȫ��Ψһ��ʶ��
#endif
    }

    //������Ʒ�����Ժ�Ч����������������������д���ṩ��������
    public virtual string GetItemStatInfoAndEffectDescription()
    {
        return "";  //Ĭ�Ϸ��ؿ��ַ���
    }
}
