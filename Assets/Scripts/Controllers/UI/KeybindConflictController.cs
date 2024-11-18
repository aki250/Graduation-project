using TMPro;
using UnityEngine;

public class KeybindConflictController : MonoBehaviour
{
    //��ʾ��Ϊ����UI
    [SerializeField] private TextMeshProUGUI behaveName_InUI;
    //��ʾ����������UI
    [SerializeField] protected TextMeshProUGUI behaveKeybind_InUI;

    //�洢��Ϊ�Ͱ�����
    private string behaveName;
    private string behaveKeybind;

    public void SetupKeybindConflict(string _behaveName, string _behaveKeybind)
    {
        //�洢�������Ϊ���ƺͰ�����
        behaveName = _behaveName;
        behaveKeybind = _behaveKeybind;

        //����UI�ı���ʾ
        behaveName_InUI.text = _behaveName;
        behaveKeybind_InUI.text = _behaveKeybind;

        //���ݵ�ǰ���Ի������벢��׼������������
        TranslateBehaveNameAndUniformBehaveKeybindName();
    }

    public void TranslateBehaveNameAndUniformBehaveKeybindName()
    {
        //�����ǰ����Ϊ���ģ�������Ϊ����
        if (LanguageManager.instance.localeID == 1)
        {
            behaveName_InUI.text = LanguageManager.instance.EnglishToChineseKeybindsDictionary[behaveName];
        }
        //�����ǰ����ΪӢ�ģ�����ʾԭʼӢ����Ϊ����
        else if (LanguageManager.instance.localeID == 0)
        {
            behaveName_InUI.text = behaveName;
        }

        //��׼�������밴��������
        behaveKeybind_InUI.text = UniformKeybindName(behaveKeybind);
    }

    private string UniformKeybindName(string _behaveKeybind_InUI)
    {
                                                //ȥ��Alphaǰ׺������У�����Ϊ��ͨ���������ְ���
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5); //ȥ��Alpha����
        }

        //������Ի���ΪӢ��
        if (LanguageManager.instance.localeID == 0)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "Mouse Left";
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "Mouse Right";
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(4, " ");
            }
        }
        // ������Ի���Ϊ����
        else if (LanguageManager.instance.localeID == 1)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "������";
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "����Ҽ�";
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4);
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "��");
            }
        }

        return _behaveKeybind_InUI;
    }
}
