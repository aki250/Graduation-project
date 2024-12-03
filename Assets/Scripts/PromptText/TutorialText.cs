using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    private TextMeshPro tutorialText;   //��ʾ�̳��ı�

    [SerializeField] private List<string> boundBehaveNameList;  //�洢�󶨰���
    [TextArea][SerializeField] private string originalText;     //Ӣ���ı�
    [TextArea][SerializeField] private string originalText_Chinese; //�����ı�

    private void Awake()
    {
        tutorialText = GetComponent<TextMeshPro>();
    }


    private void Update()
    {
        UpdateTutorialText();
    }

    //�����������ú���Ϊ�����ƶ�̬�����ı�
    private void UpdateTutorialText()
    {
        //�������Ʋ��뵽�̳��ı���
        string completedOriginalText = AddBehaveKeybindNameToTutorialText(originalText);
        string completedOriginalText_Chinese = AddBehaveKeybindNameToTutorialText(originalText_Chinese);

        //Ӣ��
        if (LanguageManager.instance.localeID == 0)
        {
            tutorialText.text = completedOriginalText;
        }
        //����
        else if (LanguageManager.instance.localeID == 1)
        {
            tutorialText.text = completedOriginalText_Chinese;
        }
    }

    //���󶨵���Ϊ�����滻���̳��ı��е�ռλ��
    private string AddBehaveKeybindNameToTutorialText(string _tutorialText)
    {
        //�������а󶨵���Ϊ�����б�
        for (int i = 0; i < boundBehaveNameList.Count; i++)
        {
            //����̳��ı��а����ض�ռλ��
            if (_tutorialText.Contains($"BehaveName{i}"))
            {
                //��ȡ�󶨵İ������ƣ���ͳһ��ʽ
                string _keybindName = KeyBindManager.instance.keybindsDictionary[boundBehaveNameList[i]].ToString();
                _keybindName = KeyBindManager.instance.UniformKeybindName(_keybindName);
                //��ȡ�󶨵İ������ƣ���ͳһ��ʽ
                _tutorialText = _tutorialText.Replace($"BehaveName{i}", _keybindName);
            }
        }

        return _tutorialText;
    }
}
