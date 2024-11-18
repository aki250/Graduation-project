using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindManager : MonoBehaviour, ISettingsSaveManager
{
    public static KeyBindManager instance;

    //�洢������Ϊ�ļ�λ���ֵ�
    public Dictionary<string, KeyCode> keybindsDictionary;

    // ����UI�������ʾ�͸��¼�λ��
    [SerializeField] private KeybindList_UI keybindList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //��ʼ����λ�ֵ�
        keybindsDictionary = new Dictionary<string, KeyCode>();
    }

    private void Update()
    {
    }

    // ���¼�λ�б�����ԣ����ݵ�ǰ�������ø��¼�λ UI��
    public void UpdateKeybindListLanguage()
    {
        keybindList.UpdateAllKeybindOptionsLanguage(); // ���� KeybindList_UI ��������
    }

    //����UI��ʾ����Ϊ��λ���ƣ��淶����λ���ƣ��������Բ��죩
    public string UniformKeybindName(string _behaveKeybind_InUI)
    {
        //ȥ��Alphaǰ׺
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5);
        }

        //Ӣ��
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
        //����
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

        //���ع淶����ļ�λ����
        return _behaveKeybind_InUI;
    }

    //�ӱ���������м��ؼ�λ����
    public void LoadData(SettingsData _data)
    {
        //�������ر�������ݲ���ӵ���λ�ֵ���
        foreach (var search in _data.keybindsDictionary)
        {
            keybindsDictionary.Add(search.Key, search.Value); //��ÿ����λ������ӵ��ֵ���
        }
    }

    //����ǰ�ļ�λ���ñ��浽������
    public void SaveData(ref SettingsData _data)
    {
        _data.keybindsDictionary.Clear(); //������еļ�λ�ֵ�

        //������ǰ�ֵ䣬�����м�λ���ñ��浽������
        foreach (var search in keybindsDictionary)
        {
            _data.keybindsDictionary.Add(search.Key, search.Value); //��ÿ����λ������ӵ�����������ֵ���
        }
    }
}
