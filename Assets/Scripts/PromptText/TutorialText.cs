using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    private TextMeshPro tutorialText;   //显示教程文本

    [SerializeField] private List<string> boundBehaveNameList;  //存储绑定按键
    [TextArea][SerializeField] private string originalText;     //英文文本
    [TextArea][SerializeField] private string originalText_Chinese; //中文文本

    private void Awake()
    {
        tutorialText = GetComponent<TextMeshPro>();
    }


    private void Update()
    {
        UpdateTutorialText();
    }

    //根据语言设置和行为绑定名称动态更新文本
    private void UpdateTutorialText()
    {
        //将绑定名称插入到教程文本中
        string completedOriginalText = AddBehaveKeybindNameToTutorialText(originalText);
        string completedOriginalText_Chinese = AddBehaveKeybindNameToTutorialText(originalText_Chinese);

        //英语
        if (LanguageManager.instance.localeID == 0)
        {
            tutorialText.text = completedOriginalText;
        }
        //中文
        else if (LanguageManager.instance.localeID == 1)
        {
            tutorialText.text = completedOriginalText_Chinese;
        }
    }

    //将绑定的行为名称替换到教程文本中的占位符
    private string AddBehaveKeybindNameToTutorialText(string _tutorialText)
    {
        //遍历所有绑定的行为名称列表
        for (int i = 0; i < boundBehaveNameList.Count; i++)
        {
            //如果教程文本中包含特定占位符
            if (_tutorialText.Contains($"BehaveName{i}"))
            {
                //获取绑定的按键名称，并统一格式
                string _keybindName = KeyBindManager.instance.keybindsDictionary[boundBehaveNameList[i]].ToString();
                _keybindName = KeyBindManager.instance.UniformKeybindName(_keybindName);
                //获取绑定的按键名称，并统一格式
                _tutorialText = _tutorialText.Replace($"BehaveName{i}", _keybindName);
            }
        }

        return _tutorialText;
    }
}
