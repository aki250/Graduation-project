using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillIconText_InGame_UI : MonoBehaviour
{
    //存储技能名称（用于查找对应的键位绑定）
    [SerializeField] private string skillName;

    //更新技能图标文本
    public void UpdateSkillIconText()
    {
        //获取与技能对应的键位绑定
        if (KeyBindManager.instance.keybindsDictionary.TryGetValue(skillName, out KeyCode keybind))
        {
            //更新UI显示对应按键名称
            GetComponent<TextMeshProUGUI>().text = UniformKeybindNameForInGameUI(keybind.ToString());

            //如果当前语言为中文
            if (LanguageManager.instance.localeID == 1)
            {
                GetComponent<TextMeshProUGUI>().fontSize = 22;
            }
            //如果当前语言为英文
            else if (LanguageManager.instance.localeID == 0)
            {
                GetComponent<TextMeshProUGUI>().fontSize = 26; 
            }
        }
    }

    //统一处理UI中的按键名称格式（处理语言和键位显示）
    private string UniformKeybindNameForInGameUI(string _behaveKeybind_InUI)
    {
        //如果按键名以Alpha开头，则移除
        if (_behaveKeybind_InUI.StartsWith("Alpha"))
        {
            _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 5); //移除前缀
        }
        //如果语言为英文
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

            //如果按键名称以 "Left" 开头，去掉 "Left" 显示剩下的部分
            if (_behaveKeybind_InUI.StartsWith("Left"))
            {
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(1, 3); // 移除Left
            }

            //转换为大写显示
            _behaveKeybind_InUI = _behaveKeybind_InUI.ToUpper();
        }

        //如果语言为中文
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
                _behaveKeybind_InUI = _behaveKeybind_InUI.Remove(0, 4); //移除Left
                _behaveKeybind_InUI = _behaveKeybind_InUI.Insert(0, "左"); //在开头插入左
            }
        }

        return _behaveKeybind_InUI; //返回标准化后按键名称
    }
}
