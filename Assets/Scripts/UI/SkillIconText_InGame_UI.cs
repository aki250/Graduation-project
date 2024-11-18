using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillIconText_InGame_UI : MonoBehaviour
{
    //�洢�������ƣ����ڲ��Ҷ�Ӧ�ļ�λ�󶨣�
    [SerializeField] private string skillName;

    //���¼���ͼ���ı�
    public void UpdateSkillIconText()
    {
        //��ȡ�뼼�ܶ�Ӧ�ļ�λ��
        if (KeyBindManager.instance.keybindsDictionary.TryGetValue(skillName, out KeyCode keybind))
        {
            //����UI��ʾ��Ӧ��������
            GetComponent<TextMeshProUGUI>().text = UniformKeybindNameForInGameUI(keybind.ToString());

            //�����ǰ����Ϊ����
            if (LanguageManager.instance.localeID == 1)
            {
                GetComponent<TextMeshProUGUI>().fontSize = 22;
            }
            //�����ǰ����ΪӢ��
            else if (LanguageManager.instance.localeID == 0)
            {
                GetComponent<TextMeshProUGUI>().fontSize = 26; 
            }
        }
    }

    //ͳһ����UI�еİ������Ƹ�ʽ���������Ժͼ�λ��ʾ��
    private string UniformKeybindNameForInGameUI(string _behaveKeybind_InUI)
    {
        //�����������Alpha��ͷ�����Ƴ�
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5); //�Ƴ�ǰ׺
        }
        //�������ΪӢ��
        if (LanguageManager.instance.localeID == 0)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "LMB";
            }
            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "RMB";
            }

            //������������� "Left" ��ͷ��ȥ�� "Left" ��ʾʣ�µĲ���
            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(1, 3); // �Ƴ�Left
            }

            //ת��Ϊ��д��ʾ
            _behaveKeybind_InUI = _behaveKeybind_InUI.ToUpper();
        }

        //�������Ϊ����
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
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4); //�Ƴ�Left
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "��"); //�ڿ�ͷ������
            }
        }

        return _behaveKeybind_InUI; //���ر�׼���󰴼�����
    }
}
