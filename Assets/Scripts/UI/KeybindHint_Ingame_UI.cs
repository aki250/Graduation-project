using TMPro;
using UnityEngine;

public class KeybindHint_Ingame_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keybindText;   //��UI�ı��������ʾ��������ʾ
    [SerializeField] private string keybindTextPreset;  //������ʾ�ı�Ԥ������
    [SerializeField] private string behaveName; //����Ϊ���ƣ����ݸ����Ʋ��Ҷ�Ӧ������

    private void Start()
    {
        GetKeybindIfAvailable();  //����ʱ���Ի�ȡ��Ӧ�İ�����
    }

    private void OnEnable()
    {
        GetKeybindIfAvailable();  //���Ը��°�����
    }

    // ��ȡָ����Ϊ���ƶ�Ӧ�İ����󶨲�����UI��ʾ
    private void GetKeybindIfAvailable()
    {
        //���KeyBindManagerʵ��
        if (KeyBindManager.instance == null) 
        {
            return;
        }

        //�ֵ��а�����Ӧ����Ϊ���ƣ�������ʾ�İ�����
        if (KeyBindManager.instance.keybindsDictionary.ContainsKey(behaveName))
        {
            //��ȡ�����󶨲�ͳһ��ʽ��
            keybindText.text = UniformKeybindName(KeyBindManager.instance.keybindsDictionary[behaveName].ToString());
            keybindText.text = keybindTextPreset + keybindText.text;  //���Ԥ���ı�
        }
    }

    //ͳһ��ʽ���������ƣ�����ͬ���Ի����µĲ��죩
    private string UniformKeybindName(string _behaveKeybind_InUI)
    {
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5);  //�Ƴ�Alphaǰ׺
        }

        // ���ݵ�ǰ���Ի����������������Ƶ���ʾ
        //Ӣ��
        if (LanguageManager.instance.localeID == 0)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "LMB";  //�����Ӧ "LMB"
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "RMB";  //�Ҽ���Ӧ "RMB"
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(1, 3);  //�Ƴ� Left ǰ׺
            }
        }
        //����
        else if (LanguageManager.instance.localeID == 1)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "���";  
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "�Ҽ�";  
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4);  //�Ƴ�Leftǰ׺
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "��"); 
            }
        }

        return _behaveKeybind_InUI;  //���ظ�ʽ����İ�������
    }
}
