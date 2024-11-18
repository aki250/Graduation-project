using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindManager : MonoBehaviour, ISettingsSaveManager
{
    public static KeyBindManager instance;

    //存储各个行为的键位绑定字典
    public Dictionary<string, KeyCode> keybindsDictionary;

    // 引用UI组件，显示和更新键位绑定
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

        //初始化键位字典
        keybindsDictionary = new Dictionary<string, KeyCode>();
    }

    private void Update()
    {
    }

    // 更新键位列表的语言（根据当前语言设置更新键位 UI）
    public void UpdateKeybindListLanguage()
    {
        keybindList.UpdateAllKeybindOptionsLanguage(); // 调用 KeybindList_UI 更新语言
    }

    //根据UI显示的行为键位名称，规范化键位名称（处理语言差异）
    public string UniformKeybindName(string _behaveKeybind_InUI)
    {
        //去除Alpha前缀
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5);
        }

        //英文
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
        //中文
        else if (LanguageManager.instance.localeID == 1)
        {
            if (_behaveKeybind_InUI.Equals("Mouse0"))
            {
                _behaveKeybind_InUI = "鼠标左键"; 
            }

            if (_behaveKeybind_InUI.Equals("Mouse1"))
            {
                _behaveKeybind_InUI = "鼠标右键";
            }

            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4);
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "左");
            }
        }

        //返回规范化后的键位名称
        return _behaveKeybind_InUI;
    }

    //从保存的数据中加载键位设置
    public void LoadData(SettingsData _data)
    {
        //遍历加载保存的数据并添加到键位字典中
        foreach (var search in _data.keybindsDictionary)
        {
            keybindsDictionary.Add(search.Key, search.Value); //将每个键位设置添加到字典中
        }
    }

    //将当前的键位设置保存到数据中
    public void SaveData(ref SettingsData _data)
    {
        _data.keybindsDictionary.Clear(); //清空现有的键位字典

        //遍历当前字典，将所有键位设置保存到数据中
        foreach (var search in keybindsDictionary)
        {
            _data.keybindsDictionary.Add(search.Key, search.Value); //将每个键位设置添加到保存的数据字典中
        }
    }
}
